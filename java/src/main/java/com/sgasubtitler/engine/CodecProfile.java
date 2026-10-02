package com.sgasubtitler.engine;

import java.util.List;
import java.util.Map;
import java.util.function.Function;
import java.util.stream.Collectors;

public record CodecProfile(int type, String name, Kind kind, int metadataLength,
                           int lzMask, int lzBase, boolean swapsPixels,
                           boolean realFixture, boolean decodeImplemented,
                           boolean fixedSizeSubtitling) {
    public enum Kind { RAW_TILES, LZ_TILES, OVERLAY, TRI_FRAME, MACROBLOCK, CONTAINER, AUDIO }
    public boolean compressedLz() { return lzMask != 0; }
    public static final Map<Integer, CodecProfile> REGISTRY = List.of(
        profile(0x81,"81",Kind.MACROBLOCK,8,0,0,false,false,false,false),
        profile(0x82,"82",Kind.MACROBLOCK,8,0,0,false,false,false,false),
        profile(0x8A,"8A",Kind.MACROBLOCK,8,0,0,false,false,false,false),
        profile(0xC1,"C1",Kind.RAW_TILES,8,0,0,false,true,true,true),
        profile(0xC2,"C2",Kind.RAW_TILES,12,0,0,false,false,false,false),
        profile(0xC4,"C4",Kind.OVERLAY,12,0,0,false,false,false,false),
        profile(0xC6,"C6",Kind.LZ_TILES,8,0xE000,0,true,false,true,true),
        profile(0xC7,"C7",Kind.LZ_TILES,8,0xE000,1,false,false,true,true),
        profile(0xC8,"C8",Kind.LZ_TILES,8,0xE000,0,true,true,true,true),
        profile(0xC9,"C9",Kind.LZ_TILES,8,0xE000,1,true,true,true,true),
        profile(0xCB,"CB",Kind.LZ_TILES,8,0xF000,1,false,false,true,true),
        profile(0xCD,"CD",Kind.OVERLAY,8,0xF000,1,true,true,true,true),
        profile(0xD1,"D1",Kind.OVERLAY,8,0,0,false,false,false,false),
        profile(0xD2,"D2",Kind.OVERLAY,8,0,0,false,false,false,false),
        profile(0xD3,"D3",Kind.OVERLAY,14,0,0,false,false,false,false),
        profile(0xD4,"D4",Kind.OVERLAY,14,0xE000,1,false,false,false,false),
        profile(0xD5,"D5",Kind.OVERLAY,8,0xE000,1,false,false,false,false),
        profile(0xD7,"D7",Kind.OVERLAY,8,0,0,false,false,false,false),
        profile(0xE7,"E7",Kind.TRI_FRAME,14,0xF000,1,true,false,false,false),
        profile(0xE8,"E8",Kind.TRI_FRAME,8,0,0,false,true,true,false),
        profile(0xE9,"E9",Kind.TRI_FRAME,8,0,0,false,true,true,false)
    ).stream().collect(Collectors.toUnmodifiableMap(CodecProfile::type, Function.identity()));
    private static CodecProfile profile(int type,String name,Kind kind,int metadata,int mask,int base,boolean swap,boolean fixture,boolean decode,boolean subtitle){return new CodecProfile(type,name,kind,metadata,mask,base,swap,fixture,decode,subtitle);}
    public static CodecProfile forType(int type) { return REGISTRY.get(type); }
}
