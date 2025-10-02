import 'package:flutter/foundation.dart';
import 'package:get/get.dart';
import 'package:alumni_app/app/routes/app_routes.dart';

class SplashController extends GetxController {
  final RxDouble progress = 0.0.obs;
  
  @override
  void onReady() {
    super.onReady();
    _startSplashTimer();
  }
  
  void _startSplashTimer() {
    if (kDebugMode) {
      debugPrint('🚀 SplashController: Starting splash timer');
    }
    
    // Simulate loading with progress bar for 5 seconds
    const totalDuration = 5000; // 5 seconds in milliseconds
    const updateInterval = 50; // Update every 50ms
    const increment = updateInterval / totalDuration; // Progress increment per update
    
    // Start the progress animation
    Future.doWhile(() async {
      await Future.delayed(const Duration(milliseconds: updateInterval));
      progress.value += increment;
      
      if (progress.value >= 1.0) {
        progress.value = 1.0;
        _navigateToAuth();
        return false; // Stop the loop
      }
      return true; // Continue the loop
    });
  }
  
  void _navigateToAuth() {
    if (kDebugMode) {
      debugPrint('🚀 SplashController: Navigating to auth');
    }
    
    // Navigate to auth page after splash completes
    Get.offAllNamed(AppRoutes.auth);
  }
}