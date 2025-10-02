import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:shared_preferences/shared_preferences.dart';

class ThemeService extends GetxService {
  static const String _themeKey = 'theme_mode';
  
  late SharedPreferences _prefs;
  final RxBool _isDarkMode = false.obs;
  
  bool get isDarkMode => _isDarkMode.value;
  ThemeMode get themeMode => _isDarkMode.value ? ThemeMode.dark : ThemeMode.light;
  
  @override
  Future<void> onInit() async {
    super.onInit();
    _prefs = await SharedPreferences.getInstance();
    _loadTheme();
  }
  
  void _loadTheme() {
    final isDark = _prefs.getBool(_themeKey) ?? false;
    _isDarkMode.value = isDark;
  }
  
  Future<void> toggleTheme() async {
    _isDarkMode.value = !_isDarkMode.value;
    await _prefs.setBool(_themeKey, _isDarkMode.value);
    Get.changeThemeMode(themeMode);
  }
  
  Future<void> setTheme(bool isDark) async {
    _isDarkMode.value = isDark;
    await _prefs.setBool(_themeKey, isDark);
    Get.changeThemeMode(themeMode);
  }
}