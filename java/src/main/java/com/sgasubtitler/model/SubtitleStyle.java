package com.sgasubtitler.model;
import java.awt.*;
public record SubtitleStyle(String family, int size, boolean bold, Position position, int verticalOffset, int outline) {
    public enum Position { HAUT, MILIEU, BAS }
    public Font font() { return new Font(family, bold ? Font.BOLD : Font.PLAIN, size); }
    public static SubtitleStyle defaults() { return new SubtitleStyle(Font.SANS_SERIF,14,true,Position.BAS,0,2); }
}
