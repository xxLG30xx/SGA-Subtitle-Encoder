from __future__ import annotations

from collections import Counter
from dataclasses import dataclass
from pathlib import Path
import struct
import wave
import zlib

SECTOR_SIZE = 2048
VIDEO_TYPES = {0x81, 0x82, 0x8A, 0x99, 0xC1, 0xC2, 0xC4, 0xC6, 0xC7, 0xC8, 0xC9, 0xCB, 0xCD, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD7, 0xE7, 0xE8, 0xE9}
AUDIO_TYPES = {0xA1, 0xA2, 0xA3, 0xAA}
KNOWN_TYPES = VIDEO_TYPES | AUDIO_TYPES | {0xF0, 0xF1, 0xF2, 0xF3, 0xF9, 0xFF}


@dataclass(frozen=True)
class Span:
    offset: int
    length: int


@dataclass
class Chunk:
    index: int
    offset: int
    type: int
    track: int
    reported_length: int
    spans: list[Span]
    data: bytes

    @property
    def timecode(self) -> int:
        return int.from_bytes(self.data[:4], "big") if len(self.data) >= 4 else 0

    @property
    def dimensions(self) -> tuple[int, int] | None:
        if self.type in VIDEO_TYPES and len(self.data) >= 8:
            return self.data[6] * 8, self.data[7] * 8
        return None


