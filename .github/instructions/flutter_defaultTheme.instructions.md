---
applyTo: '**/*.dart, **/*.flutter'
---
# 🍏 iOS-like Theme (Light & Dark)

> ใช้ไฟล์นี้เฉพาะเมื่อ `theme.instructions.md` ไม่มีไฟล์ใน repo ถ้ามีไฟล์ `theme.instructions.md` ให้ใช้ไฟล์นั้นแทน หรือถ้ามีไฟล์ `theme.instructions.md` อยู่แต่ไม่มีข้อมูลเกี่ยวกับธีม ให้รวมข้อมูลจากไฟล์นี้เข้าไปใน `theme.instructions.md` แทน หรือถ้าไฟล์ `theme.instructions.md` มีข้อมูลเกี่ยวกับธีมอยู่แล้ว แต่ไม่ครบ ให้เพิ่มข้อมูลจากไฟล์นี้เข้าไปใน `theme.instructions.md` แทน

## Light Theme

```dart
// lib/core/theme/app_theme.dart
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
    background: AppColor.appBase,
    error: AppColor.error,
    onSurface: AppColor.textPrimary,     // ✅ สีตัวอักษรหลักบนพื้น
    onBackground: AppColor.textPrimary,
    onPrimary: Colors.white,
    onSecondary: Colors.white,
    outline: AppColor.border,
  ),

  scaffoldBackgroundColor: AppColor.appBase,
  cardColor: AppColor.surface,
  dividerColor: AppColor.border,

  // ✅ ผูกสไตล์ที่ TextField ใช้จริง ๆ
  textTheme: const TextTheme(
    titleMedium: AppTextTheme.input, // EditableText ใช้ตัวนี้
    bodyLarge:  AppTextTheme.input,  // เผื่อบางเวอร์ชัน/วิดเจ็ต
    bodyMedium: AppTextTheme.body1,
    labelLarge: AppTextTheme.button,
    headlineSmall: AppTextTheme.headline2,
  ),

  textSelectionTheme: TextSelectionThemeData(
    cursorColor: AppColor.primary,
    selectionColor: AppColor.primary.withOpacity(0.18),
    selectionHandleColor: AppColor.primary,
  ),

  inputDecorationTheme: InputDecorationTheme(
    filled: true,
    fillColor: AppColor.surface,
    contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
    border: OutlineInputBorder(
      borderRadius: BorderRadius.circular(16),
      borderSide: BorderSide(color: AppColor.border),
    ),
    enabledBorder: OutlineInputBorder(
      borderRadius: BorderRadius.circular(16),
      borderSide: BorderSide(color: AppColor.border),
    ),
    focusedBorder: OutlineInputBorder(
      borderRadius: BorderRadius.circular(16),
      borderSide: BorderSide(color: AppColor.primary, width: 2),
    ),
    hintStyle: const TextStyle(color: AppColor.textSecondary), // hint จางกว่า
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

  tabBarTheme: const TabBarTheme(indicatorSize: TabBarIndicatorSize.label),

  snackBarTheme: SnackBarThemeData(
    backgroundColor: AppColor.surface,
    contentTextStyle: AppTextTheme.body1,
    actionTextColor: AppColor.primary,
    behavior: SnackBarBehavior.floating,
    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
  ),

  dialogTheme: DialogTheme(
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
    background: AppColorDark.appBase,
    error: AppColorDark.error,
    onSurface: AppColorDark.textPrimary,   // ✅ ตัวอักษรใน Dark
    onBackground: AppColorDark.textPrimary,
    onPrimary: Colors.white,
    onSecondary: Colors.white,
    outline: AppColorDark.border,
  ),

  scaffoldBackgroundColor: AppColorDark.appBase,
  cardColor: AppColorDark.surface,
  dividerColor: AppColorDark.border,

  textTheme: const TextTheme(
    titleMedium: AppTextThemeDark.input,
    bodyLarge:  AppTextThemeDark.input,
    bodyMedium: AppTextThemeDark.body1,
    labelLarge: AppTextThemeDark.button,
    headlineSmall: AppTextThemeDark.headline2,
  ),

  textSelectionTheme: TextSelectionThemeData(
    cursorColor: AppColorDark.primary,
    selectionColor: AppColorDark.primary.withOpacity(0.22),
    selectionHandleColor: AppColorDark.primary,
  ),

  inputDecorationTheme: InputDecorationTheme(
    filled: true,
    fillColor: AppColorDark.surface,
    contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
    border: OutlineInputBorder(
      borderRadius: BorderRadius.circular(16),
      borderSide: BorderSide(color: AppColorDark.border),
    ),
    enabledBorder: OutlineInputBorder(
      borderRadius: BorderRadius.circular(16),
      borderSide: BorderSide(color: AppColorDark.border),
    ),
    focusedBorder: OutlineInputBorder(
      borderRadius: BorderRadius.circular(16),
      borderSide: BorderSide(color: AppColorDark.primary, width: 2),
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

  tabBarTheme: const TabBarTheme(indicatorSize: TabBarIndicatorSize.label),

  snackBarTheme: SnackBarThemeData(
    backgroundColor: AppColorDark.surface,
    contentTextStyle: AppTextThemeDark.body1,
    actionTextColor: AppColorDark.primary,
    behavior: SnackBarBehavior.floating,
    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
  ),

  dialogTheme: DialogTheme(
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



**Usage:**

```dart
MaterialApp(
  theme: iosLightTheme,
  darkTheme: iosDarkTheme,
  // ...
)
```

---

- อ้างอิงจาก iOS 17 Human Interface Guidelines
- ใช้ font 'SF Pro Display' (หรือ Montserrat/Roboto แทนถ้าไม่มี)
- ปรับสี/spacing/shape ให้เหมาะกับ iOS look & feel