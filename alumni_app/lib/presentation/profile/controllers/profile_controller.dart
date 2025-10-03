import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:alumni_app/core/services/theme_service.dart';
import 'package:alumni_app/core/services/locale_service.dart';
import 'package:alumni_app/core/services/user_session_service.dart';
import 'package:alumni_app/presentation/main_nav/controllers/main_nav_controller.dart';
import 'package:alumni_app/shared/models/post_model.dart';

class ProfileController extends GetxController {
  final RxList<PostModel> userPosts = <PostModel>[].obs;
  final RxBool isLoading = false.obs;
  
  // ✅ เพิ่ม UserSessionService
  final UserSessionService _userSessionService = Get.find<UserSessionService>();
  
  // Mock user data - ใช้จาก UserSessionService
  String get userName => _userSessionService.currentUserName;
  final String userBio = "Alumni Class of 2020 • Software Engineer • Tech Enthusiast";
  final String userAvatar = "";
  final int followersCount = 1234;
  final int followingCount = 567;
  
  @override
  void onInit() {
    super.onInit();
    _loadUserPosts();
  }
  
  void _loadUserPosts() {
    isLoading.value = true;
    
    // Mock user's posts
    final mockPosts = [
      PostModel(
        id: 101,
        author: userName,
        avatar: userAvatar,
        media: "https://images.unsplash.com/photo-1506905925346-21bda4d32df4",
        caption: "My latest project showcase! 🚀",
        likes: 45,
        isLiked: false,
        comments: [],
      ),
      PostModel(
        id: 102,
        author: userName,
        avatar: userAvatar,
        media: "https://images.unsplash.com/photo-1501594907352-04cda38ebc29",
        caption: "Working on some exciting new features ✨",
        likes: 32,
        isLiked: true,
        comments: [],
      ),
      PostModel(
        id: 103,
        author: userName,
        avatar: userAvatar,
        media: "https://images.unsplash.com/photo-1461749280684-dccba630e2f6",
        caption: "Late night coding session 💻 #developer #coding",
        likes: 78,
        isLiked: false,
        comments: [],
      ),
      PostModel(
        id: 104,
        author: userName,
        avatar: userAvatar,
        media: "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2",
        caption: "Team collaboration at its best! 🤝 #teamwork",
        likes: 21,
        isLiked: true,
        comments: [],
      ),
      PostModel(
        id: 105,
        author: userName,
        avatar: userAvatar,
        media: "https://images.unsplash.com/photo-1559136555-9303baea8ebd",
        caption: "Another successful project delivery 📊 #success",
        likes: 56,
        isLiked: false,
        comments: [],
      ),
      PostModel(
        id: 106,
        author: userName,
        avatar: userAvatar,
        media: "https://images.unsplash.com/photo-1551434678-e076c223a692",
        caption: "Creative brainstorming session today 🎨 #creativity #innovation",
        likes: 89,
        isLiked: true,
        comments: [],
      ),
    ];
    
    userPosts.assignAll(mockPosts);
    isLoading.value = false;
  }
  
  void showSettings() {
    Get.bottomSheet(
      _SettingsBottomSheet(),
      backgroundColor: Get.theme.colorScheme.surface,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
    );
  }
  
  void viewPost(PostModel post) {
    // Navigate to profile feed view (Instagram-like feed but user's posts only)
    final startIndex = userPosts.indexWhere((p) => p.id == post.id);
    Get.to(() => _ProfileFeedPage(
      posts: userPosts, 
      initialIndex: startIndex >= 0 ? startIndex : 0,
      userName: userName,
    ));
  }

  void pickImageFromGallery() {
    // Mock functionality
    Get.snackbar(
      'Gallery',
      'Gallery picker functionality will be implemented with image_picker package',
      snackPosition: SnackPosition.BOTTOM,
      backgroundColor: Get.theme.colorScheme.surface,
      colorText: Get.theme.colorScheme.onSurface,
    );
  }

  void pickImageFromCamera() {
    // Mock functionality
    Get.snackbar(
      'Camera',
      'Camera functionality will be implemented with image_picker package',
      snackPosition: SnackPosition.BOTTOM,
      backgroundColor: Get.theme.colorScheme.surface,
      colorText: Get.theme.colorScheme.onSurface,
    );
  }

  void removeProfileImage() {
    // Mock functionality
    Get.snackbar(
      'Remove Photo',
      'Profile photo removed',
      snackPosition: SnackPosition.BOTTOM,
      backgroundColor: Get.theme.colorScheme.surface,
      colorText: Get.theme.colorScheme.onSurface,
    );
  }

