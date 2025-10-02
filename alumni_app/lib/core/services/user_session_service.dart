import 'package:get/get.dart';

/// Service สำหรับจัดการข้อมูล User ที่ login อยู่ปัจจุบัน
class UserSessionService extends GetxService {
  // ข้อมูล user ปัจจุบัน (mock data)
  final RxString _currentUserName = "John Doe".obs;
  final RxString _currentUserEmail = "john.doe@alumni.com".obs;
  final RxString _currentUserAvatar = "".obs;
  
  // Getters
  String get currentUserName => _currentUserName.value;
  String get currentUserEmail => _currentUserEmail.value;
  String get currentUserAvatar => _currentUserAvatar.value;
  
  // Method สำหรับอัปเดตข้อมูล user (เช่น หลังจาก login)
  void updateUserInfo({
    required String name,
    required String email,
    String? avatar,
  }) {
    _currentUserName.value = name;
    _currentUserEmail.value = email;
    if (avatar != null) {
      _currentUserAvatar.value = avatar;
    }
  }
  
  // Method สำหรับ clear session (logout)
  void clearSession() {
    _currentUserName.value = "";
    _currentUserEmail.value = "";
    _currentUserAvatar.value = "";
  }
  
  // Check ว่า user login อยู่หรือไม่
  bool get isLoggedIn => _currentUserName.value.isNotEmpty;
}