/// Data models for Comments API responses

class CommentDataModel {
  final int id;
  final int postId;
  final int? parentCommentId;
  final int authorId;
  final String authorName;
  final String? authorProfileImageUrl;
  final String content;
  final DateTime createdAt;
  final DateTime updatedAt;
  final List<CommentDataModel>? replies;
  final int replyCount;
  final bool isEdited;

  CommentDataModel({
    required this.id,
    required this.postId,
    this.parentCommentId,
    required this.authorId,
    required this.authorName,
    this.authorProfileImageUrl,
    required this.content,
    required this.createdAt,
    required this.updatedAt,
    this.replies,
    required this.replyCount,
    required this.isEdited,
  });

  factory CommentDataModel.fromJson(Map<String, dynamic> json) {
    return CommentDataModel(
      id: json['id'] as int,
      postId: json['postId'] as int,
      parentCommentId: json['parentCommentId'] as int?,
      authorId: json['authorId'] as int,
      authorName: json['authorName'] as String,
      authorProfileImageUrl: json['authorProfileImageUrl'] as String?,
      content: json['content'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      updatedAt: DateTime.parse(json['updatedAt'] as String),
      replies: json['replies'] != null 
          ? (json['replies'] as List<dynamic>)
              .map((reply) => CommentDataModel.fromJson(reply as Map<String, dynamic>))
              .toList()
          : null,
      replyCount: json['replyCount'] as int? ?? 0,
      isEdited: json['isEdited'] as bool? ?? false,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'postId': postId,
      'parentCommentId': parentCommentId,
      'authorId': authorId,
      'authorName': authorName,
      'authorProfileImageUrl': authorProfileImageUrl,
      'content': content,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
      'replies': replies?.map((reply) => reply.toJson()).toList(),
      'replyCount': replyCount,
      'isEdited': isEdited,
    };
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is CommentDataModel &&
          runtimeType == other.runtimeType &&
          id == other.id;

  @override
  int get hashCode => id.hashCode;

  CommentDataModel copyWith({
    int? id,
    int? postId,
    int? parentCommentId,
    int? authorId,
    String? authorName,
    String? authorProfileImageUrl,
    String? content,
    DateTime? createdAt,
    DateTime? updatedAt,
    List<CommentDataModel>? replies,
    int? replyCount,
    bool? isEdited,
  }) {
    return CommentDataModel(
      id: id ?? this.id,
      postId: postId ?? this.postId,
      parentCommentId: parentCommentId ?? this.parentCommentId,
      authorId: authorId ?? this.authorId,
      authorName: authorName ?? this.authorName,
      authorProfileImageUrl: authorProfileImageUrl ?? this.authorProfileImageUrl,
      content: content ?? this.content,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
      replies: replies ?? this.replies,
      replyCount: replyCount ?? this.replyCount,
      isEdited: isEdited ?? this.isEdited,
    );
  }
}

/// Model for paginated comment lists
class CommentListDataModel {
  final List<CommentDataModel> comments;
  final int totalCount;
  final int totalPages;
  final int currentPage;
  final int pageSize;
  final bool hasNextPage;
  final bool hasPreviousPage;

  CommentListDataModel({
    required this.comments,
    required this.totalCount,
    required this.totalPages,
    required this.currentPage,
    required this.pageSize,
    required this.hasNextPage,
    required this.hasPreviousPage,
  });

  factory CommentListDataModel.fromJson(Map<String, dynamic> json) {
    return CommentListDataModel(
      comments: (json['items'] as List<dynamic>?)
              ?.map((item) => CommentDataModel.fromJson(item as Map<String, dynamic>))
              .toList() 
          ?? [],
      totalCount: json['totalCount'] as int? ?? 0,
      totalPages: json['totalPages'] as int? ?? 0,
      currentPage: json['currentPage'] as int? ?? 1,
      pageSize: json['pageSize'] as int? ?? 10,
      hasNextPage: json['hasNextPage'] as bool? ?? false,
      hasPreviousPage: json['hasPreviousPage'] as bool? ?? false,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'items': comments.map((comment) => comment.toJson()).toList(),
      'totalCount': totalCount,
      'totalPages': totalPages,
      'currentPage': currentPage,
      'pageSize': pageSize,
      'hasNextPage': hasNextPage,
      'hasPreviousPage': hasPreviousPage,
    };
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is CommentListDataModel &&
          runtimeType == other.runtimeType &&
          comments == other.comments &&
          currentPage == other.currentPage;

  @override
  int get hashCode => comments.hashCode ^ currentPage.hashCode;

  CommentListDataModel copyWith({
    List<CommentDataModel>? comments,
    int? totalCount,
    int? totalPages,
    int? currentPage,
    int? pageSize,
    bool? hasNextPage,
    bool? hasPreviousPage,
  }) {
    return CommentListDataModel(
      comments: comments ?? this.comments,
      totalCount: totalCount ?? this.totalCount,
      totalPages: totalPages ?? this.totalPages,
      currentPage: currentPage ?? this.currentPage,
      pageSize: pageSize ?? this.pageSize,
      hasNextPage: hasNextPage ?? this.hasNextPage,
      hasPreviousPage: hasPreviousPage ?? this.hasPreviousPage,
    );
  }
}