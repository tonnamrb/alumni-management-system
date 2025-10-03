import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:flutter_i18n/flutter_i18n.dart';
import 'package:alumni_app/presentation/feed/controllers/feed_controller.dart';
import 'package:alumni_app/shared/models/post_model.dart';
import 'package:alumni_app/shared/widgets/double_tap_like_widget.dart';
import 'package:alumni_app/presentation/widgets/admin_menu_button.dart';

class FeedPage extends GetView<FeedController> {
  const FeedPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(FlutterI18n.translate(context, "feed.title")),
        actions: [
          IconButton(
            icon: const Icon(Icons.add_photo_alternate),
            onPressed: controller.uploadPost,
          ),
        ],
      ),
      body: Obx(() {
        if (controller.isLoading.value) {
          return const Center(
            child: CircularProgressIndicator(),
          );
        }
        
        if (controller.posts.isEmpty) {
          return Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(
                  Icons.photo_library_outlined,
                  size: 64,
                  color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.3),
                ),
                const SizedBox(height: 16),
                Text(
                  FlutterI18n.translate(context, "feed.no_posts"),
                  style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                    color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.6),
                  ),
                ),
              ],
            ),
          );
        }
        
        return RefreshIndicator(
          onRefresh: () async {
            // Simulate refresh
            await Future.delayed(const Duration(seconds: 1));
          },
          child: ListView.builder(
            physics: const AlwaysScrollableScrollPhysics(),
            itemCount: controller.posts.length,
            itemBuilder: (context, index) {
              final post = controller.posts[index];
              return _PostCard(post: post, controller: controller);
            },
          ),
        );
      }),
    );
  }
}

class _PostCard extends StatelessWidget {
  final PostModel post;
  final FeedController controller;
  
