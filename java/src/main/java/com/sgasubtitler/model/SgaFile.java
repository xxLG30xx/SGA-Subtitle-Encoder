package com.sgasubtitler.model;

import java.nio.file.Path;
import java.util.*;
import java.util.stream.Collectors;
import com.sgasubtitler.engine.CodecProfile;
import com.sgasubtitler.engine.GameProfileDatabase;
import com.sgasubtitler.engine.SupportMatrix;

public record SgaFile(Path path, byte[] bytes, List<SgaChunk> chunks) {
    public List<SgaChunk> videoChunks() { return chunks.stream().filter(SgaChunk::isVideo).toList(); }
    public List<SgaChunk> audioChunks() { return chunks.stream().filter(SgaChunk::isAudio).toList(); }
    public String profile() { return videoChunks().stream().map(c -> "%02X".formatted(c.type())).distinct().collect(Collectors.joining(" + ")); }
    public String chunkSummary() { return chunks.stream().collect(Collectors.groupingBy(c -> "%02X".formatted(c.type()), TreeMap::new, Collectors.counting())).toString(); }
    public String knownFamily() { return GameProfileDatabase.identify(chunks.stream().map(SgaChunk::type).collect(Collectors.toSet())); }
    public String supportSummary() { return videoChunks().stream().map(SgaChunk::type).distinct().map(type -> SupportMatrix.forType(type).display("%02X".formatted(type))).collect(Collectors.joining("; ")); }
    public double frameRate() { return audioChunks().isEmpty() ? 15.0 : 1.0 / AudioInfo.of(audioChunks().getFirst()).chunkDuration(); }
    public double durationSeconds() { return videoChunks().size() / frameRate(); }
    public record AudioInfo(int rate, int channels, String codec, int samples) {
        public static AudioInfo of(SgaChunk c) { int mult=((c.data()[4]&15)<<8)|(c.data()[5]&255); int rate=(int)Math.round(mult*(12_500_000d/384d/2048d)); return new AudioInfo(rate,1,"PCM 8-bit signe/magnitude",Math.max(0,c.data().length-8)); }
        public double chunkDuration() { return samples/(double)rate; }
    }
}
