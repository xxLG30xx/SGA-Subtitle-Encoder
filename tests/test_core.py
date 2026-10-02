from pathlib import Path

import pytest

from sga_subtitler.core import BinaryComparison, FixedSizePatcher, SgaAnalyzer, decode_c1

ROOT = Path(__file__).parents[1]


def test_all_samples_parse_and_have_expected_size():
    expected = {
        "Double Switch.SGA": 6725632,
        "Ground Zero Texas.SGA": 12023808,
        "Night Trap.SGA": 12042240,
        "Sewer Shark.SGA": 10133504,
    }
    analyzer = SgaAnalyzer()
    for name, size in expected.items():
        raw, chunks = analyzer.parse(ROOT / name)
        assert len(raw) == size
        assert chunks
        assert all(c.reported_length == sum(s.length for s in c.spans) for c in chunks)


def test_night_trap_reaches_real_pixels():
    _, chunks = SgaAnalyzer().parse(ROOT / "Night Trap.SGA")
    image, palettes = decode_c1(next(c for c in chunks if c.type == 0xC1))
    assert image.size == (168, 104)
    assert len(image.getcolors(maxcolors=image.width * image.height)) > 16
    assert len(palettes) == 4


def test_fixed_size_patch_and_audio_immutability():
    raw, chunks = SgaAnalyzer().parse(ROOT / "Night Trap.SGA")
    video = next(c for c in chunks if c.type == 0xC1)
    payload = bytearray(video.data)
    payload[100] ^= 1
    patched = FixedSizePatcher().patch_payload(raw, video, payload)
    report = BinaryComparison.compare(raw, patched, chunks)
    assert report["same_size"]
    assert report["total_bytes_different"] == 1
    assert report["audio_chunks_modified"] == 0
    with pytest.raises(ValueError):
        FixedSizePatcher().patch_payload(raw, video, payload + b"x")
