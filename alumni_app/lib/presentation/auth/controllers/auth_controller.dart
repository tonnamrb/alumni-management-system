import 'package:get/get.dart';
import '../../../domain/entities/auth_models.dart';
import '../../../domain/entities/app_error.dart';
import '../../../domain/repositories/auth_repository.dart';
import '../../../shared/services/auth_service.dart';
import '../../../core/services/user_session_service.dart';

class AuthController extends GetxController {
  final AuthRepository _authRepository;
  late final AuthService _authService;
  late final UserSessionService _userSessionService;

  AuthController(this._authRepository);
  
  // State variables
  final Rx<AuthState> authState = AuthState.initial().obs;
  final Rx<User?> currentUser = Rx<User?>(null);

  // Phone registration flow state
  final RxString phoneNumber = ''.obs;
  final RxString otpCode = ''.obs;
  final RxString password = ''.obs;
  final RxBool isOtpVerified = false.obs;

  // Getters
  bool get isAuthenticated => currentUser.value != null;
  bool get isAdmin => currentUser.value?.isAdmin ?? false;
  bool get isMember => currentUser.value?.isMember ?? true;
  UserRole get userRole => currentUser.value != null 
      ? UserRole.fromId(currentUser.value!.roleId)
      : UserRole.member;

  @override
  void onInit() {
    super.onInit();
    _authService = Get.find<AuthService>();
    _userSessionService = Get.find<UserSessionService>();
    // เอา checkAuthStatus() ออก เพราะทำให้เกิด infinite loop
    // ให้ SplashController จัดการ auth check แทน
  }

  Future<void> checkAuthStatus() async {
    authState.value = authState.value.copyWith(isLoading: true);
    
    final result = await _authRepository.getCurrentUser();
    
    result.fold(
      (user) {
        currentUser.value = user;
        authState.value = AuthState.authenticated();
      },
      (error) {
        currentUser.value = null;
        authState.value = AuthState.unauthenticated();
      },
    );
  }

  // Phone Registration Flow
  Future<bool> canRegisterWithPhone(String phone) async {
    phoneNumber.value = phone;
    authState.value = authState.value.copyWith(isLoading: true);
    
    final result = await _authRepository.canRegisterWithPhone(phone);
    
    return result.fold(
      (canRegister) {
        authState.value = authState.value.copyWith(isLoading: false);
        if (!canRegister) {
          authState.value = authState.value.copyWith(
            error: AppError.validation('เบอร์โทรศัพท์นี้ไม่สามารถลงทะเบียนได้ หรือได้ลงทะเบียนแล้ว'),
          );
        }
        return canRegister;
      },
      (error) {
        authState.value = authState.value.copyWith(
          isLoading: false,
          error: error,
        );
        return false;
      },
    );
  }

  Future<bool> requestRegistrationOtp() async {
    if (phoneNumber.value.isEmpty) {
      authState.value = authState.value.copyWith(
        error: AppError.validation('กรุณากรอกหมายเลขโทรศัพท์'),
      );
      return false;
    }

    authState.value = authState.value.copyWith(isLoading: true);
    
    final result = await _authRepository.requestRegistrationOtp(phoneNumber.value);
    
    return result.fold(
      (_) {
        authState.value = authState.value.copyWith(isLoading: false);
        return true;
      },
      (error) {
        authState.value = authState.value.copyWith(
          isLoading: false,
          error: error,
        );
        return false;
      },
    );
  }

  Future<bool> verifyOtp(String otp) async {
    otpCode.value = otp;
    authState.value = authState.value.copyWith(isLoading: true);
    
    final result = await _authRepository.verifyOtp(phoneNumber.value, otp);
    
    return result.fold(
      (isValid) {
        isOtpVerified.value = isValid;
        authState.value = authState.value.copyWith(isLoading: false);
        if (!isValid) {
          authState.value = authState.value.copyWith(
            error: AppError.validation('รหัส OTP ไม่ถูกต้อง'),
          );
        }
        return isValid;
      },
      (error) {
        authState.value = authState.value.copyWith(
          isLoading: false,
          error: error,
        );
        return false;
      },
    );
  }

