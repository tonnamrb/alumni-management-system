import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../../shared/models/post_model.dart';
import '../services/user_session_service.dart';

/// Service สำหรับ Admin functions
class AdminService extends GetxController {
  final UserSessionService _userSession = Get.find<UserSessionService>();
  
  /// ตรวจสอบว่าผู้ใช้เป็น Admin หรือไม่
  bool get isAdmin {
    // TODO: ควรเช็คจาก AuthController.currentUser.isAdmin
    // ตอนนี้ใช้ mock logic
    return _userSession.currentUserName.toLowerCase().contains('admin');
  }
  
  /// Pin/Unpin โพสต์
  Future<bool> togglePinPost(PostModel post) async {
    if (!isAdmin) return false;
    
    try {
      // Mock API call
      await Future.delayed(const Duration(milliseconds: 500));
      
      post.isPinned = !post.isPinned;
      
      // TODO: เรียก API จริง
      // final result = await _postRepository.togglePin(post.id, post.isPinned);
      
      update(); // อัพเดท UI
      
      Get.snackbar(
        'สำเร็จ',
        post.isPinned ? 'ปักหมุดโพสต์แล้ว' : 'ยกเลิกปักหมุดโพสต์แล้ว',
        snackPosition: SnackPosition.BOTTOM,
      );
      
      return true;
    } catch (e) {
      Get.snackbar(
        'ข้อผิดพลาด',
        'ไม่สามารถปักหมุดโพสต์ได้: $e',
        snackPosition: SnackPosition.BOTTOM,
      );
      return false;
    }
  }
  
  /// ลบโพสต์
  Future<bool> deletePost(PostModel post) async {
    if (!isAdmin) return false;
    
    try {
      // แสดง confirmation dialog
      final confirmed = await _showDeleteConfirmation();
      if (!confirmed) return false;
      
      // Mock API call
      await Future.delayed(const Duration(milliseconds: 500));
      
      // TODO: เรียก API จริง
      // final result = await _postRepository.deletePost(post.id);
      
      Get.snackbar(
        'สำเร็จ',
        'ลบโพสต์แล้ว',
        snackPosition: SnackPosition.BOTTOM,
      );
      
      return true;
    } catch (e) {
      Get.snackbar(
        'ข้อผิดพลาด',
        'ไม่สามารถลบโพสต์ได้: $e',
        snackPosition: SnackPosition.BOTTOM,
      );
      return false;
    }
  }
  
  /// รายงานโพสต์
  Future<bool> reportPost(PostModel post, String reason) async {
    try {
      // Mock API call
      await Future.delayed(const Duration(milliseconds: 500));
      
      post.isReported = true;
      post.reportCount++;
      
      // TODO: เรียก API จริง
      // final result = await _postRepository.reportPost(post.id, reason);
      
      update(); // อัพเดท UI
      
      Get.snackbar(
        'สำเร็จ',
        'รายงานโพสต์แล้ว',
        snackPosition: SnackPosition.BOTTOM,
      );
      
      return true;
    } catch (e) {
      Get.snackbar(
        'ข้อผิดพลาด',
        'ไม่สามารถรายงานโพสต์ได้: $e',
        snackPosition: SnackPosition.BOTTOM,
      );
      return false;
    }
  }
  
  /// แสดง confirmation dialog สำหรับการลบ
  Future<bool> _showDeleteConfirmation() async {
    return await Get.dialog<bool>(
      AlertDialog(
        title: const Text('ยืนยันการลบ'),
        content: const Text('คุณต้องการลบโพสต์นี้จริงหรือไม่?\n\nการดำเนินการนี้ไม่สามารถย้อนกลับได้'),
        actions: [
          TextButton(
            onPressed: () => Get.back(result: false),
            child: const Text('ยกเลิก'),
          ),
          TextButton(
            onPressed: () => Get.back(result: true),
            style: TextButton.styleFrom(
              foregroundColor: Colors.red,
            ),
            child: const Text('ลบ'),
          ),
        ],
      ),
    ) ?? false;
  }
}