import 'dart:ui';
import 'package:get/get.dart';

class LocaleService extends GetxService {
  static LocaleService get to => Get.find();
  
  final _currentLocale = const Locale('th', 'TH').obs;
  
  Locale get currentLocale => _currentLocale.value;
  
  static const List<Locale> supportedLocales = [
    Locale('th', 'TH'),
    Locale('en', 'US'),
  ];
  
  void changeLocale(Locale locale) {
    if (supportedLocales.contains(locale)) {
      _currentLocale.value = locale;
      Get.updateLocale(locale);
    }
  }
}