# Real SGA audit (V0.6)

This audit is generated from the four repository fixtures, not inferred from
their filenames. Run `python -m sga_subtitler.cli audit *.SGA` to reproduce the
structure table and use `frames`, `audio`, and `test-patch` for the decode,
audio, and fixed-size experiments.

## Measured container inventory

All inputs are exact multiples of the 2,048-byte CD-ROM sector size.

| File | Bytes | Sectors | Chunks | Observed types (count) | Video size |
|---|---:|---:|---:|---|---|
| Double Switch | 6,725,632 | 3,284 | 1,244 | A1 622; C9 557; CD 65 | 192×136 |
| Ground Zero Texas | 12,023,808 | 5,871 | 1,876 | A1 926; E8 89; E9 861 | 192×128 |
| Night Trap | 12,042,240 | 5,880 | 2,352 | A1 1,176; C1 1,176 | 168×104 |
| Sewer Shark | 10,133,504 | 4,948 | 2,240 | A1 1,120; C8 1,120 | 144×104 and 168×104 |

The machine-readable audit also records every logical payload span, offset,
reported chunk size, track, and timecode. Payloads crossing sectors exclude the
two-byte continuation headers, allowing surgical writes without moving data.

## SCAT correspondence

SCAT documents C1 as raw Mega Drive 4-bpp tiles; C6/C7/C8/C9 add the 8,192-byte
LZ window variants (C7/C9 use copy-count base 1, C8/C9 swap pixel nibbles on
alternating rows). CB/CD use a 4,096-byte LZ window, copy-count base 1, with CD
pixel swapping. E8 is the raw tri-frame container and E9 its compressed
interframe form. D1–D5/D7 are overlay/animation families, while 81/82/8A are
32-bit macroblock families. They are not interchangeable, and no unobserved
family is selected merely because SCAT supports it.

## Decode and audio experiment

The C1 decoder reaches the actual indexed tile pixels, reconstructs SCAT's
18-byte planar palettes and per-tile palette map, and emits dependency-free RGB
PNG files. The first three Night Trap frames were decoded at 168×104. Its A1
stream was exported as mono 8-bit PCM at 18,899 Hz (1,481,615 samples,
approximately 78.40 seconds); extraction reads audio but never rewrites it.

The first implementation deliberately targets **Night Trap's raw C1** profile.
It is the lowest-risk inverse operation: its video tiles are not LZ compressed,
so changing tile nibbles does not alter payload length. Sector headers, chunk
headers, padding, unknown bytes, and every audio span remain untouched. C8/C9
and CB/CD use distinct SCAT LZ parameters and pixel permutations; E8/E9 are
tri-frame/interframe formats and are intentionally not treated as C1.

Experimental products belong under `artifacts/` and never overwrite a source
SGA. A valid patch must have the original byte length; `FixedSizePatcher`
rejects any replacement payload whose length differs.

## First real fixed-size patch

The test modifies only three raw C1 payloads and writes `SOUS-TITRE TEST` into
tile nibbles. Because C1 is raw, there is no compression-size uncertainty and
no dependent interframe chain to rebuild. The measured comparison is:

```text
Original size:          12042240
Patched size:           12042240
Same size:              YES
Total bytes different:  546
First changed offset:   7557
Last changed offset:    29054
Total chunks:           2352
Unchanged chunks:       2349
Modified chunks:        3
Audio chunks modified:  0
Offsets preserved:      YES
Chunk sizes preserved:  YES
```

This is intentionally the minimum safe first profile. A future compressed
patch must fit the original payload allocation (using better LZ choices or a
cheaper glyph region); it must fail rather than shift following chunks.
