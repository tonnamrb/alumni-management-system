/// Post entity for domain layer
class Post {
  final int id;
  final String title;
  final String content;
  final int authorId;
  final String authorName;
  final String? authorProfileImageUrl;
  final String postType;
  final int? categoryId;
  final String? categoryName;
  final List<String> tags;
  final List<String> attachments;
  final int likesCount;
  final int commentsCount;
  final bool isLiked;
  final bool isAnonymous;
  final DateTime createdAt;
  final DateTime updatedAt;

  Post({
    required this.id,
    required this.title,
    required this.content,
    required this.authorId,
    required this.authorName,
    this.authorProfileImageUrl,
    required this.postType,
    this.categoryId,
    this.categoryName,
    required this.tags,
    required this.attachments,
    required this.likesCount,
    required this.commentsCount,
    required this.isLiked,
    required this.isAnonymous,
    required this.createdAt,
    required this.updatedAt,
  });

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is Post &&
          runtimeType == other.runtimeType &&
          id == other.id;

  @override
  int get hashCode => id.hashCode;

  Post copyWith({
    int? id,
    String? title,
    String? content,
    int? authorId,
    String? authorName,
    String? authorProfileImageUrl,
    String? postType,
    int? categoryId,
    String? categoryName,
    List<String>? tags,
    List<String>? attachments,
    int? likesCount,
    int? commentsCount,
    bool? isLiked,
    bool? isAnonymous,
    DateTime? createdAt,
    DateTime? updatedAt,
  }) {
    return Post(
      id: id ?? this.id,
      title: title ?? this.title,
      content: content ?? this.content,
      authorId: authorId ?? this.authorId,
      authorName: authorName ?? this.authorName,
      authorProfileImageUrl: authorProfileImageUrl ?? this.authorProfileImageUrl,
      postType: postType ?? this.postType,
      categoryId: categoryId ?? this.categoryId,
      categoryName: categoryName ?? this.categoryName,
      tags: tags ?? this.tags,
      attachments: attachments ?? this.attachments,
      likesCount: likesCount ?? this.likesCount,
      commentsCount: commentsCount ?? this.commentsCount,
      isLiked: isLiked ?? this.isLiked,
      isAnonymous: isAnonymous ?? this.isAnonymous,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }
}