  void togglePostLike(int postId) {
    final postIndex = userPosts.indexWhere((post) => post.id == postId);
    if (postIndex != -1) {
      final post = userPosts[postIndex];
      post.isLiked = !post.isLiked;
      post.likes += post.isLiked ? 1 : -1;
      userPosts[postIndex] = post;
      userPosts.refresh(); // Notify observers
    }
  }

  void addPostComment(int postId, String comment) {
    if (comment.trim().isEmpty) return;
    
    final postIndex = userPosts.indexWhere((post) => post.id == postId);
    if (postIndex != -1) {
      final post = userPosts[postIndex];
      // Generate unique ID for comment
      final newId = DateTime.now().millisecondsSinceEpoch;
      post.comments.add(CommentModel(
        id: newId,
        user: _userSessionService.currentUserName,
        text: comment,
      ));
      userPosts[postIndex] = post;
      userPosts.refresh(); // Notify observers
    }
  }

  // ✅ เพิ่มฟังก์ชัน like comment
  void toggleCommentLike(int postId, int commentId) {
    final postIndex = userPosts.indexWhere((post) => post.id == postId);
    if (postIndex != -1) {
      final post = userPosts[postIndex];
      final commentIndex = post.comments.indexWhere((comment) => comment.id == commentId);
      if (commentIndex != -1) {
        final comment = post.comments[commentIndex];
        comment.isLiked = !comment.isLiked;
        comment.likes += comment.isLiked ? 1 : -1;
        userPosts.refresh(); // ✅ Notify UI to update
      }
    }
  }

  // ✅ เพิ่มฟังก์ชัน reply comment
  void addCommentReply(int postId, int parentCommentId, String replyText) {
    if (replyText.trim().isEmpty) return;
    
    final postIndex = userPosts.indexWhere((post) => post.id == postId);
    if (postIndex != -1) {
      final post = userPosts[postIndex];
      final commentIndex = post.comments.indexWhere((comment) => comment.id == parentCommentId);
      if (commentIndex != -1) {
        final parentComment = post.comments[commentIndex];
        final newId = DateTime.now().millisecondsSinceEpoch + 1; // Ensure unique ID
        parentComment.replies.add(CommentModel(
          id: newId,
          user: _userSessionService.currentUserName,
          text: replyText,
        ));
        userPosts.refresh(); // ✅ Notify UI to update
      }
    }
  }

  // ✅ เพิ่มฟังก์ชัน like reply
  void toggleReplyLike(int postId, int parentCommentId, int replyId) {
    final postIndex = userPosts.indexWhere((post) => post.id == postId);
    if (postIndex != -1) {
      final post = userPosts[postIndex];
      final commentIndex = post.comments.indexWhere((comment) => comment.id == parentCommentId);
      if (commentIndex != -1) {
        final parentComment = post.comments[commentIndex];
        final replyIndex = parentComment.replies.indexWhere((reply) => reply.id == replyId);
        if (replyIndex != -1) {
          final reply = parentComment.replies[replyIndex];
          reply.isLiked = !reply.isLiked;
          reply.likes += reply.isLiked ? 1 : -1;
          userPosts.refresh(); // ✅ Notify UI to update
        }
      }
    }
  }

  void _navigateToFeed() {
    // Navigate to main nav and select Feed tab
    final mainNavController = Get.find<MainNavController>();
    mainNavController.onTap(0); // Switch to Feed tab
  }
}

class _SettingsBottomSheet extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    final themeService = Get.find<ThemeService>();
    final localeService = Get.find<LocaleService>();
    
    return Container(
      padding: const EdgeInsets.all(20),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Settings',
            style: Get.textTheme.titleLarge?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 20),
          
          // Dark Mode Toggle
          Obx(() => ListTile(
            leading: Icon(
              themeService.isDarkMode ? Icons.dark_mode : Icons.light_mode,
            ),
            title: const Text('Dark Mode'),
            trailing: Switch(
              value: themeService.isDarkMode,
              onChanged: (_) => themeService.switchTheme(),
            ),
            contentPadding: EdgeInsets.zero,
          )),
          
          // Language Selection
          ListTile(
            leading: const Icon(Icons.language),
            title: const Text('Language'),
            trailing: Obx(() => DropdownButton<String>(
              value: localeService.currentLocale.languageCode,
              underline: Container(),
              items: const [
                DropdownMenuItem(
                  value: 'en',
                  child: Text('English'),
                ),
                DropdownMenuItem(
                  value: 'th',
                  child: Text('ไทย'),
                ),
              ],
              onChanged: (String? value) {
                if (value != null) {
                  localeService.changeLocale(value == 'th' ? const Locale('th', 'TH') : const Locale('en', 'US'));
                }
              },
            )),
            contentPadding: EdgeInsets.zero,
          ),
          
          const SizedBox(height: 20),
        ],
      ),
    );
  }
}

