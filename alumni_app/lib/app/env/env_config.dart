import 'app_env.dart';

/// Environment specific configuration
abstract class EnvConfig {
  String get apiBaseUrl;
  bool get enableLogging;
  int get apiTimeoutSeconds;
  bool get enableAnalytics;
}

class EnvConfigImpl implements EnvConfig {
  const EnvConfigImpl._();
  
  static const EnvConfigImpl instance = EnvConfigImpl._();
  
  @override
  String get apiBaseUrl {
    switch (AppEnvironment.current) {
      case AppEnvironment.development:
        return 'http://10.0.2.2:5000'; // Android Emulator IP for localhost
      case AppEnvironment.staging:
        return 'https://staging-api.alumni.app';
      case AppEnvironment.production:
        return 'https://api.alumni.app';
    }
  }
  
  @override
  bool get enableLogging => !AppEnvironment.current.isProduction;
  
  @override
  int get apiTimeoutSeconds => 30;
  
  @override
  bool get enableAnalytics => AppEnvironment.current.isProduction;
}