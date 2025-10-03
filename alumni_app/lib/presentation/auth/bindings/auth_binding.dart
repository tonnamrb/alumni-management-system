import 'package:get/get.dart';
import 'package:dio/dio.dart';
import '../controllers/auth_controller.dart';
import '../../../data/repositories/auth_repository_impl.dart';
import '../../../domain/repositories/auth_repository.dart';
import '../../../app/env/env_config.dart';
import '../../../shared/services/auth_service.dart';
import '../../../shared/interceptors/auth_interceptor.dart';

class AuthBinding extends Bindings {
  @override
  void dependencies() {
    // Register AuthService first
    Get.lazyPut<AuthService>(() => AuthService(), fenix: true);
    
    // HTTP Client - use shared Dio instance or create one with env config
    if (!Get.isRegistered<Dio>()) {
      final config = EnvConfigImpl.instance;
      final dio = Dio(BaseOptions(
        baseUrl: config.apiBaseUrl,
        connectTimeout: Duration(seconds: config.apiTimeoutSeconds),
        receiveTimeout: Duration(seconds: config.apiTimeoutSeconds),
        headers: {
          'Content-Type': 'application/json',
        },
      ));
      
      // Add auth interceptor
      dio.interceptors.add(AuthInterceptor());
      
      Get.put(dio, permanent: true);
    }
    
    // Repository
    Get.lazyPut<AuthRepository>(
      () => AuthRepositoryImpl(Get.find<Dio>()),
      fenix: true,
    );
    
    // Controller
    Get.lazyPut<AuthController>(
      () => AuthController(Get.find<AuthRepository>()),
      fenix: true,
    );
  }
}