class _ProfileFeedPage extends StatefulWidget {
  final List<PostModel> posts;
  final int initialIndex;
  final String userName;
  
  const _ProfileFeedPage({
    required this.posts,
    required this.initialIndex,
    required this.userName,
  });

  @override
  State<_ProfileFeedPage> createState() => _ProfileFeedPageState();
}

class _ProfileFeedPageState extends State<_ProfileFeedPage> {
  late ScrollController _scrollController;
  late GlobalKey _initialPostKey;

  @override
  void initState() {
    super.initState();
    _scrollController = ScrollController();
    _initialPostKey = GlobalKey();
    
    // Scroll to initial post after build
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _scrollToInitialPost();
    });
  }

  void _scrollToInitialPost() {
    if (widget.initialIndex > 0) {
      // Calculate approximate position (each post card ~ 500px)
      final position = widget.initialIndex * 500.0;
      _scrollController.animateTo(
        position,
        duration: const Duration(milliseconds: 800),
        curve: Curves.easeInOut,
      );
    }
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('${widget.userName}\'s Posts'),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Get.back(),
        ),
        actions: [
          Center(
            child: Padding(
              padding: const EdgeInsets.only(right: 16),
              child: Text(
                '${widget.posts.length} posts',
                style: const TextStyle(fontWeight: FontWeight.w500),
              ),
            ),
          ),
        ],
      ),
      body: RefreshIndicator(
        onRefresh: () async {
          // Simulate refresh
          await Future.delayed(const Duration(seconds: 1));
        },
        child: Obx(() => ListView.builder(
          controller: _scrollController,
          physics: const AlwaysScrollableScrollPhysics(),
          itemCount: widget.posts.length,
          itemBuilder: (context, index) {
            final post = widget.posts[index];
            return Padding(
              key: index == widget.initialIndex ? _initialPostKey : null,
              padding: const EdgeInsets.only(bottom: 8),
              child: _ProfilePostCard(post: post),
            );
          },
        )),
      ),
      bottomNavigationBar: _ProfileFeedBottomNav(),
    );
  }
}

class _ProfileFeedBottomNav extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        border: Border(
          top: BorderSide(
            color: Get.theme.colorScheme.outline.withValues(alpha: 0.2),
          ),
        ),
      ),
      child: BottomNavigationBar(
        currentIndex: 1, // Always show Profile as selected
        onTap: (index) {
          if (index == 0) {
            // Go to Feed
            Get.back(); // Close profile feed
            Get.find<ProfileController>()._navigateToFeed();
          } else if (index == 1) {
            // Stay on Profile - just go back to profile grid
            Get.back();
          }
        },
        items: [
          BottomNavigationBarItem(
            icon: const Icon(Icons.home),
            label: "Feed",
          ),
          BottomNavigationBarItem(
            icon: const Icon(Icons.person),
            label: "Profile",
          ),
        ],
      ),
    );
  }
}

class _ProfilePostCard extends StatefulWidget {
  final PostModel post;
  
  const _ProfilePostCard({required this.post});

  @override
  State<_ProfilePostCard> createState() => _ProfilePostCardState();
}

