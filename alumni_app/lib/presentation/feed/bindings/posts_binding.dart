import 'package:get/get.dart';
import 'package:dio/dio.dart';
import '../../../data/datasources/remote/posts_api.dart';
import '../../../data/repositories/posts_repository_impl.dart';
import '../../../domain/repositories/posts_repository.dart';
import '../../../app/env/env_config.dart';
import '../../../shared/services/auth_service.dart';
import '../../../shared/interceptors/auth_interceptor.dart';

class PostsBinding extends Bindings {
  @override
  void dependencies() {
    // Register AuthService if not already registered
    if (!Get.isRegistered<AuthService>()) {
      Get.lazyPut<AuthService>(() => AuthService(), fenix: true);
    }
    
    // Register Dio instance if not already registered
    if (!Get.isRegistered<Dio>()) {
      final config = EnvConfigImpl.instance;
      final dio = Dio(BaseOptions(
        baseUrl: config.apiBaseUrl, // Get from environment config
        connectTimeout: Duration(seconds: config.apiTimeoutSeconds),
        receiveTimeout: Duration(seconds: config.apiTimeoutSeconds),
      ));
      
      // Add auth interceptor
      dio.interceptors.add(AuthInterceptor());
      
      Get.put(dio);
    }

    // Register PostsApi
    Get.lazyPut<PostsApi>(() => PostsApi(Get.find<Dio>()));
    
    // Register PostsRepository
    Get.lazyPut<PostsRepository>(() => PostsRepositoryImpl(postsApi: Get.find<PostsApi>()));
  }
}