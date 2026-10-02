package com.sgasubtitler.app;
import com.sgasubtitler.ui.MainFrame;import javax.swing.*;import java.awt.*;
public final class SgaSubtitlerApp {public static void main(String[]args){System.setProperty("apple.laf.useScreenMenuBar","true");System.setProperty("apple.awt.application.name","SGA Subtitler");SwingUtilities.invokeLater(()->{try{UIManager.setLookAndFeel(UIManager.getSystemLookAndFeelClassName());}catch(Exception ignored){}UIManager.put("Panel.background",new Color(7,18,32));new MainFrame().setVisible(true);});}}
