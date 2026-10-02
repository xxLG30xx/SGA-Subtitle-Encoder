from __future__ import annotations

import argparse
import json
from pathlib import Path

from .core import BinaryComparison, FixedSizePatcher, SgaAnalyzer, decode_c1, extract_wav


def main() -> None:
    parser = argparse.ArgumentParser(prog="sga-subtitler")
    sub = parser.add_subparsers(dest="command", required=True)
    audit = sub.add_parser("audit")
    audit.add_argument("files", nargs="+")
    frames = sub.add_parser("frames")
    frames.add_argument("file")
    frames.add_argument("output")
    frames.add_argument("--count", type=int, default=3)
    audio = sub.add_parser("audio")
    audio.add_argument("file")
    audio.add_argument("output")
    patch = sub.add_parser("test-patch")
    patch.add_argument("file")
    patch.add_argument("output")
    patch.add_argument("--frames", type=int, default=3)
    args = parser.parse_args()
    analyzer = SgaAnalyzer()
    if args.command == "audit":
        print(json.dumps([analyzer.summary(f) for f in args.files], indent=2))
        return
    raw, chunks = analyzer.parse(args.file)
    if args.command == "audio":
        print(json.dumps(extract_wav(chunks, args.output), indent=2))
        return
    video = [c for c in chunks if c.type == 0xC1]
    if args.command == "frames":
        out = Path(args.output)
        out.mkdir(parents=True, exist_ok=True)
        palettes = None
        for i, chunk in enumerate(video[: args.count]):
            image, palettes = decode_c1(chunk, palettes)
            image.save(out / f"frame_{i:04d}.png")
        print(f"exported {min(args.count, len(video))} frame(s) to {out}")
        return
    if not video:
        raise SystemExit("test-patch currently targets the raw C1 profile")
    patched = raw
    palettes = None
    for chunk in video[: args.frames]:
        image, palettes = decode_c1(chunk, palettes)
        # Dependency-free 5x7 block glyphs are sufficient for this technical test.
        text = "SOUS-TITRE TEST"
        x, y = max(0, (image.width - len(text) * 6) // 2), max(0, image.height - 12)
        lit = set()
        for char_index, char in enumerate(text):
            code = ord(char)
            for gy in range(7):
                for gx in range(5):
                    if char != " " and ((code >> ((gx + gy * 3) % 7)) & 1):
                        lit.add((x + char_index * 6 + gx, y + gy))
        payload = bytearray(chunk.data)
        width = payload[6]
        for py in range(image.height):
            for px in range(image.width):
                if (px, py) not in lit:
                    continue
                tile = (py // 8) * width + px // 8
                offset = 8 + tile * 32 + (py % 8) * 4 + (px % 8) // 2
                if offset >= len(payload):
                    continue
                if px & 1:
                    payload[offset] = (payload[offset] & 0xF0) | 0x0F
                else:
                    payload[offset] = (payload[offset] & 0x0F) | 0xF0
        chunk_raw = bytearray(patched)
        patched = FixedSizePatcher().patch_payload(bytes(chunk_raw), chunk, bytes(payload))
    Path(args.output).write_bytes(patched)
    print(json.dumps(BinaryComparison.compare(raw, patched, chunks), indent=2))


if __name__ == "__main__":
    main()
