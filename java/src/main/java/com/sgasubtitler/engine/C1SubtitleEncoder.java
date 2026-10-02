package com.sgasubtitler.engine;

import com.sgasubtitler.model.SgaChunk;
import com.sgasubtitler.model.SgaFile;
import com.sgasubtitler.model.Subtitle;
import com.sgasubtitler.model.SubtitleStyle;

import java.awt.image.BufferedImage;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.StandardOpenOption;
import java.util.Arrays;
import java.util.List;

/** Fixed-size subtitle encoder shared by raw and SCAT LZ tile profiles. */
public final class C1SubtitleEncoder {
    public record Report(long originalSize, long outputSize, long differentBytes,
                         long firstOffset, long lastOffset, int modifiedChunks,
                         int audioModified, boolean offsetsPreserved) {}

    public Report encode(SgaFile file, List<Subtitle> subtitles, SubtitleStyle style, Path output) throws IOException {
        if (file.videoChunks().isEmpty() || file.videoChunks().stream().anyMatch(c -> !supports(c.type())))
            throw new IOException("Profil non pris en charge par l'encodeur fixed-size de tuiles");
        if (output.toAbsolutePath().normalize().equals(file.path().toAbsolutePath().normalize()))
            throw new IOException("Le fichier source ne peut jamais être écrasé");
        if (subtitles.isEmpty()) throw new IOException("Aucun sous-titre SRT chargé");

        byte[] patched = file.bytes().clone(), original = file.bytes();
        double fps = file.frameRate();
        int modified = 0;
        for (int frame = 0; frame < file.videoChunks().size(); frame++) {
            long millis = Math.round(frame * 1000d / fps);
            Subtitle subtitle = subtitles.stream().filter(s -> s.activeAt(millis)).findFirst().orElse(null);
            if (subtitle == null) continue;
            SgaChunk chunk = file.videoChunks().get(frame);
            byte[] payload = encodeChunk(chunk, subtitle.text(), style);
            writePayload(patched, chunk, payload);
            modified++;
        }
        if (modified == 0) throw new IOException("Aucune frame ne correspond aux intervalles SRT");
        if (patched.length != original.length) throw new IOException("Violation fixed-size");
        int audioChanged = countModifiedAudio(file, original, patched);
        if (audioChanged != 0) throw new IOException("Sécurité: l'audio a été modifié");
        Files.write(output, patched, StandardOpenOption.CREATE_NEW);

        long count = 0, first = -1, last = -1;
        for (int i = 0; i < original.length; i++) if (original[i] != patched[i]) {
            if (first < 0) first = i;
            last = i;
            count++;
        }
        return new Report(original.length, patched.length, count, first, last, modified, audioChanged, true);
    }

    public byte[] encodeChunk(SgaChunk chunk, String text, SubtitleStyle style) throws IOException {
        CodecProfile profile = CodecProfile.forType(chunk.type());
        int metadata = profile.metadataLength();
        byte[] source = chunk.data();
        byte[] logical;
        if (profile.compressedLz()) {
            byte[] expanded = LzDecoder.decode(Arrays.copyOfRange(source, metadata, source.length), profile.lzMask(), profile.lzBase());
            logical = new byte[metadata + expanded.length];
            System.arraycopy(source, 0, logical, 0, metadata);
            System.arraycopy(expanded, 0, logical, metadata, expanded.length);
        } else logical = source.clone();

        int flags = logical[4] & 255;
        if ((flags & 128) != 0) throw new IOException(profile.name() + " avec tile-map non pris en charge en écriture");
        int paletteCount = logical[5] & 255, width = logical[6] & 255, height = logical[7] & 255;
        int tileLength = width * height * 32, paletteLength = paletteCount * 18;
        int tileOffset = (flags & 4) != 0 ? metadata : metadata + paletteLength;
        int paletteOffset = (flags & 4) != 0 ? metadata + tileLength : metadata;
        if (tileOffset + tileLength > logical.length) throw new IOException("Payload " + profile.name() + " tronqué au chunk " + chunk.index());

        if (profile.swapsPixels() && (flags & 128) == 0) swapRows(logical, tileOffset, tileLength);
        int[][] palettes = decodePalettes(logical, paletteOffset, paletteCount);
        byte[] paletteMap = Arrays.copyOfRange(logical, Math.min(logical.length, paletteOffset + paletteLength), logical.length);
        BufferedImage mask = new SubtitleRenderer().mask(width * 8, height * 8, text, style);
        for (int y = 0; y < height * 8; y++) for (int x = 0; x < width * 8; x++) {
            int luminance = mask.getRaster().getSample(x, y, 0);
            if (luminance == 0) continue;
            int tile = (y / 8) * width + x / 8;
            int at = tileOffset + tile * 32 + (y % 8) * 4 + (x % 8) / 2;
            int palette = paletteIndex(paletteMap, paletteCount, tile);
            int color = bestColor(palettes[Math.min(palette, palettes.length - 1)], luminance > 128);
            logical[at] = (byte) ((x & 1) == 0 ? ((logical[at] & 15) | (color << 4)) : ((logical[at] & 240) | color));
        }
        if (profile.swapsPixels() && (flags & 128) == 0) swapRows(logical, tileOffset, tileLength);
        if (!profile.compressedLz()) return logical;

        byte[] compressed = LzEncoder.fit(Arrays.copyOfRange(logical, metadata, logical.length),
            source.length - metadata, profile.lzMask(), profile.lzBase());
        if (compressed == null)
            throw new IOException("Le sous-titre ne tient pas dans l'allocation compressée " + profile.name() + " du chunk " + chunk.index());
        byte[] output = source.clone();
        System.arraycopy(compressed, 0, output, metadata, compressed.length);
        return output;
    }

