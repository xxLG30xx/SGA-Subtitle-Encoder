package com.sgasubtitler.engine;

import com.sgasubtitler.model.SgaChunk;

import java.awt.image.BufferedImage;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.util.Arrays;
import java.util.List;

/** SCAT E8/E9 byte-command tri/interframe decoder. */
public final class E8Codec {
    public record DecodedFrame(BufferedImage image, byte[] frameData) {}

    public DecodedFrame decode(SgaChunk chunk, byte[] inheritedFrame) throws IOException {
        int metadata = (chunk.data()[4] & 0x80) != 0 ? 10 : 8;
        byte[] frame = decompress(Arrays.copyOfRange(chunk.data(), metadata, chunk.data().length), inheritedFrame);
        byte[] logical = new byte[metadata + frame.length];
        System.arraycopy(chunk.data(), 0, logical, 0, metadata);
        System.arraycopy(frame, 0, logical, metadata, frame.length);
        SgaChunk raw = new SgaChunk(chunk.index(), chunk.offset(), 0xC1, chunk.track(), logical.length, List.of(), logical);
        return new DecodedFrame(new C1Codec().decode(raw, null), frame);
    }

    public byte[] decompress(byte[] compressed, byte[] inherited) throws IOException {
        ByteArrayOutputStream output = new ByteArrayOutputStream();
        for (int i = 0; i < compressed.length; i++) {
            int code = compressed[i] & 255;
            if (code == 0) {
                if (i + 32 >= compressed.length) throw new IOException("Bloc littéral E8 tronqué");
                output.write(compressed, i + 1, 32); i += 32;
            } else if (code >= 1 && code <= 15) {
                if (i + 4 >= compressed.length) throw new IOException("Flags E8 tronqués");
                long flags = 0; for (int j = 0; j < 4; j++) flags = flags << 8 | (compressed[++i] & 255L);
                for (int bit = 31; bit >= 0; bit--) {
                    if ((flags & (1L << bit)) == 0) {
                        if (++i >= compressed.length) throw new IOException("Littéral E8 tronqué");
                        output.write(compressed[i]);
                    } else {
                        int offset = switch (code) { case 1,6 -> 1; case 2,8 -> 4; case 3,10 -> 8; case 4,12 -> 32; case 5 -> 0; case 7 -> -1; case 9 -> -4; case 11 -> -8; case 13 -> -32; case 14 -> 64; case 15 -> -64; default -> 0; };
                        byte[] source = code >= 5 ? inherited : output.toByteArray();
                        if (source == null || source.length == 0) output.write(0);
                        else { int sourceIndex = Math.min(source.length - 1, output.size() - offset); output.write(source[Math.max(0, sourceIndex)]); }
                    }
                }
            } else if (code == 0xff) {
                if ((i & 1) == 0) i++;
                if (compressed.length - (i + 1) == 32) { output.write(compressed, i + 1, 32); break; }
            } else output.write(code);
        }
        return output.toByteArray();
    }

    /** Encodes 32-byte command groups, selecting the cheapest SCAT reference mode per group. */
    public byte[] encode(byte[] frame, byte[] inherited) {
        byte[] padded = Arrays.copyOf(frame, ((frame.length + 31) / 32) * 32);
        int[] cost = new int[padded.length + 1];
        Candidate[] choice = new Candidate[padded.length];
        for (int start = padded.length - 1; start >= 0; start--) {
            cost[start] = Integer.MAX_VALUE / 4;
            int value = padded[start] & 255;
            if (value >= 16 && value != 0xff) { cost[start] = 1 + cost[start + 1]; choice[start] = new Candidate(value, 0, -1); }
            if (start + 32 > padded.length) continue;
            Candidate best = new Candidate(0, 0, 32);
            for (int code = 1; code <= 15; code++) {
                long flags = 0;
                int literals = 0;
                for (int j = 0; j < 32; j++) {
                    if (referenceMatches(code, padded, inherited, start + j)) flags |= 1L << (31 - j);
                    else literals++;
                }
                if (5 + literals < 1 + best.literals) best = new Candidate(code, flags, literals);
            }
            int blockCost = (best.code == 0 ? 33 : 5 + best.literals) + cost[start + 32];
            if (blockCost < cost[start]) { cost[start] = blockCost; choice[start] = best; }
        }
        ByteArrayOutputStream output = new ByteArrayOutputStream(cost[0]);
        for (int start = 0; start < padded.length;) {
            Candidate best = choice[start];
            if (best.literals == -1) { output.write(padded[start++]); continue; }
            if (best.code == 0) {
                output.write(0); output.write(padded, start, 32);
            } else {
                output.write(best.code);
                long flags = best.flags;
                for (int shift = 24; shift >= 0; shift -= 8) output.write((int) (flags >>> shift));
                for (int j = 0; j < 32; j++) {
                    boolean reference = (flags & (1L << (31 - j))) != 0;
                    if (!reference) output.write(padded[start + j]);
                }
            }
            start += 32;
        }
        return output.toByteArray();
    }

    /** Fits a stream exactly; harmless decoded padding follows the logical frame. */
    public byte[] fit(byte[] frame, byte[] inherited, int allocation) {
        if ((allocation & 1) != 0) return null;
        byte[] encoded = encode(frame, inherited);
        int terminalSize = 33;
        int terminalAt = allocation - terminalSize;
        if (encoded.length > terminalAt) return null;
        byte[] fixed = new byte[allocation];
        System.arraycopy(encoded, 0, fixed, 0, encoded.length);
        Arrays.fill(fixed, encoded.length, terminalAt, (byte) 0x10);
        fixed[terminalAt] = (byte) 0xff;
        // When FF is even the decoder consumes one alignment byte before the 32-byte tail.
        return fixed;
    }

    public boolean dependsOnInherited(byte[] compressed) {
        for (int i = 0; i < compressed.length; i++) {
            int code = compressed[i] & 255;
            if (code == 0) { i += 32; continue; }
            if (code >= 1 && code <= 15) {
                if (i + 4 >= compressed.length) return false;
                long flags = 0; for (int j = 0; j < 4; j++) flags = flags << 8 | (compressed[++i] & 255L);
                if (code >= 5 && flags != 0) return true;
                i += 32 - Long.bitCount(flags);
            } else if (code == 0xff) return false;
        }
        return false;
    }

    private static boolean referenceMatches(int code, byte[] frame, byte[] inherited, int position) {
        int offset = switch (code) { case 1,6 -> 1; case 2,8 -> 4; case 3,10 -> 8; case 4,12 -> 32; case 5 -> 0; case 7 -> -1; case 9 -> -4; case 11 -> -8; case 13 -> -32; case 14 -> 64; case 15 -> -64; default -> 0; };
        byte[] source = code >= 5 ? inherited : frame;
        if (source == null || source.length == 0) return frame[position] == 0;
        int sourceIndex = Math.min(source.length - 1, position - offset);
        return sourceIndex >= 0 && frame[position] == source[sourceIndex];
    }

    private record Candidate(int code, long flags, int literals) {}
}
