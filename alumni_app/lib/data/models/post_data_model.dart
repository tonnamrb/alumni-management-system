/// Data models for posts API responses
class PostDataModel {
  final int id;
  final int userId;
  final String userName;
  final String? userAvatar;
  final String content;
  final String type;
  final String? imageUrl;
  final List<String>? mediaUrls;
  final int mediaCount;
  final bool isPinned;
  final int likesCount;
  final int commentsCount;
  final bool isLikedByCurrentUser;
  final bool isOwnPost;
  final DateTime createdAt;
  final DateTime updatedAt;
  
  PostDataModel({
    required this.id,
    required this.userId,
    required this.userName,
    this.userAvatar,
    required this.content,
    required this.type,
    this.imageUrl,
    this.mediaUrls,
    required this.mediaCount,
    required this.isPinned,
    required this.likesCount,
    required this.commentsCount,
    required this.isLikedByCurrentUser,
    required this.isOwnPost,
    required this.createdAt,
    required this.updatedAt,
  });
  
  factory PostDataModel.fromJson(Map<String, dynamic> json) {
    return PostDataModel(
      id: json['id'] ?? 0,
      userId: json['userId'] ?? 0,
      userName: json['userName'] ?? '',
      userAvatar: json['userAvatar'],
      content: json['content'] ?? '',
      type: json['type'] ?? 'Text',
      imageUrl: json['imageUrl'],
      mediaUrls: json['mediaUrls']?.cast<String>(),
      mediaCount: json['mediaCount'] ?? 0,
      isPinned: json['isPinned'] ?? false,
      likesCount: json['likesCount'] ?? 0,
      commentsCount: json['commentsCount'] ?? 0,
      isLikedByCurrentUser: json['isLikedByCurrentUser'] ?? false,
      isOwnPost: json['isOwnPost'] ?? false,
      createdAt: DateTime.tryParse(json['createdAt'] ?? '') ?? DateTime.now(),
      updatedAt: DateTime.tryParse(json['updatedAt'] ?? '') ?? DateTime.now(),
    );
  }
  
  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'userId': userId,
      'userName': userName,
      'userAvatar': userAvatar,
      'content': content,
      'type': type,
      'imageUrl': imageUrl,
      'mediaUrls': mediaUrls,
      'mediaCount': mediaCount,
      'isPinned': isPinned,
      'likesCount': likesCount,
      'commentsCount': commentsCount,
      'isLikedByCurrentUser': isLikedByCurrentUser,
      'isOwnPost': isOwnPost,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
    };
  }
}

/// Posts list with pagination data
class PostListDataModel {
  final List<PostDataModel> posts;
  final int totalCount;
  final int page;
  final int pageSize;
  final bool hasNextPage;
  final bool hasPreviousPage;
  
  PostListDataModel({
    required this.posts,
    required this.totalCount,
    required this.page,
    required this.pageSize,
    required this.hasNextPage,
    required this.hasPreviousPage,
  });
  
  factory PostListDataModel.fromJson(Map<String, dynamic> json) {
    return PostListDataModel(
      posts: (json['posts'] as List<dynamic>?)
          ?.map((item) => PostDataModel.fromJson(item as Map<String, dynamic>))
          .toList() ?? [],
      totalCount: json['totalCount'] ?? 0,
      page: json['page'] ?? 1,
      pageSize: json['pageSize'] ?? 10,
      hasNextPage: json['hasNextPage'] ?? false,
      hasPreviousPage: json['hasPreviousPage'] ?? false,
    );
  }
  
  Map<String, dynamic> toJson() {
    return {
      'posts': posts.map((post) => post.toJson()).toList(),
      'totalCount': totalCount,
      'page': page,
      'pageSize': pageSize,
      'hasNextPage': hasNextPage,
      'hasPreviousPage': hasPreviousPage,
    };
  }
}

/// Like response model
class LikeResponseModel {
  final bool isLiked;
  final int likesCount;
  
  LikeResponseModel({
    required this.isLiked,
    required this.likesCount,
  });
  
  factory LikeResponseModel.fromJson(Map<String, dynamic> json) {
    return LikeResponseModel(
      isLiked: json['isLiked'] ?? false,
      likesCount: json['likesCount'] ?? 0,
    );
  }
  
  Map<String, dynamic> toJson() {
    return {
      'isLiked': isLiked,
      'likesCount': likesCount,
    };
  }
}

/// Post type enumeration
enum PostType {
  text('Text'),
  image('Image'),
  video('Video'),
  link('Link');
  
  const PostType(this.value);
  final String value;
  
  static PostType fromString(String value) {
    return PostType.values.firstWhere(
      (type) => type.value == value,
      orElse: () => PostType.text,
    );
  }
}