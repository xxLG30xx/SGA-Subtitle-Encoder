package com.sgasubtitler.engine;

import com.sgasubtitler.model.SgaChunk;
import com.sgasubtitler.model.SgaFile;

import java.awt.image.BufferedImage;
import java.io.IOException;
import java.util.HashMap;
import java.util.Map;
import java.util.Set;

/** Profile dispatcher retaining interframe state for E8/E9 previews. */
public final class VideoDecoder {
    private final Map<Integer, E8Codec.DecodedFrame> eFrames = new HashMap<>();

    public BufferedImage decode(SgaFile file, int frameIndex) throws IOException {
        SgaChunk chunk = file.videoChunks().get(frameIndex);
        if (Set.of(0xE8, 0xE9).contains(chunk.type())) return decodeE(file, frameIndex).image();
        CodecProfile profile = CodecProfile.forType(chunk.type());
        if (profile != null && Set.of(CodecProfile.Kind.RAW_TILES, CodecProfile.Kind.LZ_TILES, CodecProfile.Kind.OVERLAY).contains(profile.kind()))
            return new C1Codec().decode(chunk, null);
        throw new IOException("Décodage visuel non implémenté pour " + (profile == null ? "%02X".formatted(chunk.type()) : profile.name()));
    }

    private E8Codec.DecodedFrame decodeE(SgaFile file, int index) throws IOException {
        E8Codec.DecodedFrame cached = eFrames.get(index);
        if (cached != null) return cached;
        byte[] inherited = null;
        if (index > 0 && file.videoChunks().get(index).type() != 0xE8) inherited = decodeE(file, index - 1).frameData();
        E8Codec.DecodedFrame decoded = new E8Codec().decode(file.videoChunks().get(index), inherited);
        eFrames.put(index, decoded);
        return decoded;
    }

    public void clear() { eFrames.clear(); }
}
