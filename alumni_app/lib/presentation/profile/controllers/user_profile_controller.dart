import 'package:get/get.dart';
import 'package:alumni_app/core/services/user_session_service.dart';
import 'package:alumni_app/presentation/profile/pages/user_profile_page.dart';
import 'package:alumni_app/shared/models/post_model.dart';

/// Controller สำหรับจัดการหน้าโปรไฟล์ของผู้ใช้อื่น
class UserProfileController extends GetxController {
  final RxList<PostModel> userPosts = <PostModel>[].obs;
  final RxBool isLoading = false.obs;
  final Rx<UserModel?> targetUser = Rx<UserModel?>(null);
  final RxBool isFollowing = false.obs;
  
  // Services
  final UserSessionService _userSessionService = Get.find<UserSessionService>();
  
  // Properties
  late String userId;
  
  @override
  void onInit() {
    super.onInit();
    // ดึง userId จาก arguments
    userId = Get.arguments?['userId'] ?? '';
    if (userId.isNotEmpty) {
      loadUserProfile();
    }
  }
  
  /// โหลดข้อมูลโปรไฟล์ของผู้ใช้
  Future<void> loadUserProfile() async {
    isLoading.value = true;
    
    try {
      // TODO: In real app, call API to get user profile
      await Future.delayed(const Duration(seconds: 1));
      
      // Mock user data based on userId
      targetUser.value = _getMockUserData(userId);
      
      // Check if following
      isFollowing.value = _checkIfFollowing(userId);
      
      // Load user posts
      await _loadUserPosts();
      
    } catch (e) {
      Get.snackbar(
        'Error',
        'Failed to load user profile: $e',
        snackPosition: SnackPosition.BOTTOM,
      );
    } finally {
      isLoading.value = false;
    }
  }
  
  /// โหลดโพสต์ของผู้ใช้
  Future<void> _loadUserPosts() async {
    // Mock posts data
    userPosts.value = _getMockUserPosts(userId);
  }
  
  /// Toggle การติดตาม
  void toggleFollow() {
    isFollowing.value = !isFollowing.value;
    
    if (isFollowing.value) {
      // Increase followers count
      final user = targetUser.value;
      if (user != null) {
        targetUser.value = UserModel(
          id: user.id,
          name: user.name,
          bio: user.bio,
          avatar: user.avatar,
          followersCount: user.followersCount + 1,
          followingCount: user.followingCount,
        );
      }
      
      Get.snackbar(
        'Followed',
        'You are now following ${targetUser.value?.name}',
        snackPosition: SnackPosition.BOTTOM,
      );
    } else {
      // Decrease followers count
      final user = targetUser.value;
      if (user != null) {
        targetUser.value = UserModel(
          id: user.id,
          name: user.name,
          bio: user.bio,
          avatar: user.avatar,
          followersCount: user.followersCount - 1,
          followingCount: user.followingCount,
        );
      }
      
      Get.snackbar(
        'Unfollowed',
        'You unfollowed ${targetUser.value?.name}',
        snackPosition: SnackPosition.BOTTOM,
      );
    }
  }
  
  /// Toggle like โพสต์
  void togglePostLike(int postId) {
    toggleLike(postId); // ใช้ method เดียวกัน
  }
  
  /// เพิ่ม comment
  void addComment(int postId, String comment) {
    if (comment.trim().isEmpty) return;
    
    final postIndex = userPosts.indexWhere((post) => post.id == postId);
    if (postIndex != -1) {
      final post = userPosts[postIndex];
      final newId = DateTime.now().millisecondsSinceEpoch;
      post.comments.add(CommentModel(
        id: newId,
        user: _userSessionService.currentUserName,
        text: comment,
      ));
      userPosts[postIndex] = post;
      userPosts.refresh();
    }
  }
  
  /// Toggle like comment
  void toggleCommentLike(int postId, int commentId) {
    final postIndex = userPosts.indexWhere((post) => post.id == postId);
    if (postIndex != -1) {
      final post = userPosts[postIndex];
      final commentIndex = post.comments.indexWhere((comment) => comment.id == commentId);
      if (commentIndex != -1) {
        final comment = post.comments[commentIndex];
        comment.isLiked = !comment.isLiked;
        comment.likes += comment.isLiked ? 1 : -1;
        userPosts.refresh();
      }
    }
  }
  
  /// Reply comment
  void addCommentReply(int postId, int parentCommentId, String replyText) {
    if (replyText.trim().isEmpty) return;
    
    final postIndex = userPosts.indexWhere((post) => post.id == postId);
    if (postIndex != -1) {
      final post = userPosts[postIndex];
      final commentIndex = post.comments.indexWhere((comment) => comment.id == parentCommentId);
      if (commentIndex != -1) {
        final parentComment = post.comments[commentIndex];
        final newId = DateTime.now().millisecondsSinceEpoch + 1;
        parentComment.replies.add(CommentModel(
          id: newId,
          user: _userSessionService.currentUserName,
          text: replyText,
        ));
        userPosts.refresh();
      }
    }
  }
  
