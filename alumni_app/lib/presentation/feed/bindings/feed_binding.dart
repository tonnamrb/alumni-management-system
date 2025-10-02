import 'package:get/get.dart';
import 'package:alumni_app/presentation/feed/controllers/feed_controller.dart';

class FeedBinding extends Bindings {
  @override
  void dependencies() {
    Get.lazyPut<FeedController>(() => FeedController(), fenix: true);
  }
}