class _ProfilePostCardState extends State<_ProfilePostCard> {
  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Post Header
          ListTile(
            leading: CircleAvatar(
              backgroundColor: Get.theme.colorScheme.primary,
              child: Text(
                widget.post.author[0].toUpperCase(),
                style: const TextStyle(
                  color: Colors.white,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
            title: Text(
              widget.post.author,
              style: const TextStyle(fontWeight: FontWeight.w600),
            ),
            subtitle: Text('${DateTime.now().difference(DateTime.now().subtract(Duration(hours: widget.post.id))).inHours}h ago'),
          ),
          
          // Post Image Placeholder with Double Tap
          _DoubleTapLikeWidget(
            post: widget.post,
            onDoubleTap: () => Get.find<ProfileController>().togglePostLike(widget.post.id),
            child: Container(
              height: 400,
              width: double.infinity,
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  begin: Alignment.topLeft,
                  end: Alignment.bottomRight,
                  colors: [
                    Get.theme.colorScheme.primary.withValues(alpha: 0.7),
                    Get.theme.colorScheme.secondary.withValues(alpha: 0.7),
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
                      'Photo by ${widget.post.author}',
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
                IconButton(
                  icon: Icon(
                    widget.post.isLiked ? Icons.favorite : Icons.favorite_border,
                    color: widget.post.isLiked ? Colors.red : null,
                  ),
                  onPressed: () {
                    Get.find<ProfileController>().togglePostLike(widget.post.id);
                    setState(() {});
                  },
                ),
                Text('${widget.post.likes}'),
                const SizedBox(width: 16),
                
                // Comment Button
                IconButton(
                  icon: const Icon(Icons.mode_comment_outlined),
                  onPressed: () => _showCommentsDialog(context, widget.post),
                ),
                Text('${widget.post.comments.length}'),
              ],
            ),
          ),
          
          // Caption
          if (widget.post.caption.isNotEmpty)
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
              child: RichText(
                text: TextSpan(
                  style: DefaultTextStyle.of(context).style,
                  children: [
                    TextSpan(
                      text: '${widget.post.author} ',
                      style: const TextStyle(fontWeight: FontWeight.w600),
                    ),
                    TextSpan(text: widget.post.caption),
                  ],
                ),
              ),
            ),
          
          // Comments preview
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Comments preview
                if (widget.post.comments.isNotEmpty)
                  GestureDetector(
                    onTap: () => _showCommentsDialog(context, widget.post),
                    child: Text(
                      widget.post.comments.length == 1 
                        ? 'View 1 comment'
                        : 'View all ${widget.post.comments.length} comments',
                      style: TextStyle(
                        color: Get.theme.colorScheme.onSurface.withValues(alpha: 0.6),
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

  void _showCommentsDialog(BuildContext context, PostModel post) {
    final commentController = TextEditingController();
    String? replyToUser; // ✅ ตัวแปรเก็บว่ากำลัง reply ใคร
    int? replyToCommentId; // ✅ เก็บ comment id ที่กำลัง reply
    
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      builder: (context) => StatefulBuilder( // ✅ ใช้ StatefulBuilder เหมือนหน้า Feed
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
                        color: Get.theme.dividerColor,
                      ),
                    ),
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        'Comments',
                        style: Get.theme.textTheme.titleMedium,
                      ),
                      TextButton(
                        onPressed: () => Navigator.pop(context),
                        child: const Text('Done'),
                      ),
                    ],
                  ),
                ),
                
                // Comments List with Obx for reactive updates
                Expanded(
                  child: Obx(() {
                    final profileController = Get.find<ProfileController>();
                    final currentPost = profileController.userPosts.firstWhere(
                      (p) => p.id == post.id,
                      orElse: () => post,
                    );
                    
                    return currentPost.comments.isEmpty
                        ? Center(
                            child: Column(
                              mainAxisAlignment: MainAxisAlignment.center,
                              children: [
                                Icon(
                                  Icons.comment_outlined,
                                  size: 48,
                                  color: Get.theme.colorScheme.onSurface.withValues(alpha: 0.3),
                                ),
                                const SizedBox(height: 8),
                                Text(
                                  'No comments yet',
                                  style: TextStyle(
                                    color: Get.theme.colorScheme.onSurface.withValues(alpha: 0.6),
                                  ),
                                ),
                              ],
                            ),
                          )
                        : ListView.builder(
                            itemCount: currentPost.comments.length,
                            itemBuilder: (context, index) {
                              final comment = currentPost.comments[index];
                              return _CommentTile(
                                post: currentPost,
                                comment: comment,
                                onReply: (String username, int commentId) {
                                  // ✅ Callback เมื่อกด Reply เหมือนหน้า Feed
                                  setState(() {
                                    replyToUser = username;
                                    replyToCommentId = commentId;
                                    commentController.text = '@$username ';
                                  });
                                },
                              );
                            },
                          );
                  }),
                ),
                
                // Reply indicator + Add Comment (เหมือนหน้า Feed)
                Column(
                  children: [
                    // ✅ แสดงว่ากำลัง reply ใคร (เหนือ input field)
                    if (replyToUser != null)
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                        color: Get.theme.colorScheme.primaryContainer.withValues(alpha: 0.3),
                        child: Row(
                          children: [
                            Icon(
                              Icons.reply,
                              size: 16,
                              color: Get.theme.colorScheme.primary,
                            ),
                            const SizedBox(width: 8),
                            Text(
                              'Replying to $replyToUser',
                              style: TextStyle(
                                fontSize: 12,
                                color: Get.theme.colorScheme.primary,
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
                                color: Get.theme.colorScheme.primary,
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
                            color: Get.theme.dividerColor,
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
                              textInputAction: TextInputAction.send,
                              onSubmitted: (_) {
                                if (commentController.text.trim().isNotEmpty) {
                                  final controller = Get.find<ProfileController>();
                                  if (replyToUser != null && replyToCommentId != null) {
                                    // ✅ เป็น Reply
                                    controller.addCommentReply(
                                      post.id, 
                                      replyToCommentId!, 
                                      commentController.text.trim(),
                                    );
                                  } else {
                                    // ✅ เป็น Comment ธรรมดา
                                    controller.addPostComment(post.id, commentController.text.trim());
                                  }
                                  
                                  setState(() {
                                    replyToUser = null;
                                    replyToCommentId = null;
                                  });
                                  commentController.clear();
                                }
                              },
                            ),
                          ),
                          TextButton(
                            onPressed: () {
                              if (commentController.text.trim().isNotEmpty) {
                                final controller = Get.find<ProfileController>();
                                if (replyToUser != null && replyToCommentId != null) {
                                  // ✅ เป็น Reply
                                  controller.addCommentReply(
                                    post.id, 
                                    replyToCommentId!, 
                                    commentController.text.trim(),
                                  );
                                } else {
                                  // ✅ เป็น Comment ธรรมดา
                                  controller.addPostComment(post.id, commentController.text.trim());
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
            ),
          ),
        ),
      ),
    );
  }
}

// Reusable Double Tap Like Widget
class _DoubleTapLikeWidget extends StatefulWidget {
  final Widget child;
  final VoidCallback onDoubleTap;
  final PostModel post;

  const _DoubleTapLikeWidget({
    required this.child,
    required this.onDoubleTap,
    required this.post,
  });

  @override
  State<_DoubleTapLikeWidget> createState() => _DoubleTapLikeWidgetState();
}

class _DoubleTapLikeWidgetState extends State<_DoubleTapLikeWidget>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _scaleAnimation;
  late Animation<double> _opacityAnimation;
  bool _showAnimation = false;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 800),
      vsync: this,
    );

