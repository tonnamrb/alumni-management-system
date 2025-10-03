import 'auth_user.dart';

/// Authentication session entity
class AuthSession {
  final String accessToken;
  final String refreshToken;
  final DateTime expiresAt;
  final AuthUser user;

  const AuthSession({
    required this.accessToken,
    required this.refreshToken,
    required this.expiresAt,
    required this.user,
  });

  bool get isExpired => DateTime.now().isAfter(expiresAt);
  
  bool get isNearExpiry {
    final bufferTime = Duration(minutes: 5);
    return DateTime.now().add(bufferTime).isAfter(expiresAt);
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is AuthSession &&
          runtimeType == other.runtimeType &&
          accessToken == other.accessToken &&
          user == other.user;

  @override
  int get hashCode => accessToken.hashCode ^ user.hashCode;
}