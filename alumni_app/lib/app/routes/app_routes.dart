import 'package:get/get.dart';
import 'package:alumni_app/presentation/splash/pages/splash_page.dart';
import 'package:alumni_app/presentation/splash/bindings/splash_binding.dart';
import 'package:alumni_app/presentation/auth/pages/auth_page.dart';
import 'package:alumni_app/presentation/auth/bindings/auth_binding.dart';
import 'package:alumni_app/presentation/main_nav/pages/main_nav_page.dart';
import 'package:alumni_app/presentation/main_nav/bindings/main_nav_binding.dart';
import 'package:alumni_app/presentation/profile/pages/user_profile_page.dart';
import 'package:alumni_app/presentation/profile/bindings/user_profile_binding.dart';

class AppRoutes {
  static const String splash = '/splash';
  static const String auth = '/auth';
  static const String main = '/main';
  static const String userProfile = '/user-profile';
  
  static List<GetPage> routes = [
    GetPage(
      name: splash,
      page: () => const SplashPage(),
      binding: SplashBinding(),
    ),
    GetPage(
      name: auth,
      page: () => const AuthPage(),
      binding: AuthBinding(),
    ),
    GetPage(
      name: main,
      page: () => const MainNavPage(),
      binding: MainNavBinding(),
    ),
    GetPage(
      name: userProfile,
      page: () => const UserProfilePage(),
      binding: UserProfileBinding(),
    ),
  ];
}