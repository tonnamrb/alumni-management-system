import 'package:flutter/material.dart';
import 'app_color.dart';
import 'app_text_theme.dart';

final ThemeData iosLightTheme = ThemeData(
  useMaterial3: true,
  brightness: Brightness.light,
  fontFamily: AppTextTheme.fontFamily,

  colorScheme: ColorScheme.light(
    primary: AppColor.primary,
    secondary: AppColor.secondary,
    surface: AppColor.surface,
    error: AppColor.error,
    onSurface: AppColor.textPrimary,
    onPrimary: Colors.white,
    onSecondary: Colors.white,
    outline: AppColor.border,
  ),

  scaffoldBackgroundColor: AppColor.appBase,
  cardColor: AppColor.surface,
  dividerColor: AppColor.border,

  textTheme: const TextTheme(
    titleMedium: AppTextTheme.input,
    bodyLarge: AppTextTheme.input,
    bodyMedium: AppTextTheme.body1,
    labelLarge: AppTextTheme.button,
    headlineSmall: AppTextTheme.headline2,
  ),

  textSelectionTheme: TextSelectionThemeData(
    cursorColor: AppColor.primary,
    selectionColor: AppColor.primary.withValues(alpha: 0.18),
    selectionHandleColor: AppColor.primary,
  ),

  inputDecorationTheme: InputDecorationTheme(
    filled: true,
    fillColor: AppColor.surface,
    contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
    border: OutlineInputBorder(
      borderRadius: BorderRadius.circular(16),
      borderSide: const BorderSide(color: AppColor.border),
    ),
    enabledBorder: OutlineInputBorder(
      borderRadius: BorderRadius.circular(16),
      borderSide: const BorderSide(color: AppColor.border),
    ),
    focusedBorder: OutlineInputBorder(
      borderRadius: BorderRadius.circular(16),
      borderSide: const BorderSide(color: AppColor.primary, width: 2),
    ),
    hintStyle: const TextStyle(color: AppColor.textSecondary),
    labelStyle: const TextStyle(color: AppColor.textSecondary),
    floatingLabelStyle: const TextStyle(fontWeight: FontWeight.w600, color: AppColor.primary),
    prefixIconColor: AppColor.textSecondary,
    suffixIconColor: AppColor.textSecondary,
    prefixStyle: const TextStyle(color: AppColor.textPrimary),
    suffixStyle: const TextStyle(color: AppColor.textSecondary),
    errorStyle: const TextStyle(fontSize: 13),
  ),

  elevatedButtonTheme: ElevatedButtonThemeData(
    style: ElevatedButton.styleFrom(
      backgroundColor: AppColor.primary,
      foregroundColor: Colors.white,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      minimumSize: const Size.fromHeight(48),
      textStyle: AppTextTheme.button,
    ),
  ),
  
  outlinedButtonTheme: OutlinedButtonThemeData(
    style: OutlinedButton.styleFrom(
      foregroundColor: AppColor.primary,
      side: const BorderSide(color: AppColor.primary, width: 2),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      textStyle: AppTextTheme.button,
    ),
  ),

  bottomNavigationBarTheme: const BottomNavigationBarThemeData(
    backgroundColor: AppColor.surface,
    selectedItemColor: AppColor.primary,
    unselectedItemColor: AppColor.textSecondary,
    type: BottomNavigationBarType.fixed,
  ),

  appBarTheme: const AppBarTheme(
    backgroundColor: AppColor.surface,
    foregroundColor: AppColor.textPrimary,
    elevation: 0,
    scrolledUnderElevation: 1,
  ),

  snackBarTheme: SnackBarThemeData(
    backgroundColor: AppColor.surface,
    contentTextStyle: AppTextTheme.body1,
    actionTextColor: AppColor.primary,
    behavior: SnackBarBehavior.floating,
    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
  ),

  dialogTheme: DialogThemeData(
    backgroundColor: AppColor.surface,
    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
    titleTextStyle: AppTextTheme.headline2,
    contentTextStyle: AppTextTheme.body1,
  ),

  bottomSheetTheme: const BottomSheetThemeData(
    backgroundColor: AppColor.surface,
    shape: RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(24))),
  ),
);