  const _PostCard({
    required this.post,
    required this.controller,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.symmetric(vertical: 4),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Post Header
          ListTile(
            leading: GestureDetector(
              onTap: () => _navigateToUserProfile(post.author),
              child: CircleAvatar(
                backgroundColor: Theme.of(context).colorScheme.primary,
                child: Text(
                  post.author[0],
                  style: const TextStyle(
                    color: Colors.white,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ),
            title: GestureDetector(
              onTap: () => _navigateToUserProfile(post.author),
              child: Row(
                children: [
                  Text(
                    post.author,
                    style: const TextStyle(fontWeight: FontWeight.w600),
                  ),
                  if (post.isPinned) ...[
                    const SizedBox(width: 8),
                    Icon(
                      Icons.push_pin,
                      size: 16,
                      color: Theme.of(context).primaryColor,
                    ),
                  ],
                  if (post.isReported) ...[
                    const SizedBox(width: 8),
                    Icon(
                      Icons.flag,
                      size: 16,
                      color: Colors.orange,
                    ),
                  ],
                ],
              ),
            ),
            subtitle: Text('${DateTime.now().difference(DateTime.now().subtract(Duration(hours: post.id))).inHours}h'),
          ),
          
          // Post Image Placeholder with Double Tap
          DoubleTapLikeWidget(
            post: post,
            onDoubleTap: () => controller.toggleLike(post.id),
            getCurrentLikeState: () {
              final currentPost = controller.posts.firstWhere((p) => p.id == post.id);
              return currentPost.isLiked;
            },
            child: Container(
              height: 300,
              width: double.infinity,
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  begin: Alignment.topLeft,
                  end: Alignment.bottomRight,
                  colors: [
                    Theme.of(context).colorScheme.primary.withValues(alpha: 0.7),
                    Theme.of(context).colorScheme.secondary.withValues(alpha: 0.7),
                  ],
                ),
              ),
              child: Center(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Icon(
                      Icons.photo_library,
                      size: 64,
                      color: Colors.white.withValues(alpha: 0.7),
                    ),
                    const SizedBox(height: 8),
                    Text(
                      'Photo by ${post.author}',
                      style: const TextStyle(
                        color: Colors.white,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
          
          // Post Actions
          Padding(
            padding: const EdgeInsets.all(8),
            child: Row(
              children: [
                // Like Button
                Obx(() {
                  // Find current post from controller to get updated state
                  final currentPost = controller.posts.firstWhere((p) => p.id == post.id);
                  return Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      IconButton(
                        icon: Icon(
                          currentPost.isLiked ? Icons.favorite : Icons.favorite_border,
                          color: currentPost.isLiked ? Colors.red : null,
                        ),
                        onPressed: () => controller.toggleLike(post.id),
                      ),
                      Text('${currentPost.likes}'),
                    ],
                  );
                }),
                const SizedBox(width: 16),
                
                // Comment Button
                Obx(() {
                  final currentPost = controller.posts.firstWhere((p) => p.id == post.id);
                  return Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      IconButton(
                        icon: const Icon(Icons.mode_comment_outlined),
                        onPressed: () => _showCommentsDialog(context, currentPost),
                      ),
                      Text('${currentPost.comments.length}'),
                    ],
                  );
                }),
                
                const Spacer(),
                
                // Admin Menu Button
                AdminMenuButton(
                  post: post,
                  onPostUpdated: () => controller.refreshPosts(),
                ),
              ],
            ),
          ),
          
          // Caption (เพิ่มส่วนนี้ใหม่)
          if (post.caption.isNotEmpty)
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
              child: RichText(
                text: TextSpan(
                  style: DefaultTextStyle.of(context).style,
                  children: [
                    TextSpan(
                      text: '${post.author} ',
                      style: const TextStyle(fontWeight: FontWeight.w600),
                    ),
                    TextSpan(text: post.caption),
                  ],
                ),
              ),
            ),
          
          // Comments Preview (แก้ไขให้ match กับ user_profile)
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Comments preview (แก้ไขให้ match กับ user_profile)
                if (post.comments.isNotEmpty)
                  GestureDetector(
                    onTap: () => _showCommentsDialog(context, post),
                    child: Text(
                      post.comments.length == 1 
                        ? 'View 1 comment'
                        : 'View all ${post.comments.length} comments',
                      style: TextStyle(
                        color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.6),
                      ),
                    ),
                  ),
                    
                const SizedBox(height: 8),
              ],
            ),
          ),
        ],
      ),
    );
  }
  
  void _navigateToUserProfile(String userName) {
    // Map ชื่อ user เป็น userId
    String userId;
    switch (userName) {
      case 'Alice Johnson':
        userId = 'alice';
        break;
      case 'Bob Smith':
        userId = 'bob';
        break;
      case 'Carol Davis':
        userId = 'carol';
        break;
      default:
        // ถ้าเป็นตัวเองก็ไม่ต้องไป user profile
        if (userName == controller.getCurrentUserName()) {
          return;
        }
        // สำหรับ user อื่นๆ ใช้ชื่อแปลงเป็น id
        userId = userName.toLowerCase().replaceAll(' ', '');
    }
    
    Get.toNamed(
      '/user-profile',
      arguments: {'userId': userId},
    );
  }
  
  void _showCommentsDialog(BuildContext context, PostModel post) {
    final commentController = TextEditingController();
    String? replyToUser; // ✅ ตัวแปรเก็บว่ากำลัง reply ใคร
    int? replyToCommentId; // ✅ เก็บ comment id ที่กำลัง reply
    
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      builder: (context) => StatefulBuilder( // ✅ ใช้ StatefulBuilder เพื่อ update UI
        builder: (context, setState) => Padding(
          padding: EdgeInsets.only(
            bottom: MediaQuery.of(context).viewInsets.bottom,
          ),
          child: Container(
            constraints: BoxConstraints(
              maxHeight: MediaQuery.of(context).size.height * 0.7,
            ),
            child: Column(
            children: [
              // Header
              Container(
                padding: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  border: Border(
                    bottom: BorderSide(
                      color: Theme.of(context).dividerColor,
                    ),
                  ),
                ),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'Comments',
                      style: Theme.of(context).textTheme.titleMedium,
                    ),
                    TextButton(
                      onPressed: () => Navigator.pop(context),
                      child: const Text('Done'),
                    ),
                  ],
                ),
              ),
              
              // Comments List
              Expanded(
                child: post.comments.isEmpty
                    ? Center(
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Icon(
                              Icons.comment_outlined,
                              size: 48,
                              color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.3),
                            ),
                            const SizedBox(height: 8),
                            Text(
                              'No comments yet',
                              style: TextStyle(
                                color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.6),
                              ),
                            ),
                          ],
                        ),
                      )
                    : Obx(() => ListView.builder( // ✅ ใช้ Obx เพื่อ listen การเปลี่ยนแปลง
                        itemCount: controller.posts.firstWhere((p) => p.id == post.id).comments.length,
                        itemBuilder: (context, index) {
                          // ✅ ดึงข้อมูลล่าสุดจาก controller
                          final currentPost = controller.posts.firstWhere((p) => p.id == post.id);
                          final comment = currentPost.comments[index];
                          return _CommentTile(
                            post: currentPost,
                            comment: comment,
                            onReply: (String username, int commentId) {
                              // ✅ Callback เมื่อกด Reply
                              setState(() {
                                replyToUser = username;
                                replyToCommentId = commentId;
                                commentController.text = '@$username ';
                              });
                            },
                          );
                        },
                      )),
              ),
              
              // Reply indicator + Add Comment
              Column(
                children: [
                  // ✅ แสดงว่ากำลัง reply ใคร
                  if (replyToUser != null)
                    Container(
                      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                      color: Theme.of(context).colorScheme.primaryContainer.withValues(alpha: 0.3),
                      child: Row(
                        children: [
                          Icon(
                            Icons.reply,
                            size: 16,
                            color: Theme.of(context).colorScheme.primary,
                          ),
                          const SizedBox(width: 8),
                          Text(
                            'Replying to $replyToUser',
                            style: TextStyle(
                              fontSize: 12,
                              color: Theme.of(context).colorScheme.primary,
                              fontWeight: FontWeight.w500,
                            ),
                          ),
                          const Spacer(),
                          GestureDetector(
                            onTap: () {
                              setState(() {
                                replyToUser = null;
                                replyToCommentId = null;
                                commentController.clear();
                              });
                            },
                            child: Icon(
                              Icons.close,
                              size: 16,
                              color: Theme.of(context).colorScheme.primary,
                            ),
                          ),
                        ],
                      ),
                    ),
                  
                  // Comment input
                  Container(
                    padding: const EdgeInsets.all(16),
                    decoration: BoxDecoration(
                      border: Border(
                        top: BorderSide(
                          color: Theme.of(context).dividerColor,
                        ),
                      ),
                    ),
                    child: Row(
                      children: [
                        Expanded(
                          child: TextField(
                            controller: commentController,
                            decoration: InputDecoration(
                              hintText: replyToUser != null 
                                ? 'Write a reply...' 
                                : 'Add a comment...',
                              border: InputBorder.none,
                            ),
                          ),
                        ),
                        TextButton(
                          onPressed: () {
                            if (commentController.text.trim().isNotEmpty) {
                              if (replyToUser != null && replyToCommentId != null) {
                                // ✅ เป็น Reply
                                controller.addCommentReply(
                                  post.id, 
                                  replyToCommentId!, 
                                  commentController.text.trim(),
                                );
                              } else {
                                // ✅ เป็น Comment ธรรมดา
                                controller.addComment(post.id, commentController.text.trim());
                              }
                              
                              setState(() {
                                replyToUser = null;
                                replyToCommentId = null;
                              });
                              commentController.clear();
                            }
                          },
                          child: Text(replyToUser != null ? 'Reply' : 'Post'),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ],
          ), // ปิด Column
        ), // ปิด Container
      ), // ปิด Padding 
    ), // ปิด StatefulBuilder
    ); // ปิด showModalBottomSheet
  }
}


