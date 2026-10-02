package com.sgasubtitler.engine;

import java.util.*;

/** Honest capability matrix consumed by both the UI and documentation/tests. */
public final class SupportMatrix {
    public record Status(boolean documented, boolean implementedFromScat, boolean decodeValidated,
                         boolean encodeValidated, boolean roundTrip, boolean preview,
                         boolean srt, boolean fixedSize, boolean realFixture, boolean realValidation) {
        public String display(String profile) {
            return profile + ": aperçu " + yes(preview) + ", réencodage " + yes(encodeValidated)
                + ", fixed-size " + yes(fixedSize) + ", validation réelle " + yes(realValidation);
        }
        private static String yes(boolean value) { return value ? "oui" : "non"; }
    }
    private static final Status DOCUMENTED = new Status(true,false,false,false,false,false,false,false,false,false);
    public static final Map<Integer, Status> PROFILES = create();
    private SupportMatrix() {}
    private static Map<Integer,Status> create() {
        Map<Integer,Status> map = new TreeMap<>();
        for (int type : CodecProfile.REGISTRY.keySet()) map.put(type, DOCUMENTED);
        Status experimentalLz = new Status(true,true,true,true,true,true,true,true,false,false);
        for (int type : new int[]{0xC6,0xC7,0xCB}) map.put(type, experimentalLz);
        Status complete = new Status(true,true,true,true,true,true,true,true,true,true);
        for (int type : new int[]{0xC1,0xC8,0xC9,0xCD}) map.put(type, complete);
        map.put(0xE8,new Status(true,true,true,true,true,true,true,true,true,true));
        map.put(0xE9,new Status(true,true,true,true,true,true,true,true,true,true));
        return Collections.unmodifiableMap(map);
    }
    public static Status forType(int type) { return PROFILES.getOrDefault(type, DOCUMENTED); }
}
