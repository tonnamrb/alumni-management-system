import 'package:flutter/material.dart';

class AppColor {
  // Primary Colors
  static const Color primary = Color(0xFF007AFF); // iOS Blue
  static const Color secondary = Color(0xFF34C759); // iOS Green
  
  // Background Colors
  static const Color appBase = Color(0xFFF2F2F7); // iOS Background
  static const Color surface = Colors.white;
  static const Color cardBackground = Colors.white;
  
  // Text Colors
  static const Color textPrimary = Color(0xFF000000);
  static const Color textSecondary = Color(0xFF8E8E93);
  static const Color textTertiary = Color(0xFFC7C7CC);
  
  // System Colors
  static const Color error = Color(0xFFFF3B30); // iOS Red
  static const Color warning = Color(0xFFFF9500); // iOS Orange
  static const Color success = Color(0xFF34C759); // iOS Green
  
  // Border & Divider
  static const Color border = Color(0xFFE5E5EA);
  static const Color divider = Color(0xFFE5E5EA);
}

class AppColorDark {
  // Primary Colors (same as light)
  static const Color primary = Color(0xFF007AFF);
  static const Color secondary = Color(0xFF34C759);
  
  // Background Colors
  static const Color appBase = Color(0xFF000000); // iOS Dark Background
  static const Color surface = Color(0xFF1C1C1E); // iOS Dark Surface
  static const Color cardBackground = Color(0xFF1C1C1E);
  
  // Text Colors
  static const Color textPrimary = Color(0xFFFFFFFF);
  static const Color textSecondary = Color(0xFF8E8E93);
  static const Color textTertiary = Color(0xFF48484A);
  
  // System Colors
  static const Color error = Color(0xFFFF453A);
  static const Color warning = Color(0xFFFF9F0A);
  static const Color success = Color(0xFF32D74B);
  
  // Border & Divider
  static const Color border = Color(0xFF38383A);
  static const Color divider = Color(0xFF38383A);
}