    _scaleAnimation = Tween<double>(
      begin: 0.5,
      end: 1.2,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.elasticOut,
    ));

    _opacityAnimation = Tween<double>(
      begin: 1.0,
      end: 0.0,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: const Interval(0.5, 1.0, curve: Curves.easeOut),
    ));

    _animationController.addStatusListener((status) {
      if (status == AnimationStatus.completed) {
        setState(() {
          _showAnimation = false;
        });
        _animationController.reset();
      }
    });
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  void _handleDoubleTap() {
    // ✅ เก็บสถานะก่อน toggle เพื่อตรวจสอบว่าเป็นการ like หรือ unlike
    final bool wasLiked = widget.post.isLiked;
    
    widget.onDoubleTap(); // เรียก togglePostLike()
    
    // ✅ แสดง animation เฉพาะเมื่อเป็นการ Like (จาก false -> true)
    if (!wasLiked) {
      setState(() {
        _showAnimation = true;
      });
      _animationController.forward();
    }
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onDoubleTap: _handleDoubleTap,
      child: Stack(
        alignment: Alignment.center, // ✅ ให้ Stack จัดตำแหน่งกลาง
        children: [
          widget.child,
          if (_showAnimation)
            Positioned.fill( // ✅ ให้ครอบคลุมทั้งพื้นที่
              child: Center( // ✅ จัดกลางในพื้นที่
                child: AnimatedBuilder(
                  animation: _animationController,
                  builder: (context, child) {
                    return Opacity(
                      opacity: _opacityAnimation.value,
                      child: Transform.scale(
                        scale: _scaleAnimation.value,
                        child: const Icon(
                          Icons.favorite,
                          color: Colors.red,
                          size: 100,
                        ),
                      ),
                    );
                  },
                ),
              ),
            ),
        ],
      ),
    );
  }
}

// ✅ Comment Tile Widget with Like & Reply functionality
class _CommentTile extends StatefulWidget {
  final PostModel post;
  final CommentModel comment;
  final bool isReply;
  final Function(String username, int commentId)? onReply; // ✅ เปลี่ยนเป็น Function เหมือนหน้า Feed
  