class SgaAnalyzer:
    """Parse early Mega-CD sectorized SGA without rewriting the container."""

    def parse(self, path: str | Path) -> tuple[bytes, list[Chunk]]:
        raw = Path(path).read_bytes()
        chunks: list[Chunk] = []
        pos = 0
        primary_video = raw[0] if raw and raw[0] in VIDEO_TYPES else None
        # These four real Mega-CD samples pair their video with A1. Restricting
        # recovery to that observed profile prevents payload bytes resembling
        # unrelated A2/A3/AA headers from being promoted to chunks.
        allowed_types = {0xA1} | ({primary_video} if primary_video else VIDEO_TYPES)
        if primary_video in {0xE8, 0xE9}:
            allowed_types |= {0xE8, 0xE9}
        if primary_video in {0xC8, 0xC9, 0xCB, 0xCD}:
            allowed_types |= {0xC8, 0xC9, 0xCB, 0xCD}
        while pos + 4 <= len(raw):
            if pos & 1:
                pos += 1
            header = int.from_bytes(raw[pos : pos + 2], "big")
            if header == 0:
                pos = min(((pos // SECTOR_SIZE) + 1) * SECTOR_SIZE, len(raw))
                continue
            typ = header >> 8
            if not (header & 0x8000) or typ not in allowed_types:
                # Some samples contain orphan sector tails. They are not chunks;
                # retain them byte-for-byte and resume at the next plausible header.
                if pos % SECTOR_SIZE == 0 and not (header & 0xF000):
                    pos += 2 + (header & 0x0FFF)
                    continue
                nxt = self._next_header(raw, pos + 2, allowed_types)
                if nxt is None:
                    break
                pos = nxt
                continue
            length = int.from_bytes(raw[pos + 2 : pos + 4], "big")
            if length == 0:
                pos += 4
                continue
            start = pos
            pos += 4
            left = length
            spans: list[Span] = []
            payload = bytearray()
            valid = True
            while left:
                if pos >= len(raw):
                    valid = False
                    break
                if pos % SECTOR_SIZE == 0:
                    if pos + 2 > len(raw):
                        valid = False
                        break
                    continuation = int.from_bytes(raw[pos : pos + 2], "big")
                    if continuation & 0xF000:
                        valid = False
                        break
                    allowed = continuation & 0x0FFF
                    pos += 2
                    if allowed == 0:
                        pos = min(((pos // SECTOR_SIZE) + 1) * SECTOR_SIZE, len(raw))
                        continue
                else:
                    allowed = SECTOR_SIZE - (pos % SECTOR_SIZE)
                take = min(left, allowed, len(raw) - pos)
                if take <= 0:
                    valid = False
                    break
                spans.append(Span(pos, take))
                payload.extend(raw[pos : pos + take])
                pos += take
                left -= take
            if not valid:
                pos = start + 2
                continue
            chunks.append(Chunk(len(chunks), start, typ, header & 0xFF, length, spans, bytes(payload)))
            gap = SECTOR_SIZE - pos % SECTOR_SIZE if pos % SECTOR_SIZE else SECTOR_SIZE
            if gap < 12:
                pos += gap
        return raw, chunks

    @staticmethod
    def _next_header(raw: bytes, pos: int, allowed_types: set[int]) -> int | None:
        if pos & 1:
            pos += 1
        for candidate in range(pos, len(raw) - 3, 2):
            typ = raw[candidate]
            length = int.from_bytes(raw[candidate + 2 : candidate + 4], "big")
            if typ in allowed_types and raw[candidate + 1] < 0x20 and 8 <= length <= 0xFFFF:
                return candidate
        return None

    def summary(self, path: str | Path) -> dict:
        raw, chunks = self.parse(path)
        return {
            "file": Path(path).name,
            "size": len(raw),
            "sectors": (len(raw) + SECTOR_SIZE - 1) // SECTOR_SIZE,
            "chunks": len(chunks),
            "types": dict(sorted(Counter(f"{c.type:02X}" for c in chunks).items())),
            "tracks": sorted({c.track for c in chunks}),
            "first_offset": chunks[0].offset if chunks else None,
            "last_offset": chunks[-1].offset if chunks else None,
        }


def lz_decompress(data: bytes, count_mask: int = 0xE000, count_base: int = 0) -> bytes:
    shift = (count_mask & -count_mask).bit_length() - 1
    offset_mask = (~count_mask) & 0xFFFF
    out = bytearray()
    pos = 0
    while pos + 2 <= len(data):
        flags = int.from_bytes(data[pos : pos + 2], "big")
        pos += 2
        for bit in range(16):
            if pos + 2 > len(data):
                return bytes(out)
            word = int.from_bytes(data[pos : pos + 2], "big")
            pos += 2
            if not (flags & (1 << (15 - bit))):
                out.extend(word.to_bytes(2, "big"))
                continue
            count, offset = (word & count_mask) >> shift, word & offset_mask
            if count == 0 and offset == 0:
                out.extend(data[pos:])
                return bytes(out)
            count += count_base
            offset = max(offset, 1)
            for _ in range(count * 2):
                if offset > len(out):
                    raise ValueError("invalid LZ back-reference")
                out.append(out[-offset])
    return bytes(out)


def _palette(packed: bytes) -> list[tuple[int, int, int]]:
    colors = []
    for color in range(16):
        rgb = []
        for channel in range(3):
            value = 0
            for power in range(3):
                pair = packed[channel * 6 + power * 2 : channel * 6 + power * 2 + 2]
                bit = (pair[0] >> color) & 1 if color < 8 else (pair[1] >> (color - 8)) & 1
                value |= bit << power
            rgb.append(value * 36)
        colors.append(tuple(rgb))
    return colors


class RGBImage:
    def __init__(self, width: int, height: int):
        self.width, self.height = width, height
        self.pixels = bytearray(width * height * 3)

    @property
    def size(self) -> tuple[int, int]:
        return self.width, self.height

    def set(self, x: int, y: int, color: tuple[int, int, int]) -> None:
        at = (y * self.width + x) * 3
        self.pixels[at:at + 3] = bytes(color)

    def getcolors(self, maxcolors: int | None = None):
        colors = Counter(bytes(self.pixels[i:i + 3]) for i in range(0, len(self.pixels), 3))
        return [(count, tuple(color)) for color, count in colors.items()]

    def save(self, path: str | Path) -> None:
        def block(name: bytes, data: bytes) -> bytes:
            return struct.pack(">I", len(data)) + name + data + struct.pack(">I", zlib.crc32(name + data) & 0xFFFFFFFF)
        scanlines = b"".join(b"\0" + self.pixels[y * self.width * 3:(y + 1) * self.width * 3] for y in range(self.height))
        png = b"\x89PNG\r\n\x1a\n" + block(b"IHDR", struct.pack(">IIBBBBB", self.width, self.height, 8, 2, 0, 0, 0))
        png += block(b"IDAT", zlib.compress(scanlines, 9)) + block(b"IEND", b"")
        Path(path).write_bytes(png)


def decode_c1(chunk: Chunk, inherited_palettes: list[list[tuple[int, int, int]]] | None = None):
    """Decode a C1/C6/C7/C8/C9/CB/CD Mega Drive tile frame to RGB pixels."""
    data = chunk.data
    flags, palette_count, width, height = data[4:8]
    metadata = 10 if flags & 0x80 else 8
    body = data
    if chunk.type in {0xC6, 0xC7, 0xC8, 0xC9, 0xCB, 0xCD}:
        mask = 0xF000 if chunk.type in {0xCB, 0xCD} else 0xE000
        base = 1 if chunk.type in {0xC7, 0xC9, 0xCB, 0xCD} else 0
        body = data[:metadata] + lz_decompress(data[metadata:], mask, base)
    tile_count = int.from_bytes(body[8:10], "big") if flags & 0x80 else width * height
    tile_len = tile_count * 32
    palette_len = palette_count * 18
    if flags & 4:
        tile_offset, palette_offset = metadata, metadata + tile_len
    else:
        palette_offset, tile_offset = metadata, metadata + palette_len
    tiles = bytearray(body[tile_offset : tile_offset + tile_len])
    if chunk.type in {0xC8, 0xC9, 0xCD} and not flags & 0x80:
        for i in range(4, len(tiles), 8):
            for j in range(4):
                tiles[i + j] = (tiles[i + j] >> 4) | ((tiles[i + j] & 15) << 4)
    palettes = [_palette(body[palette_offset + i * 18 : palette_offset + (i + 1) * 18]) for i in range(palette_count)]
    if not palettes:
        palettes = inherited_palettes or [[(i * 17,) * 3 for i in range(16)]]
    palette_map_offset = palette_offset + palette_len
    palette_map = body[palette_map_offset:]
    image = RGBImage(width * 8, height * 8)
    bits = 1 if palette_count == 2 else 2
    for ty in range(height):
        for tx in range(width):
            ti = ty * width + tx
            pi = 0
            if palette_count > 1 and not flags & 0x80:
                per_byte = 8 // bits
                datum = palette_map[ti // per_byte] if ti // per_byte < len(palette_map) else 0
                pi = (datum >> (8 - bits - bits * (ti % per_byte))) & ((1 << bits) - 1)
                pi = min(pi, len(palettes) - 1)
            tile = tiles[ti * 32 : ti * 32 + 32]
            if len(tile) < 32:
                continue
            for y in range(8):
                for x in range(8):
                    value = tile[y * 4 + x // 2]
                    ci = value >> 4 if x % 2 == 0 else value & 15
                    image.set(tx * 8 + x, ty * 8 + y, palettes[pi][ci])
    return image, palettes


class FixedSizePatcher:
    def patch_payload(self, original: bytes, chunk: Chunk, payload: bytes) -> bytes:
        if len(payload) != chunk.reported_length:
            raise ValueError("fixed-size patch requires an exactly equal payload length")
        patched = bytearray(original)
        cursor = 0
        for span in chunk.spans:
            patched[span.offset : span.offset + span.length] = payload[cursor : cursor + span.length]
            cursor += span.length
        if len(patched) != len(original):
            raise AssertionError("output size changed")
        return bytes(patched)


class BinaryComparison:
    @staticmethod
    def compare(original: bytes, patched: bytes, chunks: list[Chunk]) -> dict:
        differences = [i for i, pair in enumerate(zip(original, patched)) if pair[0] != pair[1]]
        modified = []
        for chunk in chunks:
            if any(original[s.offset:s.offset+s.length] != patched[s.offset:s.offset+s.length] for s in chunk.spans):
                modified.append(chunk)
        return {
            "original_size": len(original), "patched_size": len(patched), "same_size": len(original) == len(patched),
            "total_bytes_different": len(differences), "first_changed_offset": differences[0] if differences else None,
            "last_changed_offset": differences[-1] if differences else None, "total_chunks": len(chunks),
            "unchanged_chunks": len(chunks) - len(modified), "modified_chunks": len(modified),
            "audio_chunks_modified": sum(c.type in AUDIO_TYPES for c in modified),
            "offsets_preserved": len(original) == len(patched), "chunk_sizes_preserved": len(original) == len(patched),
        }


def extract_wav(chunks: list[Chunk], output: str | Path) -> dict:
    audio = [c for c in chunks if c.type in AUDIO_TYPES]
    if not audio:
        raise ValueError("no audio chunks")
    first = audio[0]
    rate = int.from_bytes(first.data[4:6], "big") if first.type != 0xA1 else round((int.from_bytes(first.data[4:6], "big") & 0x0FFF) * 12_500_000 / 384 / 2048)
    rate = rate or 22050
    samples = bytearray()
    for chunk in audio:
        raw = chunk.data[8:]
        if raw.endswith(b"\0"):
            raw = raw[:-1]
        for sample in raw:
            sign, magnitude = sample >> 7, sample & 0x7F
            samples.append(sample if sign else 127 - magnitude)
    with wave.open(str(output), "wb") as wav:
        wav.setparams((1, 1, rate, len(samples), "NONE", "not compressed"))
        wav.writeframes(samples)
    return {"codec": "8-bit sign/magnitude PCM", "rate": rate, "channels": 1, "samples": len(samples), "duration": len(samples) / rate}
