package com.sgasubtitler.model;
public record Subtitle(int number, long startMillis, long endMillis, String text) {
    public boolean activeAt(long millis) { return millis >= startMillis && millis < endMillis; }
}
