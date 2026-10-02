package com.sgasubtitler.engine;

import com.sgasubtitler.model.*;

import java.io.IOException;
import java.nio.file.*;
import java.util.*;

/** Minimal dependency-chain fixed-size encoder for E8 keyframes and E9 interframes. */
public final class E8SubtitleEncoder {
    public C1SubtitleEncoder.Report encode(SgaFile file, List<Subtitle> subtitles, SubtitleStyle style, Path output) throws IOException {
        if (file.videoChunks().isEmpty() || file.videoChunks().stream().anyMatch(c -> c.type() != 0xE8 && c.type() != 0xE9))
            throw new IOException("Ce moteur exige un flux E8/E9 pur");
        if (output.toAbsolutePath().normalize().equals(file.path().toAbsolutePath().normalize()))
            throw new IOException("Le fichier source ne peut jamais être écrasé");
        if (subtitles.isEmpty()) throw new IOException("Aucun sous-titre SRT chargé");

        List<SgaChunk> video = file.videoChunks();
        double fps = file.frameRate();
        BitSet requested = new BitSet(video.size());
        for (int i = 0; i < video.size(); i++) {
            long millis = Math.round(i * 1000d / fps);
            int index = i;
            if (subtitles.stream().anyMatch(s -> s.activeAt(millis))) requested.set(index);
        }
        if (requested.isEmpty()) throw new IOException("Aucune frame ne correspond aux intervalles SRT");

        BitSet recode = new BitSet(video.size());
        for (int start = requested.nextSetBit(0); start >= 0; start = requested.nextSetBit(start + 1)) {
            int end = start + 1;
            while (end < video.size() && video.get(end).type() != 0xE8) end++;
            recode.set(start, end);
        }

        E8Codec codec = new E8Codec();
        C1SubtitleEncoder tileEncoder = new C1SubtitleEncoder();
        byte[] patched = file.bytes().clone(), previousOriginal = null, previousPatched = null;
        int modified = 0;
        for (int i = 0; i < video.size(); i++) {
            SgaChunk chunk = video.get(i);
            int metadata = (chunk.data()[4] & 0x80) != 0 ? 10 : 8;
            byte[] inheritedOriginal = chunk.type() == 0xE8 ? null : previousOriginal;
            byte[] originalFrame = codec.decompress(Arrays.copyOfRange(chunk.data(), metadata, chunk.data().length), inheritedOriginal);
            byte[] desiredFrame = originalFrame;
            if (requested.get(i)) {
                long millis = Math.round(i * 1000d / fps);
                Subtitle subtitle = subtitles.stream().filter(s -> s.activeAt(millis)).findFirst().orElseThrow();
                byte[] logical = new byte[metadata + originalFrame.length];
                System.arraycopy(chunk.data(), 0, logical, 0, metadata);
                System.arraycopy(originalFrame, 0, logical, metadata, originalFrame.length);
                SgaChunk raw = new SgaChunk(chunk.index(), chunk.offset(), 0xC1, chunk.track(), logical.length, List.of(), logical);
                byte[] rendered = tileEncoder.encodeChunk(raw, subtitle.text(), style);
                desiredFrame = Arrays.copyOfRange(rendered, metadata, rendered.length);
            }
            if (recode.get(i)) {
                byte[] inheritedPatched = chunk.type() == 0xE8 ? null : previousPatched;
                byte[] encoded = codec.fit(desiredFrame, inheritedPatched, chunk.data().length - metadata);
                if (encoded == null) throw new IOException("La chaîne E8/E9 ne tient pas au chunk " + chunk.index() + " (frame " + i + ")");
                byte[] payload = chunk.data().clone();
                System.arraycopy(encoded, 0, payload, metadata, encoded.length);
                byte[] verify = codec.decompress(encoded, inheritedPatched);
                if (!Arrays.equals(desiredFrame, Arrays.copyOf(verify, desiredFrame.length)))
                    throw new IOException("Échec du round-trip E8/E9 au chunk " + chunk.index());
                writePayload(patched, chunk, payload);
                modified++;
            }
            previousOriginal = originalFrame;
            previousPatched = desiredFrame;
        }
        if (patched.length != file.bytes().length) throw new IOException("Violation fixed-size E8/E9");
        ensureAudioUnchanged(file, file.bytes(), patched);
        Files.write(output, patched, StandardOpenOption.CREATE_NEW);
        long different = 0, first = -1, last = -1;
        for (int i = 0; i < patched.length; i++) if (patched[i] != file.bytes()[i]) { if (first < 0) first = i; last = i; different++; }
        return new C1SubtitleEncoder.Report(file.bytes().length, patched.length, different, first, last, modified, 0, true);
    }

    private static void writePayload(byte[] destination, SgaChunk chunk, byte[] payload) throws IOException {
        if (payload.length != chunk.reportedLength()) throw new IOException("Payload E8/E9 non fixed-size");
        int cursor = 0; for (SgaChunk.Span span : chunk.spans()) { System.arraycopy(payload, cursor, destination, (int) span.offset(), span.length()); cursor += span.length(); }
    }
    private static void ensureAudioUnchanged(SgaFile file, byte[] original, byte[] patched) throws IOException {
        for (SgaChunk chunk : file.audioChunks()) for (SgaChunk.Span span : chunk.spans()) for (int i = 0; i < span.length(); i++)
            if (original[(int) span.offset() + i] != patched[(int) span.offset() + i]) throw new IOException("Sécurité: l'audio a été modifié");
    }
}
