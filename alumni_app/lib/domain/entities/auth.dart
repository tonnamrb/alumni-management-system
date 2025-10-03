import 'user.dart';

/// Authentication result entity
class AuthResult {
  const AuthResult({
    required this.accessToken,
    required this.refreshToken,
    required this.expiresAt,
    required this.user,
  });
  
  final String accessToken;
  final String refreshToken;
  final DateTime expiresAt;
  final User user;
  
  @override
  bool operator ==(Object other) =>
    identical(this, other) ||
    (other is AuthResult &&
      other.accessToken == accessToken &&
      other.refreshToken == refreshToken &&
      other.expiresAt == expiresAt &&
      other.user == user);
      
  @override
  int get hashCode => Object.hash(accessToken, refreshToken, expiresAt, user);
  
  @override
  String toString() => 'AuthResult(user: ${user.email}, expiresAt: $expiresAt)';
}

/// Password requirements entity
class PasswordRequirements {
  const PasswordRequirements({
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
  
  @override
  bool operator ==(Object other) =>
    identical(this, other) ||
    (other is PasswordRequirements &&
      other.minLength == minLength &&
      other.requireUppercase == requireUppercase &&
      other.requireLowercase == requireLowercase &&
      other.requireNumbers == requireNumbers &&
      other.requireSpecialChars == requireSpecialChars);
      
  @override
  int get hashCode => Object.hash(
    minLength,
    requireUppercase,
    requireLowercase,
    requireNumbers,
    requireSpecialChars,
  );
}

/// Authentication status entity
class AuthStatus {
  const AuthStatus({
    required this.authenticationEnabled,
    required this.supportedMethods,
    required this.registrationOpen,
    required this.passwordRequirements,
  });
  
  final bool authenticationEnabled;
  final List<String> supportedMethods;
  final bool registrationOpen;
  final PasswordRequirements passwordRequirements;
  
  @override
  bool operator ==(Object other) =>
    identical(this, other) ||
    (other is AuthStatus &&
      other.authenticationEnabled == authenticationEnabled &&
      other.supportedMethods == supportedMethods &&
      other.registrationOpen == registrationOpen &&
      other.passwordRequirements == passwordRequirements);
      
  @override
  int get hashCode => Object.hash(
    authenticationEnabled,
    supportedMethods,
    registrationOpen,
    passwordRequirements,
  );
}