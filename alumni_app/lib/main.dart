import 'package:flutter/material.dart';
import 'package:flutter/foundation.dart';
import 'package:get/get.dart';
import 'package:flutter_i18n/flutter_i18n.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:alumni_app/app/injection.dart';
import 'package:alumni_app/app/routes/app_routes.dart';
import 'package:alumni_app/core/theme/app_theme.dart';
import 'package:alumni_app/core/services/theme_service.dart';
import 'package:alumni_app/core/services/locale_service.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  
  if (kDebugMode) {
    debugPrint('🚀 Alumni App: Starting application');
  }
  
  // Initialize dependency injection
  await AppInjection.initialize();
  
  runApp(const AlumniApp());
}

class AlumniApp extends StatelessWidget {
  const AlumniApp({super.key});

  @override
  Widget build(BuildContext context) {
    final themeService = Get.find<ThemeService>();
    final localeService = Get.find<LocaleService>();
    
    return GetMaterialApp(
      title: 'Alumni App',
      debugShowCheckedModeBanner: false,
      
      // Theme Configuration
      theme: iosLightTheme,
      darkTheme: iosDarkTheme,
      themeMode: themeService.themeMode,
      
      // Localization Configuration
      locale: localeService.currentLocale,
      supportedLocales: LocaleService.supportedLocales,
      localizationsDelegates: [
        FlutterI18nDelegate(
          translationLoader: FileTranslationLoader(
            useCountryCode: false,
            fallbackFile: 'en',
            basePath: 'assets/i18n',
          ),
        ),
        GlobalMaterialLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
      ],
      
      // Navigation Configuration
      initialRoute: AppRoutes.splash,
      getPages: AppRoutes.routes,
      
      // Global Configuration
      defaultTransition: Transition.cupertino,
      transitionDuration: const Duration(milliseconds: 300),
    );
  }
}