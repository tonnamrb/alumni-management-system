import 'package:get/get.dart';
import 'package:alumni_app/presentation/splash/pages/splash_page.dart';
import 'package:alumni_app/presentation/splash/bindings/splash_binding.dart';
import 'package:alumni_app/presentation/auth/pages/login_page.dart';
import 'package:alumni_app/presentation/auth/pages/phone_input_page.dart';
import 'package:alumni_app/presentation/auth/pages/otp_verification_page.dart';
import 'package:alumni_app/presentation/auth/pages/password_setup_page.dart';
import 'package:alumni_app/presentation/auth/bindings/auth_binding.dart';
import 'package:alumni_app/presentation/main_nav/pages/main_nav_page.dart';
import 'package:alumni_app/presentation/main_nav/bindings/main_nav_binding.dart';
import 'package:alumni_app/presentation/profile/pages/user_profile_page.dart';
import 'package:alumni_app/presentation/profile/bindings/user_profile_binding.dart';
import 'package:alumni_app/presentation/test_posts_page.dart';

class AppRoutes {
  static const String splash = '/splash';
  static const String login = '/auth/login';
  static const String phoneInput = '/auth/phone-input';
  static const String otpVerification = '/auth/otp-verification';
  static const String passwordSetup = '/auth/password-setup';
  static const String main = '/main';
  static const String home = '/home';
  static const String userProfile = '/user-profile';
  static const String postsTest = '/test/posts';
  
  static List<GetPage> routes = [
    GetPage(
      name: splash,
      page: () => const SplashPage(),
      binding: SplashBinding(),
    ),
    GetPage(
      name: login,
      page: () => const LoginPage(),
      binding: AuthBinding(),
    ),
    GetPage(
      name: phoneInput,
      page: () => const PhoneInputPage(),
      binding: AuthBinding(),
    ),
    GetPage(
      name: otpVerification,
      page: () => const OtpVerificationPage(),
      binding: AuthBinding(),
    ),
    GetPage(
      name: passwordSetup,
      page: () => const PasswordSetupPage(),
      binding: AuthBinding(),
    ),
    GetPage(
      name: main,
      page: () => const MainNavPage(),
      binding: MainNavBinding(),
    ),
    GetPage(
      name: home,
      page: () => const MainNavPage(),
      binding: MainNavBinding(),
    ),
    GetPage(
      name: userProfile,
      page: () => const UserProfilePage(),
      binding: UserProfileBinding(),
    ),
    GetPage(
      name: postsTest,
      page: () => const PostsTestPage(),
    ),
  ];
}