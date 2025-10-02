import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:shared_preferences/shared_preferences.dart';

class LocaleService extends GetxService {
  static const String _localeKey = 'locale';
  
  late SharedPreferences _prefs;
  final Rx<Locale> _currentLocale = const Locale('en', 'US').obs;
  
  Locale get currentLocale => _currentLocale.value;
  
  // Supported locales
  static const List<Locale> supportedLocales = [
    Locale('en', 'US'),
    Locale('th', 'TH'),
  ];
  
  @override
  Future<void> onInit() async {
    super.onInit();
    _prefs = await SharedPreferences.getInstance();
    _loadLocale();
  }
  
  void _loadLocale() {
    final localeCode = _prefs.getString(_localeKey) ?? 'en';
    _currentLocale.value = Locale(localeCode);
  }
  
  Future<void> changeLocale(String localeCode) async {
    final locale = Locale(localeCode);
    _currentLocale.value = locale;
    await _prefs.setString(_localeKey, localeCode);
    
    // Update GetX locale
    Get.updateLocale(locale);
  }
  
  String getLanguageName(String localeCode) {
    switch (localeCode) {
      case 'th':
        return 'ไทย';
      case 'en':
      default:
        return 'English';
    }
  }
}