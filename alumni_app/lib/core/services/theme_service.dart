import 'package:flutter/material.dart';
import 'package:get/get.dart';

class ThemeService extends GetxService {
  static ThemeService get to => Get.find();
  
  final _isDarkMode = false.obs;
  
  bool get isDarkMode => _isDarkMode.value;
  
  ThemeMode get themeMode => _isDarkMode.value ? ThemeMode.dark : ThemeMode.light;
  
  void switchTheme() {
    _isDarkMode.toggle();
  }
  
  void setDarkMode(bool isDark) {
    _isDarkMode.value = isDark;
  }
}