  const _CommentTile({
    required this.post,
    required this.comment,
    this.isReply = false,
    this.onReply, // ✅ รับ callback
  });

  @override
  State<_CommentTile> createState() => _CommentTileState();
}

class _CommentTileState extends State<_CommentTile> {
  @override
  Widget build(BuildContext context) {
    return Container(
      padding: EdgeInsets.only(
        left: widget.isReply ? 56 : 16,
        right: 16,
        top: 8,
        bottom: 8,
      ),
      decoration: widget.isReply ? BoxDecoration(
        border: Border(
          left: BorderSide(
            color: Get.theme.colorScheme.primary.withValues(alpha: 0.3),
            width: 2,
          ),
        ),
      ) : null,
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Avatar
          CircleAvatar(
            radius: widget.isReply ? 12 : 16,
            backgroundColor: Get.theme.colorScheme.secondary,
            child: Text(
              widget.comment.user[0].toUpperCase(),
              style: TextStyle(
                color: Colors.white,
                fontSize: widget.isReply ? 10 : 12,
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
                        text: '${widget.comment.user} ',
                        style: const TextStyle(fontWeight: FontWeight.w600),
                      ),
                      TextSpan(text: widget.comment.text),
                    ],
                  ),
                ),
                
                const SizedBox(height: 4),
                
                // Actions row
                Row(
                  children: [
                    // Time ago
                    Text(
                      _getTimeAgo(widget.comment.createdAt),
                      style: TextStyle(
                        fontSize: 12,
                        color: Get.theme.colorScheme.onSurface.withValues(alpha: 0.6),
                      ),
                    ),
                    
                    const SizedBox(width: 16),
                    
                    // Like button - ✅ เพิ่มพื้นที่กดให้ใหญ่ขึ้น
                    InkWell(
                      onTap: () => _toggleCommentLike(),
                      borderRadius: BorderRadius.circular(20),
                      child: Padding(
                        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                        child: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Icon(
                              widget.comment.isLiked ? Icons.favorite : Icons.favorite_border,
                              size: 16, // ✅ เพิ่มขนาดไอคอน
                              color: widget.comment.isLiked ? Colors.red : Get.theme.colorScheme.onSurface.withValues(alpha: 0.6),
                            ),
                            const SizedBox(width: 6), // ✅ เพิ่มระยะห่าง
                            Text(
                              '${widget.comment.likes}',
                              style: TextStyle(
                                fontSize: 13, // ✅ เพิ่มขนาดตัวอักษร
                                color: widget.comment.isLiked ? Colors.red : Get.theme.colorScheme.onSurface.withValues(alpha: 0.6),
                                fontWeight: widget.comment.isLiked ? FontWeight.w600 : FontWeight.normal,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                    
                    const SizedBox(width: 16),
                    
                    // Reply button (only for main comments) - ✅ เพิ่มพื้นที่กดให้ใหญ่ขึ้น
                    if (!widget.isReply && widget.onReply != null)
                      InkWell(
                        onTap: () => widget.onReply!(widget.comment.user, widget.comment.id), // ✅ เรียก callback พร้อม parameter
                        borderRadius: BorderRadius.circular(20),
                        child: Padding(
                          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                          child: Text(
                            'Reply',
                            style: TextStyle(
                              fontSize: 13, // ✅ เพิ่มขนาดตัวอักษร
                              color: Get.theme.colorScheme.onSurface.withValues(alpha: 0.7),
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                        ),
                      ),
                  ],
                ),
                
                // Replies
                if (!widget.isReply && widget.comment.replies.isNotEmpty) ...[
                  const SizedBox(height: 8),
                  ...widget.comment.replies.map((reply) => _CommentTile(
                    post: widget.post,
                    comment: reply,
                    isReply: true,
                    // ✅ ไม่ส่ง onReply สำหรับ replies (ไม่มี nested replies)
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
    final controller = Get.find<ProfileController>();
    if (widget.isReply) {
      // Find parent comment
      final parentComment = widget.post.comments.firstWhere(
        (c) => c.replies.any((r) => r.id == widget.comment.id),
      );
      controller.toggleReplyLike(widget.post.id, parentComment.id, widget.comment.id);
    } else {
      controller.toggleCommentLike(widget.post.id, widget.comment.id);
    }
    // Force UI rebuild
    setState(() {});
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