import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:flutter_i18n/flutter_i18n.dart';
import 'package:alumni_app/presentation/main_nav/controllers/main_nav_controller.dart';
import 'package:alumni_app/presentation/feed/pages/feed_page.dart';
import 'package:alumni_app/presentation/profile/pages/profile_page.dart';

class MainNavPage extends GetView<MainNavController> {
  const MainNavPage({super.key});

  @override
  Widget build(BuildContext context) {
    // Trigger controller initialization
    controller;
    
    return Scaffold(
      body: Obx(() => IndexedStack(
        index: controller.currentIndex.value,
        children: const [
          FeedPage(),
          ProfilePage(),
        ],
      )),
      bottomNavigationBar: Obx(() => BottomNavigationBar(
        currentIndex: controller.currentIndex.value,
        onTap: controller.onTap,
        items: [
          BottomNavigationBarItem(
            icon: const Icon(Icons.home),
            label: FlutterI18n.translate(context, "bottom_nav.feed"),
          ),
          BottomNavigationBarItem(
            icon: const Icon(Icons.person),
            label: FlutterI18n.translate(context, "bottom_nav.profile"),
          ),
        ],
      )),
    );
  }
}