// ✅ Comment Tile Widget with Like & Reply functionality (Feed Version)
class _CommentTile extends StatelessWidget {
  final dynamic post; // PostModel from FeedController
  final dynamic comment; // CommentModel
  final bool isReply;
  final Function(String username, int commentId)? onReply; // ✅ Callback สำหรับ reply
  
  const _CommentTile({
    required this.post,
    required this.comment,
    this.isReply = false,
    this.onReply,
  });

  @override
  Widget build(BuildContext context) {
    // ✅ ใช้ Obx เพื่อ listen การเปลี่ยนแปลงของ comment
    return Obx(() {
      // ✅ หา comment ปัจจุบันจาก controller
      final controller = Get.find<FeedController>();
      final currentPost = controller.posts.firstWhere((p) => p.id == post.id);
      
      dynamic currentComment;
      if (isReply) {
        // ถ้าเป็น reply ให้หาจาก parent comment
        final parentComment = currentPost.comments.firstWhere(
          (c) => c.replies.any((r) => r.id == comment.id),
        );
        currentComment = parentComment.replies.firstWhere((r) => r.id == comment.id);
      } else {
        // ถ้าเป็น main comment
        currentComment = currentPost.comments.firstWhere((c) => c.id == comment.id);
      }
      
      return _buildCommentTile(context, currentComment);
    });
  }