    private static boolean supports(int type) {
        CodecProfile p = CodecProfile.forType(type);
        return p != null && p.fixedSizeSubtitling() && (p.kind() == CodecProfile.Kind.RAW_TILES || p.kind() == CodecProfile.Kind.LZ_TILES || p.kind() == CodecProfile.Kind.OVERLAY);
    }
    private static void swapRows(byte[] data, int offset, int length) { for (int i = 4; i < length; i += 8) for (int j = 0; j < 4 && i + j < length; j++) { int at = offset + i + j, value = data[at] & 255; data[at] = (byte) ((value >>> 4) | ((value & 15) << 4)); } }
    private static int paletteIndex(byte[] map, int count, int tile) { if (count <= 1) return 0; int bits = count == 2 ? 1 : 2, per = 8 / bits, at = tile / per; return at < map.length ? ((map[at] & 255) >>> (8 - bits - bits * (tile % per))) & ((1 << bits) - 1) : 0; }
    private static int[][] decodePalettes(byte[] data, int offset, int count) { if (count == 0) return new int[][] { grayscale() }; int[][] result = new int[count][16]; for (int p = 0; p < count; p++) for (int color = 0; color < 16; color++) { int rgb = 0; for (int channel = 0; channel < 3; channel++) { int value = 0; for (int power = 0; power < 3; power++) { int at = offset + p * 18 + channel * 6 + power * 2; if (at + 1 < data.length) { int bit = color < 8 ? ((data[at] & 255) >>> color) & 1 : ((data[at + 1] & 255) >>> (color - 8)) & 1; value |= bit << power; } } rgb |= value * 36 << (16 - channel * 8); } result[p][color] = rgb; } return result; }
    private static int[] grayscale() { int[] colors = new int[16]; for (int i = 0; i < 16; i++) colors[i] = (i * 17 << 16) | (i * 17 << 8) | i * 17; return colors; }
    private static int bestColor(int[] palette, boolean brightest) { int best = 0, score = brightest ? -1 : Integer.MAX_VALUE; for (int i = 0; i < palette.length; i++) { int rgb = palette[i], value = ((rgb >> 16) & 255) * 3 + ((rgb >> 8) & 255) * 6 + (rgb & 255); if ((brightest && value > score) || (!brightest && value < score)) { best = i; score = value; } } return best; }
    private static void writePayload(byte[] destination, SgaChunk chunk, byte[] payload) throws IOException { if (payload.length != chunk.reportedLength()) throw new IOException("Payload non fixed-size"); int cursor = 0; for (SgaChunk.Span span : chunk.spans()) { System.arraycopy(payload, cursor, destination, (int) span.offset(), span.length()); cursor += span.length(); } }
    private static int countModifiedAudio(SgaFile file, byte[] original, byte[] patched) { int changed = 0; for (SgaChunk chunk : file.audioChunks()) { boolean differs = false; for (SgaChunk.Span span : chunk.spans()) for (int i = 0; i < span.length(); i++) if (original[(int) span.offset() + i] != patched[(int) span.offset() + i]) { differs = true; break; } if (differs) changed++; } return changed; }
}
