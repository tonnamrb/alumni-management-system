import 'package:flutter/foundation.dart';
import 'package:get/get.dart';
import 'package:alumni_app/app/routes/app_routes.dart';
import 'package:alumni_app/core/services/user_session_service.dart';

class SplashController extends GetxController {
  final UserSessionService _sessionService = Get.find<UserSessionService>();
  
  final RxBool isCheckingAuth = true.obs;
  
  @override
  void onReady() {
    super.onReady();
    _checkAuthenticationStatus();
  }
  
  Future<void> _checkAuthenticationStatus() async {
    if (kDebugMode) {
      debugPrint('🔐 SplashController: Checking authentication status...');
    }
    
    // แสดง splash ไว้เล็กน้อยเพื่อ UX ที่ดี
    await Future.delayed(const Duration(milliseconds: 1500));
    
    try {
      // เช็คว่า user login อยู่หรือไม่
      final isLoggedIn = _sessionService.isLoggedIn;
      
      if (isLoggedIn) {
        if (kDebugMode) {
          debugPrint('✅ SplashController: User is logged in, navigating to home');
        }
        
        // ถ้า login แล้วให้ไปหน้า home
        _navigateToHome();
      } else {
        if (kDebugMode) {
          debugPrint('❌ SplashController: User not logged in, navigating to auth');
        }
        
        // ถ้ายังไม่ login ให้ไปหน้า auth
        _navigateToAuth();
      }
    } catch (e) {
      if (kDebugMode) {
        debugPrint('⚠️ SplashController: Error checking auth status: $e');
      }
      
      // กรณีเกิด error ให้ไปหน้า auth
      _navigateToAuth();
    } finally {
      isCheckingAuth.value = false;
    }
  }
  
  void _navigateToHome() {
    if (kDebugMode) {
      debugPrint('🏠 SplashController: Navigating to home');
    }
    
    // Navigate to home page
    Get.offAllNamed(AppRoutes.home);
  }
  
  void _navigateToAuth() {
    if (kDebugMode) {
      debugPrint('🔐 SplashController: Navigating to auth');
    }
    
    // Navigate to login page
    Get.offAllNamed(AppRoutes.login);
  }
}