  /// Toggle like reply
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
          userPosts.refresh();
        }
      }
    }
  }
  
  /// View post detail - เหมือน ProfileController
  void viewPost(dynamic post, {bool videoMode = false}) {
    Get.to(() => UserProfilePostDetail(
      initialPost: post, 
      controller: this,
      isVideoMode: videoMode,
    ));
  }
  
  // Mock data methods
  UserModel _getMockUserData(String userId) {
    switch (userId) {
      case 'alice':
      case 'alicejohnson':
        return UserModel(
          id: 'alice',
          name: 'Alice Johnson',
          bio: 'Alumni Class of 2019 • UX Designer • Love traveling and photography 📸✈️',
          avatar: 'https://images.unsplash.com/photo-1494790108755-2616b612b786?w=400',
          followersCount: 856,
          followingCount: 423,
        );
      case 'bob':
      case 'bobsmith':
        return UserModel(
          id: 'bob',
          name: 'Bob Smith',
          bio: 'Alumni Class of 2021 • Full Stack Developer • Coffee enthusiast ☕💻',
          avatar: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400',
          followersCount: 642,
          followingCount: 312,
        );
      case 'carol':
      case 'caroldavis':
        return UserModel(
          id: 'carol',
          name: 'Carol Davis',
          bio: 'Alumni Class of 2020 • Product Manager • Tech for good 🌍💡',
          avatar: 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=400',
          followersCount: 1234,
          followingCount: 567,
        );
      case 'david':
      case 'davidwilson':
        return UserModel(
          id: 'david',
          name: 'David Wilson',
          bio: 'Alumni Class of 2018 • Data Scientist • AI & Machine Learning enthusiast 🤖📊',
          avatar: 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=400',
          followersCount: 987,
          followingCount: 234,
        );
      case 'emma':
      case 'emmabrown':
        return UserModel(
          id: 'emma',
          name: 'Emma Brown',
          bio: 'Alumni Class of 2022 • Mobile App Developer • Flutter & React Native 📱💙',
          avatar: 'https://images.unsplash.com/photo-1487412720507-e7ab37603c6f?w=400',
          followersCount: 543,
          followingCount: 678,
        );
      default:
        // ถ้าไม่เจอ user ให้สร้าง user ใหม่จากชื่อ
        final displayName = _generateDisplayNameFromId(userId);
        return UserModel(
          id: userId,
          name: displayName,
          bio: 'Alumni member • New to the community 🎓',
          avatar: _getRandomAvatar(),
          followersCount: _getRandomNumber(50, 300),
          followingCount: _getRandomNumber(20, 150),
        );
    }
  }
  
  String _generateDisplayNameFromId(String userId) {
    // แปลง userId เป็น display name
    if (userId.isEmpty) return 'Anonymous User';
    
    // แยกคำและทำให้เป็น Title Case
    final words = userId.replaceAll(RegExp(r'[^a-zA-Z]'), ' ').split(' ');
    return words
        .where((word) => word.isNotEmpty)
        .map((word) => word[0].toUpperCase() + word.substring(1).toLowerCase())
        .join(' ');
  }
  
  String _getRandomAvatar() {
    final avatars = [
      'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=400',
      'https://images.unsplash.com/photo-1599566150163-29194dcaad36?w=400',
      'https://images.unsplash.com/photo-1527980965255-d3b416303d12?w=400',
      'https://images.unsplash.com/photo-1544723795-3fb6469f5b39?w=400',
      'https://images.unsplash.com/photo-1546961329-78bef0414d7c?w=400',
    ];
    return avatars[userId.hashCode.abs() % avatars.length];
  }
  
  int _getRandomNumber(int min, int max) {
    return min + (userId.hashCode.abs() % (max - min));
  }
  
  bool _checkIfFollowing(String userId) {
    // Mock following status - randomly return true/false
    return userId.hashCode % 2 == 0;
  }
  
  List<PostModel> _getMockUserPosts(String userId) {
    final user = _getMockUserData(userId);
    final userName = user.name;
    
    switch (userId) {
      case 'alice':
      case 'alicejohnson':
        return [
          PostModel(
            id: 101,
            author: userName,
            avatar: '',
            media: 'https://images.unsplash.com/photo-1506905925346-21bda4d32df4',
            caption: 'Beautiful landscape view from the mountains! 🏔️',
            likes: 45,
            isLiked: false,
            comments: [
              CommentModel(
                id: 1001,
                user: 'Bob Smith',
                text: 'Amazing shot! 📸',
                likes: 3,
              ),
              CommentModel(
                id: 1002,
                user: 'Carol Davis',
                text: 'Where was this taken?',
                likes: 1,
                replies: [
                  CommentModel(
                    id: 1003,
                    user: userName,
                    text: '@Carol Davis It was taken in Kyoto, Japan!',
                  ),
                ],
              ),
            ],
          ),
          PostModel(
            id: 102,
            author: userName,
            avatar: '',
            media: 'https://images.unsplash.com/photo-1501594907352-04cda38ebc29',
            caption: 'Working on some new design concepts today ✨',
            likes: 67,
            isLiked: true,
            comments: [
              CommentModel(
                id: 1004,
                user: 'David Wilson',
                text: 'Love your design work! 🎨',
                likes: 5,
              ),
            ],
          ),
        ];
        
      case 'bob':
      case 'bobsmith':
        return [
          PostModel(
            id: 201,
            author: userName,
            avatar: '',
            media: 'https://images.unsplash.com/photo-1461749280684-dccba630e2f6',
            caption: 'Late night coding session 💻 #developer',
            likes: 23,
            isLiked: false,
            comments: [
              CommentModel(
                id: 2001,
                user: 'Alice Johnson',
                text: 'Great code architecture! 💻',
                likes: 2,
              ),
            ],
          ),
        ];
        
      case 'carol':
      case 'caroldavis':
        return [
          PostModel(
            id: 301,
            author: userName,
            avatar: '',
            media: 'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2',
            caption: 'Leading the team to new achievements! 🚀 #leadership',
            likes: 89,
            isLiked: false,
            comments: [
              CommentModel(
                id: 3001,
                user: 'Bob Smith',
                text: 'Inspiring leadership! 👏',
                likes: 7,
              ),
              CommentModel(
                id: 3002,
                user: 'Alice Johnson',
                text: 'Can you share more about this project?',
                likes: 4,
              ),
            ],
          ),
          PostModel(
            id: 302,
            author: userName,
            avatar: '',
            media: 'https://images.unsplash.com/photo-1559136555-9303baea8ebd',
            caption: 'Project management success! 📊 #projectmanagement',
            likes: 156,
            isLiked: true,
            comments: [],
          ),
        ];
        
      case 'david':
      case 'davidwilson':
        return [
          PostModel(
            id: 401,
            author: userName,
            avatar: '',
            media: 'https://images.unsplash.com/photo-1551434678-e076c223a692',
            caption: 'Creative brainstorming session! 🎨 #creativity',
            likes: 78,
            isLiked: false,
            comments: [
              CommentModel(
                id: 4001,
                user: 'Alice Johnson',
                text: 'Impressive data visualization! 📊',
                likes: 8,
              ),
              CommentModel(
                id: 4002,
                user: 'Carol Davis',
                text: 'Can you share the dataset?',
                likes: 3,
              ),
            ],
          ),
        ];
        
      case 'emma':
      case 'emmabrown':
        return [
          PostModel(
            id: 501,
            author: userName,
            avatar: '',
            media: 'https://images.unsplash.com/photo-1581291518857-4e27b48ff24e',
            caption: 'Marketing strategy meeting 📈 #marketing',
            likes: 34,
            isLiked: true,
            comments: [
              CommentModel(
                id: 5001,
                user: 'Bob Smith',
                text: 'Nice Flutter app! 📱',
                likes: 4,
              ),
            ],
          ),
          PostModel(
            id: 502,
            author: userName,
            avatar: '',
            media: 'https://images.unsplash.com/photo-1559056199-641a0ac8b55e',
            caption: 'UI/UX design showcase 🎨 #design #ux',
            likes: 92,
            isLiked: false,
            comments: [
              CommentModel(
                id: 5002,
                user: 'David Wilson',
                text: 'Love the UI/UX design! 🎨',
                likes: 6,
              ),
              CommentModel(
                id: 5003,
                user: 'Alice Johnson',
                text: 'Which design system did you use?',
                likes: 2,
                replies: [
                  CommentModel(
                    id: 5004,
                    user: userName,
                    text: '@Alice Johnson Material Design 3 with custom theming!',
                  ),
                ],
              ),
            ],
          ),
        ];
        
      default:
        // สร้าง mock posts สำหรับ user อื่นๆ
        return [
          PostModel(
            id: 900 + userId.hashCode.abs() % 100,
            author: userName,
            avatar: '',
            media: 'https://images.unsplash.com/photo-1522202176988-66273c2fd55f',
            caption: 'Another great day! ☀️',
            likes: 15 + (userId.hashCode.abs() % 30),
            isLiked: userId.hashCode % 3 == 0,
            comments: [
              CommentModel(
                id: 9001 + userId.hashCode.abs() % 100,
                user: 'John Doe',
                text: 'Great post! 👍',
                likes: 1 + (userId.hashCode.abs() % 5),
              ),
            ],
          ),
        ];
    }
  }
  
  // ✅ Toggle like สำหรับ post (เรียกใช้จาก double tap)
  void toggleLike(int postId) {
    final postIndex = userPosts.indexWhere((post) => post.id == postId);
    if (postIndex != -1) {
      final post = userPosts[postIndex];
      post.isLiked = !post.isLiked;
      post.likes += post.isLiked ? 1 : -1;
      userPosts.refresh(); // Force UI rebuild
    }
  }
}

/// Model สำหรับข้อมูล User
class UserModel {
  final String id;
  final String name;
  final String bio;
  final String avatar;
  final int followersCount;
  final int followingCount;
  
  UserModel({
    required this.id,
    required this.name,
    required this.bio,
    required this.avatar,
    required this.followersCount,
    required this.followingCount,
  });
}