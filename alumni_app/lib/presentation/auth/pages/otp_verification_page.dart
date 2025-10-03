import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:pinput/pinput.dart';
import '../controllers/auth_controller.dart';
import '../../../shared/widgets/custom_button.dart';
import '../../../shared/widgets/loading_overlay.dart';

class OtpVerificationPage extends StatefulWidget {
  const OtpVerificationPage({Key? key}) : super(key: key);

  @override
  State<OtpVerificationPage> createState() => _OtpVerificationPageState();
}

class _OtpVerificationPageState extends State<OtpVerificationPage> {
  final AuthController authController = Get.find<AuthController>();
  final TextEditingController otpController = TextEditingController();
  final FocusNode focusNode = FocusNode();

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      focusNode.requestFocus();
    });
  }

  @override
  void dispose() {
    otpController.dispose();
    focusNode.dispose();
    super.dispose();
  }

  Future<void> _verifyOtp() async {
    if (otpController.text.length != 6) {
      Get.snackbar(
        'ข้อผิดพลาด',
        'กรุณากรอกรหัส OTP ให้ครบ 6 หลัก',
        snackPosition: SnackPosition.TOP,
        backgroundColor: Colors.red,
        colorText: Colors.white,
      );
      return;
    }

    final isValid = await authController.verifyOtp(otpController.text);
    
    if (isValid) {
      Get.toNamed('/auth/password-setup');
    } else {
      otpController.clear();
      focusNode.requestFocus();
    }
  }

  Future<void> _resendOtp() async {
    final sent = await authController.requestRegistrationOtp();
    
    if (sent) {
      Get.snackbar(
        'สำเร็จ',
        'ส่งรหัส OTP ใหม่แล้ว',
        snackPosition: SnackPosition.TOP,
        backgroundColor: Colors.green,
        colorText: Colors.white,
      );
      otpController.clear();
      focusNode.requestFocus();
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
                  'ยืนยันรหัส OTP',
                  style: TextStyle(
                    fontSize: 28,
                    fontWeight: FontWeight.bold,
                    color: Colors.black87,
                  ),
                ),
                
                const SizedBox(height: 8),
                
                Obx(() => Text(
                  'เราได้ส่งรหัส 6 หลักไปที่\n${authController.phoneNumber.value}',
                  style: const TextStyle(
                    fontSize: 16,
                    color: Colors.grey,
                    height: 1.5,
                  ),
                )),
                
                const SizedBox(height: 50),
                
                // OTP Input
                Center(
                  child: Pinput(
                    controller: otpController,
                    focusNode: focusNode,
                    length: 6,
                    defaultPinTheme: PinTheme(
                      width: 50,
                      height: 60,
                      textStyle: const TextStyle(
                        fontSize: 20,
                        fontWeight: FontWeight.w600,
                        color: Colors.black,
                      ),
                      decoration: BoxDecoration(
                        border: Border.all(color: Colors.grey.shade300),
                        borderRadius: BorderRadius.circular(12),
                      ),
                    ),
                    focusedPinTheme: PinTheme(
                      width: 50,
                      height: 60,
                      textStyle: const TextStyle(
                        fontSize: 20,
                        fontWeight: FontWeight.w600,
                        color: Colors.black,
                      ),
                      decoration: BoxDecoration(
                        border: Border.all(color: Colors.blue, width: 2),
                        borderRadius: BorderRadius.circular(12),
                      ),
                    ),
                    onCompleted: (pin) {
                      _verifyOtp();
                    },
                  ),
                ),
                
                const SizedBox(height: 30),
                
                // Verify button
                SizedBox(
                  width: double.infinity,
                  child: CustomButton(
                    text: 'ยืนยัน',
                    onPressed: _verifyOtp,
                    isLoading: authController.authState.value.isLoading,
                  ),
                ),
                
                const SizedBox(height: 30),
                
                // Resend OTP
                Center(
                  child: GestureDetector(
                    onTap: _resendOtp,
                    child: const Text(
                      'ไม่ได้รับรหัส? ส่งใหม่',
                      style: TextStyle(
                        color: Colors.blue,
                        fontWeight: FontWeight.w600,
                        fontSize: 16,
                      ),
                    ),
                  ),
                ),
                
                const Spacer(),
              ],
            ),
          ),
        ),
      )),
    );
  }
}