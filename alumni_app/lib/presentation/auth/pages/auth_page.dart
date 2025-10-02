import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:flutter_i18n/flutter_i18n.dart';
import 'package:alumni_app/presentation/auth/controllers/auth_controller.dart';

class AuthPage extends GetView<AuthController> {
  const AuthPage({super.key});

  @override
  Widget build(BuildContext context) {
    // Trigger controller initialization
    controller;
    
    return Scaffold(
      body: SafeArea(
        child: SingleChildScrollView(
          physics: const AlwaysScrollableScrollPhysics(),
          child: Container(
            width: double.infinity,
            constraints: BoxConstraints(
              minHeight: MediaQuery.of(context).size.height - MediaQuery.of(context).padding.top,
            ),
            padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 32),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                // Logo and Welcome
                Container(
                  width: 80,
                  height: 80,
                  decoration: BoxDecoration(
                    color: Theme.of(context).primaryColor,
                    borderRadius: BorderRadius.circular(20),
                  ),
                  child: const Icon(
                    Icons.school,
                    size: 40,
                    color: Colors.white,
                  ),
                ),
                
                const SizedBox(height: 24),
                
                Text(
                  FlutterI18n.translate(context, "auth.welcome"),
                  style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                  textAlign: TextAlign.center,
                ),
                
                const SizedBox(height: 40),
                
                // Auth Form
                Container(
                  constraints: const BoxConstraints(maxWidth: 400),
                  child: Form(
                    key: controller.formKey,
                    child: Column(
                      children: [
                        // Email Field
                        TextFormField(
                          controller: controller.emailController,
                          keyboardType: TextInputType.emailAddress,
                          validator: controller.validateEmail,
                          decoration: InputDecoration(
                            labelText: FlutterI18n.translate(context, "auth.email"),
                            prefixIcon: const Icon(Icons.email_outlined),
                          ),
                        ),
                        
                        const SizedBox(height: 16),
                        
                        // Password Field
                        TextFormField(
                          controller: controller.passwordController,
                          obscureText: true,
                          validator: controller.validatePassword,
                          decoration: InputDecoration(
                            labelText: FlutterI18n.translate(context, "auth.password"),
                            prefixIcon: const Icon(Icons.lock_outlined),
                          ),
                        ),
                        
                        const SizedBox(height: 24),
                        
                        // Submit Button
                        Obx(() => ElevatedButton(
                          onPressed: controller.isLoading.value ? null : controller.authenticate,
                          child: controller.isLoading.value
                            ? const SizedBox(
                                width: 20,
                                height: 20,
                                child: CircularProgressIndicator(
                                  strokeWidth: 2,
                                  valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
                                ),
                              )
                            : Obx(() => Text(
                                FlutterI18n.translate(
                                  context, 
                                  controller.isLoginMode.value ? "auth.login_button" : "auth.register_button"
                                ),
                              )),
                        )),
                        
                        const SizedBox(height: 16),
                        
                        // Toggle Mode
                        TextButton(
                          onPressed: controller.toggleMode,
                          child: Obx(() => Text(
                            controller.isLoginMode.value 
                              ? FlutterI18n.translate(context, "auth.register")
                              : FlutterI18n.translate(context, "auth.login"),
                          )),
                        ),
                      ],
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}