package com.sgasubtitler.engine;

import com.sgasubtitler.model.*;
import java.io.*; import java.nio.file.*; import java.util.*;

public final class SgaParser {
    public static final int SECTOR=2048;
    private static final Set<Integer> VIDEO=Set.of(0x81,0x82,0x8A,0x99,0xC1,0xC2,0xC4,0xC6,0xC7,0xC8,0xC9,0xCB,0xCD,0xD1,0xD2,0xD3,0xD4,0xD5,0xD7,0xE7,0xE8,0xE9);
    public SgaFile parse(Path path) throws IOException {
        byte[] raw=Files.readAllBytes(path); List<SgaChunk> out=new ArrayList<>(); int pos=0;
        int primary=raw.length>0?raw[0]&255:-1; Set<Integer> allowed=new HashSet<>(Set.of(0xA1)); allowed.add(primary);
        if(primary==0xE8||primary==0xE9) allowed.addAll(Set.of(0xE8,0xE9));
        if(Set.of(0xC8,0xC9,0xCB,0xCD).contains(primary)) allowed.addAll(Set.of(0xC8,0xC9,0xCB,0xCD));
        while(pos+4<=raw.length) {
            if((pos&1)!=0) pos++; int header=u16(raw,pos);
            if(header==0){pos=Math.min(((pos/SECTOR)+1)*SECTOR,raw.length);continue;}
            int type=header>>>8;
            if((header&0x8000)==0||!allowed.contains(type)) {
                if(pos%SECTOR==0&&(header&0xF000)==0){pos+=2+(header&0xFFF);continue;}
                int next=findHeader(raw,pos+2,allowed); if(next<0) break; pos=next; continue;
            }
            int length=u16(raw,pos+2); if(length==0){pos+=4;continue;} int start=pos; pos+=4; int left=length; var spans=new ArrayList<SgaChunk.Span>(); var data=new ByteArrayOutputStream(length); boolean valid=true;
            while(left>0){
                if(pos>=raw.length){valid=false;break;} int available;
                if(pos%SECTOR==0){int continuation=u16(raw,pos);if((continuation&0xF000)!=0){valid=false;break;} available=continuation&0xFFF;pos+=2;if(available==0){pos=Math.min(((pos/SECTOR)+1)*SECTOR,raw.length);continue;}}
                else available=SECTOR-(pos%SECTOR);
                int take=Math.min(left,Math.min(available,raw.length-pos));if(take<=0){valid=false;break;}spans.add(new SgaChunk.Span(pos,take));data.write(raw,pos,take);pos+=take;left-=take;
            }
            if(!valid){pos=start+2;continue;}
            out.add(new SgaChunk(out.size(),start,type,header&255,length,List.copyOf(spans),data.toByteArray()));
            int gap=pos%SECTOR==0?SECTOR:SECTOR-pos%SECTOR;if(gap<12)pos+=gap;
        }
        if(out.isEmpty()) throw new IOException("Aucun chunk SGA valide détecté");
        return new SgaFile(path,raw,List.copyOf(out));
    }
    private static int findHeader(byte[] raw,int pos,Set<Integer> allowed){if((pos&1)!=0)pos++;for(int i=pos;i+3<raw.length;i+=2){int len=u16(raw,i+2);if(allowed.contains(raw[i]&255)&&(raw[i+1]&255)<32&&len>=8)return i;}return -1;}
    private static int u16(byte[] b,int p){return ((b[p]&255)<<8)|(b[p+1]&255);}
}
