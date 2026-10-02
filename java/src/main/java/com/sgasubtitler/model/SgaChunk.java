package com.sgasubtitler.model;

import java.util.List;

public record SgaChunk(int index, long offset, int type, int track, int reportedLength,
                       List<Span> spans, byte[] data) {
    public record Span(long offset, int length) {}
    public boolean isAudio() { return type == 0xA1 || type == 0xA2 || type == 0xA3 || type == 0xAA; }
    public boolean isVideo() { return switch (type) { case 0x81,0x82,0x8A,0x99,0xC1,0xC2,0xC4,0xC6,0xC7,0xC8,0xC9,0xCB,0xCD,0xD1,0xD2,0xD3,0xD4,0xD5,0xD7,0xE7,0xE8,0xE9 -> true; default -> false; }; }
    public int width() { return data.length >= 8 ? Byte.toUnsignedInt(data[6]) * 8 : 0; }
    public int height() { return data.length >= 8 ? Byte.toUnsignedInt(data[7]) * 8 : 0; }
}
