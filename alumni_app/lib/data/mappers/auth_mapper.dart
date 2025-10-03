import '../../domain/entities/auth.dart';
import '../models/auth_data_model.dart';
import 'user_mapper.dart';

/// Mapper to convert between auth data models and entities
class AuthMapper {
  /// Convert LoginDataModel to AuthResult entity
  static AuthResult toAuthResult(LoginDataModel model) {
    return AuthResult(
      accessToken: model.accessToken,
      refreshToken: model.refreshToken,
      expiresAt: DateTime.parse(model.expiresAt),
      user: UserMapper.toEntity(model.user),
    );
  }
  
  /// Convert PasswordRequirementsModel to PasswordRequirements entity
  static PasswordRequirements toPasswordRequirements(PasswordRequirementsModel model) {
    return PasswordRequirements(
      minLength: model.minLength,
      requireUppercase: model.requireUppercase,
      requireLowercase: model.requireLowercase,
      requireNumbers: model.requireNumbers,
      requireSpecialChars: model.requireSpecialChars,
    );
  }
  
  /// Convert AuthStatusDataModel to AuthStatus entity
  static AuthStatus toAuthStatus(AuthStatusDataModel model) {
    return AuthStatus(
      authenticationEnabled: model.authenticationEnabled,
      supportedMethods: model.supportedMethods,
      registrationOpen: model.registrationOpen,
      passwordRequirements: toPasswordRequirements(model.passwordRequirements),
    );
  }
}