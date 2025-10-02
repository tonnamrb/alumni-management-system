import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:flutter_i18n/flutter_i18n.dart';
import 'package:alumni_app/presentation/splash/controllers/splash_controller.dart';

class SplashPage extends GetView<SplashController> {
  const SplashPage({super.key});

  @override
  Widget build(BuildContext context) {
    // Trigger controller initialization
    controller;
    
    return Scaffold(
      backgroundColor: Theme.of(context).primaryColor,
      body: SafeArea(
        child: Container(
          width: double.infinity,
          padding: const EdgeInsets.symmetric(horizontal: 32),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              // App Logo/Icon
              Container(
                width: 120,
                height: 120,
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(24),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withValues(alpha: 0.1),
                      blurRadius: 20,
                      offset: const Offset(0, 4),
                    ),
                  ],
                ),
                child: const Icon(
                  Icons.school,
                  size: 64,
                  color: Colors.blue,
                ),
              ),
              
              const SizedBox(height: 32),
              
              // App Name
              Text(
                FlutterI18n.translate(context, "app.name"),
                style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                  color: Colors.white,
                  fontWeight: FontWeight.bold,
                ),
              ),
              
              const SizedBox(height: 64),
              
              // Progress Bar
              Container(
                width: double.infinity,
                height: 4,
                decoration: BoxDecoration(
                  color: Colors.white.withValues(alpha: 0.2),
                  borderRadius: BorderRadius.circular(2),
                ),
                child: Obx(() => FractionallySizedBox(
                  alignment: Alignment.centerLeft,
                  widthFactor: controller.progress.value,
                  child: Container(
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(2),
                    ),
                  ),
                )),
              ),
              
              const SizedBox(height: 16),
              
              // Loading Text
              Text(
                FlutterI18n.translate(context, "splash.loading"),
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  color: Colors.white.withValues(alpha: 0.8),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}