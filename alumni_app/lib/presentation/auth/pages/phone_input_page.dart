import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/auth_controller.dart';
import '../../../shared/widgets/custom_button.dart';
import '../../../shared/widgets/loading_overlay.dart';

class PhoneInputPage extends StatefulWidget {
  const PhoneInputPage({Key? key}) : super(key: key);

  @override
  State<PhoneInputPage> createState() => _PhoneInputPageState();
}

class _PhoneInputPageState extends State<PhoneInputPage> {
  final AuthController authController = Get.find<AuthController>();
  final TextEditingController phoneController = TextEditingController();


  @override
  void dispose() {
    phoneController.dispose();
    super.dispose();
  }

  Future<void> _validateAndProceed() async {
    if (phoneController.text.isEmpty) {
      Get.snackbar(
        'ข้อผิดพลาด',
        'กรุณากรอกหมายเลขโทรศัพท์',
        snackPosition: SnackPosition.TOP,
        backgroundColor: Colors.red,
        colorText: Colors.white,
      );
      return;
    }

    final canRegister = await authController.canRegisterWithPhone(phoneController.text.trim());
    
    if (canRegister) {
      final otpSent = await authController.requestRegistrationOtp();
      if (otpSent) {
        Get.toNamed('/auth/otp-verification');
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: Obx(() => LoadingOverlay(
        isLoading: authController.authState.value.isLoading,
        child: SafeArea(
          child: Padding(
            padding: const EdgeInsets.all(24.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const SizedBox(height: 60),
                
                // Back button
                IconButton(
                  onPressed: () => Get.back(),
                  icon: const Icon(Icons.arrow_back_ios),
                  padding: EdgeInsets.zero,
                ),
                
                const SizedBox(height: 40),
                
                // Title
                const Text(
                  'ลงทะเบียนด้วยเบอร์โทร',
                  style: TextStyle(
                    fontSize: 28,
                    fontWeight: FontWeight.bold,
                    color: Colors.black87,
                  ),
                ),
                
                const SizedBox(height: 8),
                
                const Text(
                  'กรอกหมายเลขโทรศัพท์ของคุณ\nเราจะส่งรหัส OTP เพื่อยืนยัน',
                  style: TextStyle(
                    fontSize: 16,
                    color: Colors.grey,
                    height: 1.5,
                  ),
                ),
                
                const SizedBox(height: 50),
                
                // Phone input field
                TextFormField(
                  controller: phoneController,
                  keyboardType: TextInputType.phone,
                  decoration: InputDecoration(
                    labelText: 'หมายเลขโทรศัพท์',
                    hintText: '0812345678',
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                    focusedBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(12),
                      borderSide: const BorderSide(color: Colors.blue, width: 2),
                    ),
                  ),
                ),
                
                const SizedBox(height: 30),
                
                // Continue button
                SizedBox(
                  width: double.infinity,
                  child: CustomButton(
                    text: 'ดำเนินการต่อ',
                    onPressed: _validateAndProceed,
                    isLoading: authController.authState.value.isLoading,
                  ),
                ),
                
                const Spacer(),
                
                // Login link
                Row(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    const Text(
                      'มีบัญชีแล้ว? ',
                      style: TextStyle(color: Colors.grey),
                    ),
                    GestureDetector(
                      onTap: () => Get.toNamed('/auth/login'),
                      child: const Text(
                        'เข้าสู่ระบบ',
                        style: TextStyle(
                          color: Colors.blue,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ),
                  ],
                ),
                
                const SizedBox(height: 20),
              ],
            ),
          ),
        ),
      )),
    );
  }
}