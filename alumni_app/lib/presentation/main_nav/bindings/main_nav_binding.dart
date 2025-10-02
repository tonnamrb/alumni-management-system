import 'package:get/get.dart';
import 'package:alumni_app/presentation/main_nav/controllers/main_nav_controller.dart';
import 'package:alumni_app/presentation/feed/controllers/feed_controller.dart';
import 'package:alumni_app/presentation/profile/controllers/profile_controller.dart';

class MainNavBinding extends Bindings {
  @override
  void dependencies() {
    Get.lazyPut<MainNavController>(() => MainNavController(), fenix: true);
    Get.lazyPut<FeedController>(() => FeedController(), fenix: true);
    Get.lazyPut<ProfileController>(() => ProfileController(), fenix: true);
  }
}