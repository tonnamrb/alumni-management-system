import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../../shared/models/post_model.dart';
import '../../core/services/admin_service.dart';
import 'admin_menu_bottom_sheet.dart';

/// ปุ่ม admin menu สำหรับแสดงใน post card
class AdminMenuButton extends StatelessWidget {
  final PostModel post;
  final VoidCallback? onPostUpdated;

  const AdminMenuButton({
    Key? key,
    required this.post,
    this.onPostUpdated,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return GetBuilder<AdminService>(
      builder: (adminService) {
        // ถ้าไม่ใช่ admin ไม่แสดงปุ่ม
        if (!adminService.isAdmin) {
          return const SizedBox.shrink();
        }

        return PopupMenuButton<String>(
          icon: Icon(
            Icons.admin_panel_settings,
            color: Theme.of(context).primaryColor,
            size: 20,
          ),
          tooltip: 'Admin Menu',
          onSelected: (value) => _handleMenuAction(context, value),
          itemBuilder: (BuildContext context) => [
            PopupMenuItem<String>(
              value: 'pin',
              child: Row(
                children: [
                  Icon(
                    post.isPinned ? Icons.push_pin : Icons.push_pin_outlined,
                    size: 18,
                    color: Theme.of(context).primaryColor,
                  ),
                  const SizedBox(width: 12),
                  Text(post.isPinned ? 'ยกเลิกปักหมุด' : 'ปักหมุด'),
                ],
              ),
            ),
            const PopupMenuItem<String>(
              value: 'delete',
              child: Row(
                children: [
                  Icon(
                    Icons.delete_outline,
                    size: 18,
                    color: Colors.red,
                  ),
                  SizedBox(width: 12),
                  Text(
                    'ลบโพสต์',
                    style: TextStyle(color: Colors.red),
                  ),
                ],
              ),
            ),
            PopupMenuItem<String>(
              value: 'report',
              child: Row(
                children: [
                  Icon(
                    Icons.flag_outlined,
                    size: 18,
                    color: post.isReported ? Colors.orange : Colors.grey[600],
                  ),
                  const SizedBox(width: 12),
                  Text(
                    post.isReported 
                        ? 'ดูรายงาน (${post.reportCount})'
                        : 'รายงาน',
                    style: TextStyle(
                      color: post.isReported ? Colors.orange : Colors.grey[600],
                    ),
                  ),
                ],
              ),
            ),
            const PopupMenuItem<String>(
              value: 'menu',
              child: Row(
                children: [
                  Icon(
                    Icons.more_horiz,
                    size: 18,
                    color: Colors.grey,
                  ),
                  SizedBox(width: 12),
                  Text('เพิ่มเติม'),
                ],
              ),
            ),
          ],
        );
      },
    );
  }

  void _handleMenuAction(BuildContext context, String action) async {
    final adminService = Get.find<AdminService>();

    switch (action) {
      case 'pin':
        final success = await adminService.togglePinPost(post);
        if (success) onPostUpdated?.call();
        break;
        
      case 'delete':
        final success = await adminService.deletePost(post);
        if (success) onPostUpdated?.call();
        break;
        
      case 'report':
        // แสดง report dialog หรือ report details
        if (post.isReported) {
          _showReportDetails(context);
        } else {
          _showReportDialog(context);
        }
        break;
        
      case 'menu':
        // แสดง full admin menu
        AdminMenuBottomSheet.show(
          context,
          post,
          onPostUpdated: onPostUpdated,
        );
        break;
    }
  }

  void _showReportDetails(BuildContext context) {
    Get.dialog(
      AlertDialog(
        title: const Text('รายละเอียดการรายงาน'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('โพสต์นี้ถูกรายงาน ${post.reportCount} ครั้ง'),
            const SizedBox(height: 16),
            const Text('สถานะ: กำลังตรวจสอบ'),
            // TODO: แสดงรายละเอียดการรายงานเพิ่มเติม
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Get.back(),
            child: const Text('ปิด'),
          ),
          ElevatedButton(
            onPressed: () {
              Get.back();
              // TODO: ดำเนินการกับรายงาน
            },
            child: const Text('ดำเนินการ'),
          ),
        ],
      ),
    );
  }

  void _showReportDialog(BuildContext context) {
    final reasons = [
      'เนื้อหาไม่เหมาะสม',
      'สแปมหรือโฆษณา', 
      'การล่วงละเมิดหรือคุกคาม',
      'ข้อมูลเท็จ',
      'อื่นๆ',
    ];

    String selectedReason = reasons.first;

    Get.dialog(
      StatefulBuilder(
        builder: (context, setState) => AlertDialog(
          title: const Text('รายงานโพสต์'),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Text('เลือกเหตุผลในการรายงาน:'),
              const SizedBox(height: 16),
              ...reasons.map((reason) => 
                RadioListTile<String>(
                  title: Text(reason),
                  value: reason,
                  groupValue: selectedReason,
                  onChanged: (value) {
                    setState(() {
                      selectedReason = value!;
                    });
                  },
                ),
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: () => Get.back(),
              child: const Text('ยกเลิก'),
            ),
            ElevatedButton(
              onPressed: () async {
                Get.back();
                final adminService = Get.find<AdminService>();
                final success = await adminService.reportPost(post, selectedReason);
                if (success) onPostUpdated?.call();
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.red,
                foregroundColor: Colors.white,
              ),
              child: const Text('รายงาน'),
            ),
          ],
        ),
      ),
    );
  }
}