/// Login request model
class LoginRequestModel {
  const LoginRequestModel({
    required this.email,
    required this.password,
  });
  
  final String email;
  final String password;
  
  Map<String, dynamic> toJson() => {
    'email': email,
    'password': password,
  };
}

/// Mobile login request model
class MobileLoginRequestModel {
  const MobileLoginRequestModel({
    required this.mobilePhone,
    required this.password,
  });
  
  final String mobilePhone;
  final String password;
  
  Map<String, dynamic> toJson() => {
    'mobilePhone': mobilePhone,
    'password': password,
  };
}

/// Register request model
class RegisterRequestModel {
  const RegisterRequestModel({
    required this.name,
    required this.email,
    required this.password,
  });
  
  final String name;
  final String email;
  final String password;
  
  Map<String, dynamic> toJson() => {
    'name': name,
    'email': email,
    'password': password,
  };
}

/// Mobile register request model
class MobileRegisterRequestModel {
  const MobileRegisterRequestModel({
    required this.mobilePhone,
    required this.name,
    required this.password,
    this.email,
  });
  
  final String mobilePhone;
  final String name;
  final String password;
  final String? email;
  
  Map<String, dynamic> toJson() => {
    'mobilePhone': mobilePhone,
    'name': name,
    'password': password,
    if (email != null) 'email': email,
  };
}

/// Change password request model
class ChangePasswordRequestModel {
  const ChangePasswordRequestModel({
    required this.currentPassword,
    required this.newPassword,
    required this.confirmPassword,
  });
  
  final String currentPassword;
  final String newPassword;
  final String confirmPassword;
  
  Map<String, dynamic> toJson() => {
    'currentPassword': currentPassword,
    'newPassword': newPassword,
    'confirmPassword': confirmPassword,
  };
}

/// OTP request model
class RequestOtpModel {
  const RequestOtpModel({
    required this.mobilePhone,
  });
  
  final String mobilePhone;
  
  Map<String, dynamic> toJson() => {
    'mobilePhone': mobilePhone,
  };
}

/// OTP verification request model
class VerifyOtpRequestModel {
  const VerifyOtpRequestModel({
    required this.mobilePhone,
    required this.otpCode,
  });
  
  final String mobilePhone;
  final String otpCode;
  
  Map<String, dynamic> toJson() => {
    'mobilePhone': mobilePhone,
    'otpCode': otpCode,
  };
}