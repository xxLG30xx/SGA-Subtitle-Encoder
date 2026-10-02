package com.sgasubtitler.engine;

import java.util.List;
import java.util.Set;

public final class GameProfileDatabase {
    public record Entry(String family, Set<Integer> chunks, String notes) {}
    public static final List<Entry> ENTRIES = List.of(
        new Entry("Night Trap (Mega-CD)", Set.of(0xC1,0xA1), "C1 brut, audio A1"),
        new Entry("Sewer Shark", Set.of(0xC8,0xA1), "C8 LZ, permutation de pixels"),
        new Entry("Double Switch", Set.of(0xC9,0xCD,0xA1), "C9 principal et overlays CD"),
        new Entry("Ground Zero Texas", Set.of(0xE8,0xE9,0xA1), "Tri-frames E8/E9"),
        new Entry("Slam City", Set.of(0xC2,0xC4,0xD4,0xF0,0xF1), "Interframes et containers"),
        new Entry("Supreme Warrior", Set.of(0xD2,0xD5,0xF1,0xFF), "Overlays D2/D5"),
        new Entry("Corpse Killer", Set.of(0xD3,0xD4), "Animations compressées"),
        new Entry("Make My Video", Set.of(0xC6,0xE7), "C6 et tri-frame E7"),
        new Entry("Ports 32-bit Digital Pictures", Set.of(0x81,0x82,0x8A), "Macroblocs indexés")
    );
    private GameProfileDatabase() {}
    public static String identify(Set<Integer> observed) { return ENTRIES.stream().filter(e -> observed.containsAll(e.chunks())).map(Entry::family).findFirst().orElse("Famille non déterminée"); }
}
