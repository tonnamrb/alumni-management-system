/// Application Environment Configuration
enum AppEnvironment {
  development('development'),
  staging('staging'),
  production('production');

  const AppEnvironment(this.value);
  
  final String value;
  
  static AppEnvironment get current {
    const environment = String.fromEnvironment('ENVIRONMENT', defaultValue: 'development');
    return AppEnvironment.values.firstWhere(
      (env) => env.value == environment,
      orElse: () => AppEnvironment.development,
    );
  }
  
  bool get isDevelopment => this == AppEnvironment.development;
  bool get isStaging => this == AppEnvironment.staging;
  bool get isProduction => this == AppEnvironment.production;
}