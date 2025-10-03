import 'package:get/get.dart';
import 'package:alumni_app/core/services/theme_service.dart';
import 'package:alumni_app/core/services/locale_service.dart';
import 'package:alumni_app/core/services/user_session_service.dart';
import 'package:alumni_app/core/services/admin_service.dart';
import '../core/storage/shared_prefs_service.dart';

class AppInjection {
  static Future<void> initialize() async {
    // Initialize storage services
    await SharedPrefsService.instance.initialize();
    
    // Core Services - use Get.put with permanent: true for services called immediately in UI
    Get.put<ThemeService>(ThemeService(), permanent: true);
    Get.put<LocaleService>(LocaleService(), permanent: true);
    Get.put<UserSessionService>(UserSessionService(), permanent: true);
    Get.put<AdminService>(AdminService(), permanent: true);
  }
}