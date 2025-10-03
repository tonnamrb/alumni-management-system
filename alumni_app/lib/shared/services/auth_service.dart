import 'dart:convert';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../../domain/entities/auth_models.dart';

/// Authentication service for session management
class AuthService {
  static const String _sessionKey = 'user_session';
  static const _storage = FlutterSecureStorage(
    aOptions: AndroidOptions(
      encryptedSharedPreferences: true,
    ),
  );

  /// Store authentication session
  Future<void> storeSession(AuthResult authResult) async {
    final sessionData = {
      'accessToken': authResult.accessToken,
      'refreshToken': authResult.refreshToken,
      'expiresAt': authResult.expiresAt.toIso8601String(),
      'user': authResult.user.toJson(),
    };
    
    await _storage.write(
      key: _sessionKey,
      value: jsonEncode(sessionData),
    );
  }

  /// Get stored authentication session
  Future<AuthResult?> getStoredSession() async {
    try {
      final sessionJson = await _storage.read(key: _sessionKey);
      if (sessionJson == null) return null;

      final sessionData = jsonDecode(sessionJson) as Map<String, dynamic>;
      
      return AuthResult(
        accessToken: sessionData['accessToken'] as String,
        refreshToken: sessionData['refreshToken'] as String,
        expiresAt: DateTime.parse(sessionData['expiresAt'] as String),
        user: User.fromJson(sessionData['user'] as Map<String, dynamic>),
      );
    } catch (e) {
      // Invalid session data, clear it
      await clearSession();
      return null;
    }
  }

  /// Clear stored session
  Future<void> clearSession() async {
    await _storage.delete(key: _sessionKey);
  }

  /// Check if user is currently authenticated
  Future<bool> isAuthenticated() async {
    final session = await getStoredSession();
    return session != null && !session.isExpired;
  }

  /// Get current access token
  Future<String?> getAccessToken() async {
    final session = await getStoredSession();
    if (session != null && !session.isExpired) {
      return session.accessToken;
    }
    return null;
  }

  /// Get current user
  Future<User?> getCurrentUser() async {
    final session = await getStoredSession();
    return session?.user;
  }
}

/// Extension for AuthResult to add session management methods
extension AuthResultExtension on AuthResult {
  bool get isExpired => DateTime.now().isAfter(expiresAt);
  
  bool get isNearExpiry {
    final bufferTime = Duration(minutes: 5);
    return DateTime.now().add(bufferTime).isAfter(expiresAt);
  }
}