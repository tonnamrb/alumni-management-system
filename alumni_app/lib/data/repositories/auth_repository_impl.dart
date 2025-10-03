import 'package:dio/dio.dart';
import 'package:result_dart/result_dart.dart';
import '../../domain/repositories/auth_repository.dart';
import '../../domain/entities/auth_models.dart';
import '../../domain/entities/app_error.dart';
import '../../app/env/env_config.dart';

class AuthRepositoryImpl implements AuthRepository {
  final Dio _dio;
  late final String _baseUrl;

  AuthRepositoryImpl(this._dio) {
    _baseUrl = EnvConfigImpl.instance.apiBaseUrl;
  }

  @override
  Future<Result<bool, AppError>> canRegisterWithPhone(String phone) async {
    try {
      final normalizedPhone = _normalizePhoneNumber(phone);
      // ทดลองขอ OTP เพื่อดูว่าเบอร์นี้สมัครได้หรือไม่
      final response = await _dio.post('$_baseUrl/api/v1/auth/request-registration-otp', data: {
        'mobilePhone': normalizedPhone,
      });
      
      if (response.statusCode == 200) {
        // หาก success = true แสดงว่าเบอร์นี้สมัครได้
        final responseData = response.data as Map<String, dynamic>;
        return Success(responseData['success'] == true);
      } else {
        return Failure(AppError.server('ไม่สามารถตรวจสอบเบอร์โทรได้', statusCode: response.statusCode));
      }
    } on DioException catch (e) {
      // หาก error จาก API แสดงว่าเบอร์นี้สมัครไม่ได้
      if (e.response?.statusCode == 400) {
        return Success(false); // ส่งกลับ false แทนการ throw error
      }
      return Failure(_handleDioError(e));
    } catch (e) {
      return Failure(AppError.unknown('เกิดข้อผิดพลาดที่ไม่คาดคิด: $e'));
    }
  }

  @override
  Future<Result<void, AppError>> requestRegistrationOtp(String phone) async {
    try {
      final normalizedPhone = _normalizePhoneNumber(phone);
      final response = await _dio.post('$_baseUrl/api/v1/auth/request-registration-otp', data: {
        'mobilePhone': normalizedPhone,
      });
      
      if (response.statusCode == 200) {
        final responseData = response.data as Map<String, dynamic>;
        if (responseData['success'] == true) {
          return Success(());
        } else {
          final error = responseData['error'] as String? ?? 'ไม่สามารถส่ง OTP ได้';
          return Failure(AppError.server(error));
        }
      } else {
        return Failure(AppError.server('ไม่สามารถส่ง OTP ได้', statusCode: response.statusCode));
      }
    } on DioException catch (e) {
      return Failure(_handleDioError(e));
    } catch (e) {
      return Failure(AppError.unknown('เกิดข้อผิดพลาดที่ไม่คาดคิด: $e'));
    }
  }

  @override
  Future<Result<bool, AppError>> verifyOtp(String phone, String otp) async {
    try {
      final normalizedPhone = _normalizePhoneNumber(phone);
      final response = await _dio.post('$_baseUrl/api/v1/auth/verify-registration-otp', data: {
        'mobilePhone': normalizedPhone,
        'otpCode': otp,
      });
      
      if (response.statusCode == 200) {
        final responseData = response.data as Map<String, dynamic>;
        return Success(responseData['success'] == true);
      } else {
        return Failure(AppError.server('ไม่สามารถตรวจสอบ OTP ได้', statusCode: response.statusCode));
      }
    } on DioException catch (e) {
      return Failure(_handleDioError(e));
    } catch (e) {
      return Failure(AppError.unknown('เกิดข้อผิดพลาดที่ไม่คาดคิด: $e'));
    }
  }

  @override
  Future<Result<AuthResult, AppError>> completeRegistration({
    required String phone,
    required String otpCode,
    required String password,
    String? firstName,
    String? lastName,
  }) async {
    try {
      final normalizedPhone = _normalizePhoneNumber(phone);
      final requestData = {
        'mobilePhone': normalizedPhone,
        'otpCode': otpCode,
        'password': password,
      };
      
      // เพิ่ม firstName และ lastName ถ้ามี
      if (firstName != null && firstName.isNotEmpty) {
        requestData['firstName'] = firstName;
      }
      if (lastName != null && lastName.isNotEmpty) {
        requestData['lastName'] = lastName;
      }
      
      final response = await _dio.post('$_baseUrl/api/v1/auth/complete-registration', data: requestData);
      
      if (response.statusCode == 200) {
        final responseData = response.data as Map<String, dynamic>;
        if (responseData['success'] == true) {
          final authResult = AuthResult.fromJson(responseData['data']);
          return Success(authResult);
        } else {
          final error = responseData['error'] as String? ?? 'ไม่สามารถสร้างบัญชีได้';
          return Failure(AppError.server(error));
        }
      } else {
        return Failure(AppError.server('ไม่สามารถสร้างบัญชีได้', statusCode: response.statusCode));
      }
    } on DioException catch (e) {
      return Failure(_handleDioError(e));
    } catch (e) {
      return Failure(AppError.unknown('เกิดข้อผิดพลาดที่ไม่คาดคิด: $e'));
    }
  }

