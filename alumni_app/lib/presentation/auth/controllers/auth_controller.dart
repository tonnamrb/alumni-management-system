import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:alumni_app/app/routes/app_routes.dart';
import 'package:alumni_app/core/services/user_session_service.dart';

class AuthController extends GetxController {
  final formKey = GlobalKey<FormState>();
  final emailController = TextEditingController();
  final passwordController = TextEditingController();
  
  final RxBool isLoading = false.obs;
  final RxBool isLoginMode = true.obs;
  
  // ✅ เพิ่ม UserSessionService
  final UserSessionService _userSessionService = Get.find<UserSessionService>();
  
  @override
  void onClose() {
    emailController.dispose();
    passwordController.dispose();
    super.onClose();
  }
  
  void toggleMode() {
    isLoginMode.value = !isLoginMode.value;
    // Clear form when switching modes
    emailController.clear();
    passwordController.clear();
  }
  
  Future<void> authenticate() async {
    if (!formKey.currentState!.validate()) {
      return;
    }
    
    isLoading.value = true;
    
    if (kDebugMode) {
      debugPrint('🔐 AuthController: ${isLoginMode.value ? 'Login' : 'Register'} attempt');
      debugPrint('🔐 Email: ${emailController.text}');
    }
    
    // Simulate authentication delay
    await Future.delayed(const Duration(seconds: 2));
    
    // Mock authentication - always succeed
    if (kDebugMode) {
      debugPrint('🔐 AuthController: Authentication successful, navigating to main');
    }
    
    // ✅ อัพเดตข้อมูล user ใน session หลังจาก login สำเร็จ
    _userSessionService.updateUserInfo(
      name: "John Doe", // In real app, ใช้ข้อมูลจาก API response
      email: emailController.text,
    );
    
    isLoading.value = false;
    
    // Navigate to main screen
    Get.offAllNamed(AppRoutes.main);
  }
  
  String? validateEmail(String? value) {
    if (value == null || value.isEmpty) {
      return 'Email is required';
    }
    
    final emailRegex = RegExp(r'^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$');
    if (!emailRegex.hasMatch(value)) {
      return 'Enter a valid email address';
    }
    
    return null;
  }
  
  String? validatePassword(String? value) {
    if (value == null || value.isEmpty) {
      return 'Password is required';
    }
    
    if (value.length < 6) {
      return 'Password must be at least 6 characters';
    }
    
    return null;
  }
}