final ThemeData iosDarkTheme = ThemeData(
  useMaterial3: true,
  brightness: Brightness.dark,
  fontFamily: AppTextThemeDark.fontFamily,

  colorScheme: ColorScheme.dark(
    primary: AppColorDark.primary,
    secondary: AppColorDark.secondary,
    surface: AppColorDark.surface,
    error: AppColorDark.error,
    onSurface: AppColorDark.textPrimary,
    onPrimary: Colors.white,
    onSecondary: Colors.white,
    outline: AppColorDark.border,
  ),

  scaffoldBackgroundColor: AppColorDark.appBase,
  cardColor: AppColorDark.surface,
  dividerColor: AppColorDark.border,

  textTheme: const TextTheme(
    titleMedium: AppTextThemeDark.input,
    bodyLarge: AppTextThemeDark.input,
    bodyMedium: AppTextThemeDark.body1,
    labelLarge: AppTextThemeDark.button,
    headlineSmall: AppTextThemeDark.headline2,
  ),

  textSelectionTheme: TextSelectionThemeData(
    cursorColor: AppColorDark.primary,
    selectionColor: AppColorDark.primary.withValues(alpha: 0.22),
    selectionHandleColor: AppColorDark.primary,
  ),

  inputDecorationTheme: InputDecorationTheme(
    filled: true,
    fillColor: AppColorDark.surface,
    contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
    border: OutlineInputBorder(
      borderRadius: BorderRadius.circular(16),
      borderSide: const BorderSide(color: AppColorDark.border),
    ),
    enabledBorder: OutlineInputBorder(
      borderRadius: BorderRadius.circular(16),
      borderSide: const BorderSide(color: AppColorDark.border),
    ),
    focusedBorder: OutlineInputBorder(
      borderRadius: BorderRadius.circular(16),
      borderSide: const BorderSide(color: AppColorDark.primary, width: 2),
    ),
    hintStyle: const TextStyle(color: AppColorDark.textSecondary),
    labelStyle: const TextStyle(color: AppColorDark.textSecondary),
    floatingLabelStyle: const TextStyle(fontWeight: FontWeight.w600, color: AppColorDark.primary),
    prefixIconColor: AppColorDark.textSecondary,
    suffixIconColor: AppColorDark.textSecondary,
    prefixStyle: const TextStyle(color: AppColorDark.textPrimary),
    suffixStyle: const TextStyle(color: AppColorDark.textSecondary),
    errorStyle: const TextStyle(fontSize: 13),
  ),

  elevatedButtonTheme: ElevatedButtonThemeData(
    style: ElevatedButton.styleFrom(
      backgroundColor: AppColorDark.primary,
      foregroundColor: Colors.white,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      minimumSize: const Size.fromHeight(48),
      textStyle: AppTextThemeDark.button,
    ),
  ),
  
  outlinedButtonTheme: OutlinedButtonThemeData(
    style: OutlinedButton.styleFrom(
      foregroundColor: AppColorDark.primary,
      side: const BorderSide(color: AppColorDark.primary, width: 2),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      textStyle: AppTextThemeDark.button,
    ),
  ),

  bottomNavigationBarTheme: const BottomNavigationBarThemeData(
    backgroundColor: AppColorDark.surface,
    selectedItemColor: AppColorDark.primary,
    unselectedItemColor: AppColorDark.textSecondary,
    type: BottomNavigationBarType.fixed,
  ),

  appBarTheme: const AppBarTheme(
    backgroundColor: AppColorDark.surface,
    foregroundColor: AppColorDark.textPrimary,
    elevation: 0,
    scrolledUnderElevation: 1,
  ),

  snackBarTheme: SnackBarThemeData(
    backgroundColor: AppColorDark.surface,
    contentTextStyle: AppTextThemeDark.body1,
    actionTextColor: AppColorDark.primary,
    behavior: SnackBarBehavior.floating,
    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
  ),

  dialogTheme: DialogThemeData(
    backgroundColor: AppColorDark.surface,
    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
    titleTextStyle: AppTextThemeDark.headline2,
    contentTextStyle: AppTextThemeDark.body1,
  ),

  bottomSheetTheme: const BottomSheetThemeData(
    backgroundColor: AppColorDark.surface,
    shape: RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(24))),
  ),
);