package com.sgasubtitler.engine;

import java.io.ByteArrayOutputStream;
import java.util.ArrayDeque;
import java.util.HashMap;
import java.util.Map;

/** Word-oriented LZ encoder matching SCAT's 16-bit flag stream. */
public final class LzEncoder {
    private LzEncoder() {}

    public static byte[] encode(byte[] input, int lengthMask, int lengthBase) {
        byte[] best = encodeUntil(input, lengthMask, lengthBase, input.length);
        // SCAT streams may end compression early: the 0000 pair terminates LZ
        // and every remaining byte is copied verbatim by the decoder.
        for (int cutoff = 0; cutoff < input.length; cutoff += 32) {
            byte[] candidate = encodeUntil(input, lengthMask, lengthBase, cutoff);
            if (candidate.length < best.length) best = candidate;
        }
        return best;
    }

    private static byte[] encodeUntil(byte[] input, int lengthMask, int lengthBase, int cutoff) {
        int shift = Integer.numberOfTrailingZeros(lengthMask);
        int maxUnits = (lengthMask >>> shift) + lengthBase;
        int maxOffset = (~lengthMask) & 0xffff;
        ByteArrayOutputStream output = new ByteArrayOutputStream(input.length);
        Map<Integer, ArrayDeque<Integer>> positions = new HashMap<>();
        int cursor = 0;
        boolean finished = false;
        while (!finished) {
            int flags = 0;
            ByteArrayOutputStream words = new ByteArrayOutputStream(32);
            for (int token = 0; token < 16; token++) {
                if (cursor >= input.length || cursor >= cutoff) {
                    flags |= 1 << (15 - token);
                    words.write(0); words.write(0); // End marker.
                    finished = true;
                    break;
                }
                Match match = find(input, cursor, positions, maxUnits, maxOffset, lengthBase);
                if (match.units >= Math.max(1, lengthBase)) {
                    flags |= 1 << (15 - token);
                    int encodedLength = match.units - lengthBase;
                    int word = (encodedLength << shift) | match.offset;
                    words.write(word >>> 8); words.write(word);
                    for (int i = 0; i < match.units * 2; i++) addPosition(input, cursor + i, positions, maxOffset);
                    cursor += match.units * 2;
                } else {
                    words.write(input[cursor]);
                    words.write(cursor + 1 < input.length ? input[cursor + 1] : 0);
                    int literalLength = Math.min(2, input.length - cursor);
                    for (int i = 0; i < literalLength; i++) addPosition(input, cursor + i, positions, maxOffset);
                    cursor += literalLength;
                }
            }
            output.write(flags >>> 8); output.write(flags);
            output.writeBytes(words.toByteArray());
            if (finished && cursor < input.length) output.write(input, cursor, input.length - cursor);
        }
        return output.toByteArray();
    }

    public static byte[] fit(byte[] input, int allocation, int mask, int base) {
        byte[] encoded = encode(input, mask, base);
        if (encoded.length > allocation) return null;
        byte[] fixed = new byte[allocation];
        System.arraycopy(encoded, 0, fixed, 0, encoded.length);
        return fixed;
    }

    private static Match find(byte[] data, int cursor, Map<Integer, ArrayDeque<Integer>> positions,
                              int maxUnits, int maxOffset, int lengthBase) {
        if (cursor + 1 >= data.length) return Match.NONE;
        ArrayDeque<Integer> candidates = positions.get(key(data, cursor));
        if (candidates == null) return Match.NONE;
        int bestUnits = 0, bestOffset = 0;
        for (var iterator = candidates.descendingIterator(); iterator.hasNext();) {
            int previous = iterator.next(), offset = cursor - previous;
            if (offset <= 0 || offset > maxOffset) continue;
            int bytes = 0, limit = Math.min(maxUnits * 2, data.length - cursor);
            while (bytes < limit && data[cursor + bytes] == data[cursor + bytes - offset]) bytes++;
            int units = bytes / 2;
            if (units > bestUnits && units >= Math.max(1, lengthBase)) {
                bestUnits = units; bestOffset = offset;
                if (units == maxUnits) break;
            }
        }
        return new Match(bestUnits, bestOffset);
    }

    private static void addPosition(byte[] data, int cursor, Map<Integer, ArrayDeque<Integer>> positions, int window) {
        if (cursor + 1 >= data.length) return;
        ArrayDeque<Integer> queue = positions.computeIfAbsent(key(data, cursor), ignored -> new ArrayDeque<>());
        queue.addLast(cursor);
        while (!queue.isEmpty() && cursor - queue.getFirst() > window) queue.removeFirst();
    }

    private static int key(byte[] data, int cursor) { return ((data[cursor] & 255) << 8) | (data[cursor + 1] & 255); }
    private record Match(int units, int offset) { private static final Match NONE = new Match(0, 0); }
}