  @override
  Future<Result<AuthResult, AppError>> loginWithPhone({
    required String phone,
    required String password,
  }) async {
    try {
      final normalizedPhone = _normalizePhoneNumber(phone);
      final response = await _dio.post('$_baseUrl/api/v1/auth/login/mobile', data: {
        'mobilePhone': normalizedPhone,
        'password': password,
      });
      
      if (response.statusCode == 200) {
        final responseData = response.data as Map<String, dynamic>;
        if (responseData['success'] == true) {
          final authResult = AuthResult.fromJson(responseData['data']);
          return Success(authResult);
        } else {
          final error = responseData['error'] as String? ?? 'หมายเลขโทรศัพท์หรือรหัสผ่านไม่ถูกต้อง';
          return Failure(AppError.authentication(error));
        }
      } else {
        return Failure(AppError.authentication(
          'หมายเลขโทรศัพท์หรือรหัสผ่านไม่ถูกต้อง',
          statusCode: response.statusCode,
        ));
      }
    } on DioException catch (e) {
      return Failure(_handleDioError(e));
    } catch (e) {
      return Failure(AppError.unknown('เกิดข้อผิดพลาดที่ไม่คาดคิด: $e'));
    }
  }

  @override
  Future<Result<User, AppError>> getCurrentUser() async {
    try {
      final response = await _dio.get('$_baseUrl/api/v1/auth/profile');
      
      if (response.statusCode == 200) {
        final user = User.fromJson(response.data);
        return Success(user);
      } else {
        return Failure(AppError.authentication(
          'ไม่สามารถดึงข้อมูลผู้ใช้ได้',
          statusCode: response.statusCode,
        ));
      }
    } on DioException catch (e) {
      return Failure(_handleDioError(e));
    } catch (e) {
      return Failure(AppError.unknown('เกิดข้อผิดพลาดที่ไม่คาดคิด: $e'));
    }
  }

  @override
  Future<Result<AuthResult, AppError>> refreshToken(String refreshToken) async {
    // TODO: Implement refresh token endpoint in backend
    return Failure(AppError.authentication(
      'Refresh token not implemented yet in backend',
    ));
  }

  @override
  Future<Result<void, AppError>> logout() async {
    try {
      await _dio.post('$_baseUrl/api/v1/auth/logout');
      return Success(());
    } catch (e) {
      return Success(()); // Even if API call fails, consider logout successful
    }
  }

  // Helper methods
  String _normalizePhoneNumber(String phone) {
    // Remove all non-digit characters
    String digits = phone.replaceAll(RegExp(r'[^\d]'), '');
    
    // Handle Thai phone numbers
    if (digits.startsWith('66')) {
      return digits; // Already has country code
    } else if (digits.startsWith('0') && digits.length == 10) {
      return '66${digits.substring(1)}'; // Replace leading 0 with 66
    } else if (digits.length == 9) {
      return '66$digits'; // Add country code
    }
    
    return digits; // Return as is for other formats
  }

  AppError _handleDioError(DioException e) {
    switch (e.type) {
      case DioExceptionType.connectionTimeout:
      case DioExceptionType.sendTimeout:
      case DioExceptionType.receiveTimeout:
        return AppError.network('การเชื่อมต่อหมดเวลา กรุณาลองใหม่อีกครั้ง');
      case DioExceptionType.connectionError:
        return AppError.network('ไม่สามารถเชื่อมต่อเซิร์ฟเวอร์ได้');
      case DioExceptionType.badResponse:
        final statusCode = e.response?.statusCode;
        String message = 'เกิดข้อผิดพลาดจากเซิร์ฟเวอร์';
        
        try {
          final responseData = e.response?.data;
          if (responseData is Map<String, dynamic>) {
            message = responseData['message'] ?? message;
          } else if (responseData is String) {
            message = responseData;
          }
        } catch (_) {
          // Use default message if parsing fails
        }
        
        if (statusCode == 401) {
          return AppError.authentication(message, statusCode: statusCode);
        } else if (statusCode == 403) {
          return AppError.authorization(message);
        } else if (statusCode != null && statusCode >= 400 && statusCode < 500) {
          return AppError.validation(message);
        } else {
          return AppError.server(message, statusCode: statusCode);
        }
      case DioExceptionType.cancel:
        return AppError.unknown('การร้องขอถูกยกเลิก');
      case DioExceptionType.unknown:
        return AppError.network('ไม่สามารถเชื่อมต่อได้ กรุณาตรวจสอบการเชื่อมต่ออินเทอร์เน็ต');
      default:
        return AppError.unknown('เกิดข้อผิดพลาดที่ไม่คาดคิด');
    }
  }
}