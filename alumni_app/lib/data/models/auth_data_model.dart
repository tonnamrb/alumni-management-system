import 'user_data_model.dart';

/// Login response data model
class LoginDataModel {
  const LoginDataModel({
    required this.accessToken,
    required this.refreshToken,
    required this.expiresAt,
    required this.user,
  });
  
  final String accessToken;
  final String refreshToken;
  final String expiresAt;
  final UserDataModel user;
  
  factory LoginDataModel.fromJson(Map<String, dynamic> json) {
    return LoginDataModel(
      accessToken: json['accessToken'] as String,
      refreshToken: json['refreshToken'] as String,
      expiresAt: json['expiresAt'] as String,
      user: UserDataModel.fromJson(json['user'] as Map<String, dynamic>),
    );
  }
  
  Map<String, dynamic> toJson() => {
    'accessToken': accessToken,
    'refreshToken': refreshToken,
    'expiresAt': expiresAt,
    'user': user.toJson(),
  };
}

/// Password requirements data model
class PasswordRequirementsModel {
  const PasswordRequirementsModel({
    required this.minLength,
    required this.requireUppercase,
    required this.requireLowercase,
    required this.requireNumbers,
    required this.requireSpecialChars,
  });
  
  final int minLength;
  final bool requireUppercase;
  final bool requireLowercase;
  final bool requireNumbers;
  final bool requireSpecialChars;
  
  factory PasswordRequirementsModel.fromJson(Map<String, dynamic> json) {
    return PasswordRequirementsModel(
      minLength: json['minLength'] as int? ?? 8,
      requireUppercase: json['requireUppercase'] as bool? ?? true,
      requireLowercase: json['requireLowercase'] as bool? ?? true,
      requireNumbers: json['requireNumbers'] as bool? ?? true,
      requireSpecialChars: json['requireSpecialCharacters'] as bool? ?? true,
    );
  }
  
  Map<String, dynamic> toJson() => {
    'minLength': minLength,
    'requireUppercase': requireUppercase,
    'requireLowercase': requireLowercase,
    'requireNumbers': requireNumbers,
    'requireSpecialChars': requireSpecialChars,
  };
}

/// Auth status response data model
class AuthStatusDataModel {
  const AuthStatusDataModel({
    required this.authenticationEnabled,
    required this.supportedMethods,
    required this.registrationOpen,
    required this.passwordRequirements,
  });
  
  final bool authenticationEnabled;
  final List<String> supportedMethods;
  final bool registrationOpen;
  final PasswordRequirementsModel passwordRequirements;
  
  factory AuthStatusDataModel.fromJson(Map<String, dynamic> json) {
    return AuthStatusDataModel(
      authenticationEnabled: json['authenticationEnabled'] as bool? ?? false,
      supportedMethods: (json['supportedMethods'] as List<dynamic>?)
          ?.map((e) => e as String)
          .toList() ?? [],
      registrationOpen: json['registrationOpen'] as bool? ?? false,
      passwordRequirements: json['passwordRequirements'] != null 
          ? PasswordRequirementsModel.fromJson(
              json['passwordRequirements'] as Map<String, dynamic>
            )
          : const PasswordRequirementsModel(
              minLength: 8,
              requireUppercase: true,
              requireLowercase: true,
              requireNumbers: true,
              requireSpecialChars: true,
            ),
    );
  }
  
  Map<String, dynamic> toJson() => {
    'authenticationEnabled': authenticationEnabled,
    'supportedMethods': supportedMethods,
    'registrationOpen': registrationOpen,
    'passwordRequirements': passwordRequirements.toJson(),
  };
}