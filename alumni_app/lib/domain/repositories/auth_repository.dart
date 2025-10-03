import 'package:result_dart/result_dart.dart';
import '../entities/auth_models.dart';
import '../entities/app_error.dart';

/// Mobile phone authentication repository interface
abstract class AuthRepository {
  // ตรวจสอบว่าเบอร์นี้สมัครได้หรือไม่
  Future<Result<bool, AppError>> canRegisterWithPhone(String phone);
  
  // ส่ง OTP สำหรับลงทะเบียน
  Future<Result<void, AppError>> requestRegistrationOtp(String phone);
  
  // ตรวจสอบ OTP
  Future<Result<bool, AppError>> verifyOtp(String phone, String otp);
  
  // เสร็จสิ้นการลงทะเบียน
  Future<Result<AuthResult, AppError>> completeRegistration({
    required String phone,
    required String otpCode,
    required String password,
    String? firstName,
    String? lastName,
  });
  
  // เข้าสู่ระบบด้วยเบอร์โทร
  Future<Result<AuthResult, AppError>> loginWithPhone({
    required String phone,
    required String password,
  });
  
  // ตรวจสอบสถานะการเข้าสู่ระบบ
  Future<Result<User, AppError>> getCurrentUser();
  
  // Refresh token
  Future<Result<AuthResult, AppError>> refreshToken(String refreshToken);
  
  // ออกจากระบบ
  Future<Result<void, AppError>> logout();
}