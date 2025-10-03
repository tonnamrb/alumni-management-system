import 'package:dio/dio.dart';
import 'package:get/get.dart';
import '../services/auth_service.dart';

/// Interceptor สำหรับจัดการ JWT token authentication
class AuthInterceptor extends Interceptor {
  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) async {
    // Skip token for auth endpoints that don't need it
    final skipTokenPaths = [
      '/api/v1/auth/check-mobile',
      '/api/v1/auth/request-registration-otp', 
      '/api/v1/auth/verify-registration-otp',
      '/api/v1/auth/complete-registration',
      '/api/v1/auth/login/mobile',
      '/api/v1/auth/status',
    ];

    final shouldSkipToken = skipTokenPaths.any((path) => options.path.contains(path));
    
    if (!shouldSkipToken) {
      try {
        final authService = Get.find<AuthService>();
        final token = await authService.getAccessToken();
        
        if (token != null && token.isNotEmpty) {
          options.headers['Authorization'] = 'Bearer $token';
        }
      } catch (e) {
        // AuthService not available, continue without token
        print('AuthService not available: $e');
      }
    }

    handler.next(options);
  }

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) async {
    // Handle 401 Unauthorized - token expired or invalid
    if (err.response?.statusCode == 401) {
      final requestPath = err.requestOptions.path;
      
      // Don't redirect during login/registration attempts - let the UI handle it
      final isAuthRequest = requestPath.contains('/auth/login') || 
                           requestPath.contains('/auth/register') ||
                           requestPath.contains('/auth/check-mobile') ||
                           requestPath.contains('/auth/request-registration-otp') ||
                           requestPath.contains('/auth/verify-registration-otp') ||
                           requestPath.contains('/auth/complete-registration');
      
      if (!isAuthRequest) {
        try {
          final authService = Get.find<AuthService>();
          await authService.clearSession(); // Clear invalid session
          
          // Redirect to login page (not registration page)
          Get.offAllNamed('/auth/login');
        } catch (e) {
          print('Error handling 401: $e');
        }
      }
    }

    handler.next(err);
  }
}