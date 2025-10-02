import 'package:get/get.dart';
import 'package:alumni_app/presentation/profile/controllers/user_profile_controller.dart';

class UserProfileBinding extends Bindings {
  @override
  void dependencies() {
    Get.lazyPut<UserProfileController>(
      () => UserProfileController(),
      fenix: true,
    );
  }
}