  Future<bool> completeRegistration(String password, {String? firstName, String? lastName}) async {
    if (!isOtpVerified.value) {
      authState.value = authState.value.copyWith(
        error: AppError.validation('กรุณายืนยัน OTP ก่อน'),
      );
      return false;
    }

    this.password.value = password;
    authState.value = authState.value.copyWith(isLoading: true);
    
    final result = await _authRepository.completeRegistration(
      phone: phoneNumber.value,
      otpCode: otpCode.value,
      password: password,
      firstName: firstName,
      lastName: lastName,
    );
    
    return result.fold(
      (authResult) async {
        // Store session in secure storage
        await _authService.storeSession(authResult);
        
        // Update user session service
        _userSessionService.updateUserInfo(
          name: authResult.user.fullName,
          email: authResult.user.email,
          avatar: authResult.user.alumniProfile?.profilePictureUrl,
        );
        
        currentUser.value = authResult.user;
        authState.value = AuthState.authenticated();
        _clearRegistrationState();
        return true;
      },
      (error) {
        authState.value = authState.value.copyWith(
          isLoading: false,
          error: error,
        );
        return false;
      },
    );
  }

  // Login
  Future<bool> loginWithPhone({
    required String phone,
    required String password,
  }) async {
    authState.value = authState.value.copyWith(isLoading: true);
    
    final result = await _authRepository.loginWithPhone(
      phone: phone,
      password: password,
    );
    
    return result.fold(
      (authResult) async {
        // Store session in secure storage
        await _authService.storeSession(authResult);
        
        // Update user session service
        _userSessionService.updateUserInfo(
          name: authResult.user.fullName,
          email: authResult.user.email,
          avatar: authResult.user.alumniProfile?.profilePictureUrl,
        );
        
        currentUser.value = authResult.user;
        authState.value = AuthState.authenticated();
        return true;
      },
      (error) {
        // ตรวจสอบว่าเป็นกรณีที่ต้องลงทะเบียนหรือไม่
        if (error.message.contains('ต้องลงทะเบียน') || error.message.contains('ยังไม่ได้ตั้งรหัสผ่าน')) {
          // กรณีที่เบอร์มีในระบบแต่ยังไม่ได้ลงทะเบียน
          phoneNumber.value = phone;
          authState.value = authState.value.copyWith(
            isLoading: false,
            error: AppError.validation('กรุณาลงทะเบียนด้วยเบอร์โทรศัพท์นี้ก่อน'),
          );
        } else {
          authState.value = authState.value.copyWith(
            isLoading: false,
            error: error,
          );
        }
        return false;
      },
    );
  }

  /// ตรวจสอบว่าเบอร์นี้ต้องลงทะเบียนหรือไม่ (ใช้หลัง login fail)
  Future<bool> shouldRegisterPhone(String phone) async {
    final canRegister = await canRegisterWithPhone(phone);
    if (canRegister) {
      phoneNumber.value = phone;
    }
    return canRegister;
  }

  // Logout
  Future<void> logout() async {
    authState.value = authState.value.copyWith(isLoading: true);
    
    await _authRepository.logout();
    await _authService.clearSession();
    
    // Clear user session service
    _userSessionService.clearSession();
    
    currentUser.value = null;
    authState.value = AuthState.unauthenticated();
    _clearRegistrationState();
  }

  void clearError() {
    authState.value = authState.value.copyWith(error: null);
  }

  void _clearRegistrationState() {
    phoneNumber.value = '';
    otpCode.value = '';
    password.value = '';
    isOtpVerified.value = false;
  }
}

class AuthState {
  final bool isLoading;
  final bool isAuthenticated;
  final AppError? error;

  const AuthState({
    this.isLoading = false,
    this.isAuthenticated = false,
    this.error,
  });

  factory AuthState.initial() => const AuthState();
  
  factory AuthState.loading() => const AuthState(isLoading: true);
  
  factory AuthState.authenticated() => const AuthState(isAuthenticated: true);
  
  factory AuthState.unauthenticated() => const AuthState(isAuthenticated: false);

  AuthState copyWith({
    bool? isLoading,
    bool? isAuthenticated,
    AppError? error,
  }) {
    return AuthState(
      isLoading: isLoading ?? this.isLoading,
      isAuthenticated: isAuthenticated ?? this.isAuthenticated,
      error: error ?? this.error,
    );
  }
}