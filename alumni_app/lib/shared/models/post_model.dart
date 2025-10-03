/// Shared Post Model สำหรับใช้ทั้งแอป
class PostModel {
  final int id;
  final String author;
  final String avatar;
  final String media;
  final String caption;
  int likes;
  bool isLiked;
  final List<CommentModel> comments;
  final DateTime createdAt;
  
  // Admin features
  bool isPinned;
  bool isReported;
  int reportCount;
  
  PostModel({
    required this.id,
    required this.author,
    required this.avatar,
    required this.media,
    required this.caption,
    required this.likes,
    required this.isLiked,
    required this.comments,
    DateTime? createdAt,
    this.isPinned = false,
    this.isReported = false,
    this.reportCount = 0,
  }) : createdAt = createdAt ?? DateTime.now();
}

/// Shared Comment Model สำหรับใช้ทั้งแอป
class CommentModel {
  final int id;
  final String user;
  final String text;
  int likes;
  bool isLiked;
  final List<CommentModel> replies;
  final DateTime createdAt;
  
  CommentModel({
    required this.id,
    required this.user,
    required this.text,
    this.likes = 0,
    this.isLiked = false,
    List<CommentModel>? replies,
    DateTime? createdAt,
  }) : replies = replies ?? [],
       createdAt = createdAt ?? DateTime.now();
}