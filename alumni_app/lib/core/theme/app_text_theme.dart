import 'package:flutter/material.dart';

class AppTextTheme {
  static const String fontFamily = 'SF Pro Display';
  
  // Headlines
  static const TextStyle headline1 = TextStyle(
    fontSize: 32,
    fontWeight: FontWeight.bold,
    letterSpacing: -0.5,
  );
  
  static const TextStyle headline2 = TextStyle(
    fontSize: 24,
    fontWeight: FontWeight.bold,
    letterSpacing: -0.5,
  );
  
  static const TextStyle headline3 = TextStyle(
    fontSize: 20,
    fontWeight: FontWeight.w600,
    letterSpacing: -0.3,
  );
  
  // Body Text
  static const TextStyle body1 = TextStyle(
    fontSize: 16,
    fontWeight: FontWeight.normal,
    letterSpacing: -0.2,
  );
  
  static const TextStyle body2 = TextStyle(
    fontSize: 14,
    fontWeight: FontWeight.normal,
    letterSpacing: -0.1,
  );
  
  // Button
  static const TextStyle button = TextStyle(
    fontSize: 16,
    fontWeight: FontWeight.w600,
    letterSpacing: -0.2,
  );
  
  // Caption
  static const TextStyle caption = TextStyle(
    fontSize: 12,
    fontWeight: FontWeight.normal,
    letterSpacing: 0,
  );
  
  // Input
  static const TextStyle input = TextStyle(
    fontSize: 16,
    fontWeight: FontWeight.normal,
    letterSpacing: -0.2,
  );
}

class AppTextThemeDark {
  static const String fontFamily = 'SF Pro Display';
  
  // Same styles as light theme but will use dark colors from theme
  static const TextStyle headline1 = AppTextTheme.headline1;
  static const TextStyle headline2 = AppTextTheme.headline2;
  static const TextStyle headline3 = AppTextTheme.headline3;
  static const TextStyle body1 = AppTextTheme.body1;
  static const TextStyle body2 = AppTextTheme.body2;
  static const TextStyle button = AppTextTheme.button;
  static const TextStyle caption = AppTextTheme.caption;
  static const TextStyle input = AppTextTheme.input;
}