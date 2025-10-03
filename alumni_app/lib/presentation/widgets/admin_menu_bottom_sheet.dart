import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../../shared/models/post_model.dart';
import '../../core/services/admin_service.dart';

/// Bottom sheet แสดง admin menu options
class AdminMenuBottomSheet extends StatelessWidget {
  final PostModel post;
  final VoidCallback? onPostUpdated;

  const AdminMenuBottomSheet({
    Key? key,
    required this.post,
    this.onPostUpdated,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final adminService = Get.find<AdminService>();

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: const BoxDecoration(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          // Handle bar
          Container(
            width: 40,
            height: 4,
            margin: const EdgeInsets.only(bottom: 20),
            decoration: BoxDecoration(
              color: Colors.grey[300],
              borderRadius: BorderRadius.circular(2),
            ),
          ),
          
          // Title
          Text(
            'Admin Menu',
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          
          const SizedBox(height: 20),
          
          // Pin/Unpin Option
          _buildMenuItem(
            context,
            icon: post.isPinned ? Icons.push_pin : Icons.push_pin_outlined,
            title: post.isPinned ? 'ยกเลิกปักหมุด' : 'ปักหมุดโพสต์',
            subtitle: post.isPinned 
                ? 'ยกเลิกการปักหมุดโพสต์นี้' 
                : 'ปักหมุดโพสต์นี้ไว้ด้านบน',
            onTap: () async {
              Get.back();
              final success = await adminService.togglePinPost(post);
              if (success) {
                onPostUpdated?.call();
              }
            },
          ),
          
          // Delete Option
          _buildMenuItem(
            context,
            icon: Icons.delete_outline,
            title: 'ลบโพสต์',
            subtitle: 'ลบโพสต์นี้ออกจากระบบ',
            isDestructive: true,
            onTap: () async {
              Get.back();
              final success = await adminService.deletePost(post);
              if (success) {
                onPostUpdated?.call();
              }
            },
          ),
          
          // Report Option
          _buildMenuItem(
            context,
            icon: Icons.flag_outlined,
            title: 'รายงานโพสต์',
            subtitle: post.isReported 
                ? 'โพสต์นี้ถูกรายงานแล้ว (${post.reportCount} ครั้ง)'
                : 'รายงานเนื้อหาไม่เหมาะสม',
            onTap: () {
              Get.back();
              _showReportDialog(context, post);
            },
          ),
          
          const SizedBox(height: 10),
          
          // Cancel Button
          SizedBox(
            width: double.infinity,
            child: TextButton(
              onPressed: () => Get.back(),
              style: TextButton.styleFrom(
                padding: const EdgeInsets.symmetric(vertical: 16),
              ),
              child: const Text('ยกเลิก'),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildMenuItem(
    BuildContext context, {
    required IconData icon,
    required String title,
    required String subtitle,
    required VoidCallback onTap,
    bool isDestructive = false,
  }) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(12),
      child: Container(
        width: double.infinity,
        padding: const EdgeInsets.all(16),
        margin: const EdgeInsets.only(bottom: 8),
        decoration: BoxDecoration(
          border: Border.all(
            color: isDestructive ? Colors.red[200]! : Colors.grey[200]!,
          ),
          borderRadius: BorderRadius.circular(12),
        ),
        child: Row(
          children: [
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: isDestructive 
                    ? Colors.red[50] 
                    : Theme.of(context).primaryColor.withOpacity(0.1),
                borderRadius: BorderRadius.circular(8),
              ),
              child: Icon(
                icon,
                color: isDestructive 
                    ? Colors.red[600] 
                    : Theme.of(context).primaryColor,
                size: 24,
              ),
            ),
            
            const SizedBox(width: 16),
            
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    title,
                    style: Theme.of(context).textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.w600,
                      color: isDestructive ? Colors.red[700] : null,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    subtitle,
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                      color: Colors.grey[600],
                    ),
                  ),
                ],
              ),
            ),
            
            Icon(
              Icons.chevron_right,
              color: Colors.grey[400],
            ),
          ],
        ),
      ),
    );
  }

  void _showReportDialog(BuildContext context, PostModel post) {
    final reasons = [
      'เนื้อหาไม่เหมาะสม',
      'สแปมหรือโฆษณา',
      'การล่วงละเมิดหรือคุกคาม',
      'ข้อมูลเท็จ',
      'อื่นๆ',
    ];

    String selectedReason = reasons.first;

    Get.dialog(
      AlertDialog(
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
                  selectedReason = value!;
                  Get.back();
                  _showReportDialog(context, post);
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
              await adminService.reportPost(post, selectedReason);
              onPostUpdated?.call();
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.red,
              foregroundColor: Colors.white,
            ),
            child: const Text('รายงาน'),
          ),
        ],
      ),
    );
  }

  /// Static method สำหรับเรียกใช้ bottom sheet
  static void show(
    BuildContext context, 
    PostModel post, {
    VoidCallback? onPostUpdated,
  }) {
    Get.bottomSheet(
      AdminMenuBottomSheet(
        post: post,
        onPostUpdated: onPostUpdated,
      ),
      backgroundColor: Colors.white,
      isScrollControlled: true,
    );
  }
}