import 'package:get/get.dart';
import 'package:alumni_app/core/services/user_session_service.dart';
import 'package:alumni_app/shared/models/post_model.dart';

class FeedController extends GetxController {
  final RxList<PostModel> posts = <PostModel>[].obs;
  final RxBool isLoading = false.obs;
  
  // ✅ เพิ่ม UserSessionService
  final UserSessionService _userSessionService = Get.find<UserSessionService>();
  
  @override
  void onInit() {
    super.onInit();
    _loadMockPosts();
  }
  
  void _loadMockPosts() {
    isLoading.value = true;
    
    // Mock data with clickable users for testing user profiles
    final mockPosts = [
      PostModel(
        id: 1,
        author: "Alice Johnson",
        avatar: "",
        media: "https://images.unsplash.com/photo-1759239572496-4ec13e7643d6?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
        caption: "Beautiful day for photography! 📸",
        likes: 42,
        isLiked: false,
        comments: [
          CommentModel(
            id: 1001, 
            user: "Bob Smith", 
            text: "Great post! Love the colors 🎨", 
            likes: 3, 
            isLiked: true,
            replies: [
              CommentModel(id: 1004, user: "Alice Johnson", text: "Thank you! 😊"),
            ],
          ),
          CommentModel(id: 1002, user: "Carol Davis", text: "Amazing work as always!", likes: 1)
        ],
      ),
      PostModel(
        id: 2,
        author: "Bob Smith",
        avatar: "",
        media: "",
        caption: "Just finished this amazing project! 🚀",
        likes: 28,
        isLiked: true,
        comments: [
          CommentModel(id: 1003, user: "David Wilson", text: "This looks incredible! 🔥", likes: 2),
        ],
      ),
      PostModel(
        id: 3,
        author: "Carol Davis",
        avatar: "",
        media: "",
        caption: "Leadership thoughts for today 💭 #leadership",
        likes: 15,
        isLiked: false,
        comments: [
          CommentModel(id: 1005, user: "Emma Brown", text: "Great product insight! 💡", likes: 4),
        ],
      ),
      PostModel(
        id: 4,
        author: "David Wilson",
        avatar: "",
        media: "",
        caption: "Data speaks louder than words 📊 #analytics",
        likes: 67,
        isLiked: false,
        comments: [
          CommentModel(id: 1006, user: "Alice Johnson", text: "Awesome data visualization! 📊", likes: 5),
        ],
      ),
      PostModel(
        id: 5,
        author: "Emma Brown",
        avatar: "",
        media: "",
        caption: "Marketing strategies that actually work! 🎯 #marketing",
        likes: 34,
        isLiked: true,
        comments: [
          CommentModel(
            id: 1007, 
            user: "Bob Smith", 
            text: "Nice Flutter app! 📱", 
            likes: 4,
            replies: [
              CommentModel(id: 1008, user: "Emma Brown", text: "@Bob Smith Thanks! Used Material 3 💙"),
            ],
          ),
        ],
      ),
    ];
    
    posts.assignAll(mockPosts);
    isLoading.value = false;
  }
  
  void toggleLike(int postId) {
    final postIndex = posts.indexWhere((post) => post.id == postId);
    if (postIndex != -1) {
      final post = posts[postIndex];
      post.isLiked = !post.isLiked;
      post.likes += post.isLiked ? 1 : -1;
      posts[postIndex] = post;
      posts.refresh(); // Notify observers
    }
  }
  
  void addComment(int postId, String comment) {
    if (comment.trim().isEmpty) return;
    
    final postIndex = posts.indexWhere((post) => post.id == postId);
    if (postIndex != -1) {
      final post = posts[postIndex];
      final newId = DateTime.now().millisecondsSinceEpoch;
      post.comments.add(CommentModel(
        id: newId,
        user: _userSessionService.currentUserName, // ✅ ใช้ชื่อจริงของ user
        text: comment,
      ));
      posts[postIndex] = post;
      posts.refresh(); // Notify observers
    }
  }

  // ✅ เพิ่มฟังก์ชัน like comment สำหรับ Feed
  void toggleCommentLike(int postId, int commentId) {
    final postIndex = posts.indexWhere((post) => post.id == postId);
    if (postIndex != -1) {
      final post = posts[postIndex];
      final commentIndex = post.comments.indexWhere((comment) => comment.id == commentId);
      if (commentIndex != -1) {
        final comment = post.comments[commentIndex];
        comment.isLiked = !comment.isLiked;
        comment.likes += comment.isLiked ? 1 : -1;
        posts.refresh();
      }
    }
  }

  // ✅ เพิ่มฟังก์ชัน reply comment สำหรับ Feed
  void addCommentReply(int postId, int parentCommentId, String replyText) {
    if (replyText.trim().isEmpty) return;
    
    final postIndex = posts.indexWhere((post) => post.id == postId);
    if (postIndex != -1) {
      final post = posts[postIndex];
      final commentIndex = post.comments.indexWhere((comment) => comment.id == parentCommentId);
      if (commentIndex != -1) {
        final parentComment = post.comments[commentIndex];
        final newId = DateTime.now().millisecondsSinceEpoch + 1;
        parentComment.replies.add(CommentModel(
          id: newId,
          user: _userSessionService.currentUserName, // ✅ ใช้ชื่อจริงของ user
          text: replyText,
        ));
        posts.refresh();
      }
    }
  }

  // ✅ เพิ่มฟังก์ชัน like reply สำหรับ Feed
  void toggleReplyLike(int postId, int parentCommentId, int replyId) {
    final postIndex = posts.indexWhere((post) => post.id == postId);
    if (postIndex != -1) {
      final post = posts[postIndex];
      final commentIndex = post.comments.indexWhere((comment) => comment.id == parentCommentId);
      if (commentIndex != -1) {
        final parentComment = post.comments[commentIndex];
        final replyIndex = parentComment.replies.indexWhere((reply) => reply.id == replyId);
        if (replyIndex != -1) {
          final reply = parentComment.replies[replyIndex];
          reply.isLiked = !reply.isLiked;
          reply.likes += reply.isLiked ? 1 : -1;
          posts.refresh();
        }
      }
    }
  }
  
  void uploadPost() {
    // Mock upload functionality
    Get.snackbar(
      'Upload',
      'Upload feature coming soon!',
      snackPosition: SnackPosition.BOTTOM,
    );
  }
  
  // ✅ เพิ่ม method เพื่อให้ UI เข้าถึงชื่อ user ปัจจุบัน
  String getCurrentUserName() {
    return _userSessionService.currentUserName;
  }
}