  Widget _buildCommentTile(BuildContext context, dynamic currentComment) {
    return Container(
      padding: EdgeInsets.only(
        left: isReply ? 32 : 16, // ✅ ลดระยะห่างจาก 56 เป็น 32
        right: 16,
        top: 8,
        bottom: 8,
      ),
      decoration: isReply ? BoxDecoration(
        border: Border(
          left: BorderSide(
            color: Theme.of(context).colorScheme.primary.withValues(alpha: 0.3),
            width: 2,
          ),
        ),
      ) : null,
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Avatar
          CircleAvatar(
            radius: isReply ? 12 : 16,
            backgroundColor: Theme.of(context).colorScheme.secondary,
            child: Text(
              currentComment.user[0].toUpperCase(), // ✅ ใช้ currentComment
              style: TextStyle(
                color: Colors.white,
                fontSize: isReply ? 10 : 12,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          
          const SizedBox(width: 12),
          
          // Content
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Username and comment text
                RichText(
                  text: TextSpan(
                    style: DefaultTextStyle.of(context).style,
                    children: [
                      TextSpan(
                        text: '${currentComment.user} ', // ✅ ใช้ currentComment
                        style: const TextStyle(fontWeight: FontWeight.w600),
                      ),
                      TextSpan(text: currentComment.text), // ✅ ใช้ currentComment
                    ],
                  ),
                ),
                
                const SizedBox(height: 4),
                
                // Actions row
                Row(
                  children: [
                    // Time ago
                    Text(
                      _getTimeAgo(currentComment.createdAt), // ✅ ใช้ currentComment
                      style: TextStyle(
                        fontSize: 12,
                        color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.6),
                      ),
                    ),
                    
                    const SizedBox(width: 16),
                    
                    // Like button - ✅ เพิ่มพื้นที่กดให้ใหญ่ขึ้น
                    InkWell(
                      onTap: () => _toggleCommentLike(), // ✅ ไม่ส่ง parameter
                      borderRadius: BorderRadius.circular(20),
                      child: Padding(
                        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                        child: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Icon(
                              currentComment.isLiked ? Icons.favorite : Icons.favorite_border, // ✅ ใช้ currentComment
                              size: 16, // ✅ เพิ่มขนาดไอคอน
                              color: currentComment.isLiked ? Colors.red : Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.6), // ✅ ใช้ currentComment
                            ),
                            const SizedBox(width: 6), // ✅ เพิ่มระยะห่าง
                            Text(
                              '${currentComment.likes}', // ✅ ใช้ currentComment
                              style: TextStyle(
                                fontSize: 13, // ✅ เพิ่มขนาดตัวอักษร
                                color: currentComment.isLiked ? Colors.red : Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.6), // ✅ ใช้ currentComment
                                fontWeight: currentComment.isLiked ? FontWeight.w600 : FontWeight.normal, // ✅ ใช้ currentComment
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                    
                    const SizedBox(width: 16),
                    
                    // Reply button (only for main comments) - ✅ เรียก callback แทน dialog
                    if (!isReply && onReply != null)
                      InkWell(
                        onTap: () => onReply!(currentComment.user, currentComment.id), // ✅ ใช้ currentComment
                        borderRadius: BorderRadius.circular(20),
                        child: Padding(
                          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                          child: Text(
                            'Reply',
                            style: TextStyle(
                              fontSize: 13, // ✅ เพิ่มขนาดตัวอักษร
                              color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.7),
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                        ),
                      ),
                  ],
                ),
                
                // Replies
                if (!isReply && currentComment.replies.isNotEmpty) ...[
                  const SizedBox(height: 8),
                  ...currentComment.replies.map((reply) => _CommentTile( // ✅ ใช้ currentComment
                    post: post,
                    comment: reply,
                    isReply: true,
                    onReply: null, // ✅ ไม่สามารถ reply ต่อ reply ได้
                  )),
                ],
              ],
            ),
          ),
        ],
      ),
    );
  }

  void _toggleCommentLike() {
    final controller = Get.find<FeedController>();
    final currentPost = controller.posts.firstWhere((p) => p.id == post.id);
    
    if (isReply) {
      // Find parent comment
      final parentComment = currentPost.comments.firstWhere(
        (c) => c.replies.any((r) => r.id == comment.id),
      );
      controller.toggleReplyLike(post.id, parentComment.id, comment.id);
    } else {
      controller.toggleCommentLike(post.id, comment.id);
    }
  }



  String _getTimeAgo(DateTime dateTime) {
    final now = DateTime.now();
    final difference = now.difference(dateTime);
    
    if (difference.inDays > 0) {
      return '${difference.inDays}d';
    } else if (difference.inHours > 0) {
      return '${difference.inHours}h';
    } else if (difference.inMinutes > 0) {
      return '${difference.inMinutes}m';
    } else {
      return 'now';
    }
  }
}