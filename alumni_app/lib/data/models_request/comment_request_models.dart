/// Request models for Comments API

class CreateCommentRequestModel {
  final int postId;
  final String content;
  final int? parentCommentId;

  CreateCommentRequestModel({
    required this.postId,
    required this.content,
    this.parentCommentId,
  });

  Map<String, dynamic> toJson() {
    return {
      'postId': postId,
      'content': content,
      if (parentCommentId != null) 'parentCommentId': parentCommentId,
    };
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is CreateCommentRequestModel &&
          runtimeType == other.runtimeType &&
          postId == other.postId &&
          content == other.content &&
          parentCommentId == other.parentCommentId;

  @override
  int get hashCode => postId.hashCode ^ content.hashCode ^ parentCommentId.hashCode;
}

class UpdateCommentRequestModel {
  final String content;

  UpdateCommentRequestModel({
    required this.content,
  });

  Map<String, dynamic> toJson() {
    return {
      'content': content,
    };
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is UpdateCommentRequestModel &&
          runtimeType == other.runtimeType &&
          content == other.content;

  @override
  int get hashCode => content.hashCode;
}

class CommentFilterParams {
  final int? postId;
  final int? authorId;
  final DateTime? fromDate;
  final DateTime? toDate;
  final int page;
  final int pageSize;
  final String? sortBy;
  final String? sortDirection;

  CommentFilterParams({
    this.postId,
    this.authorId,
    this.fromDate,
    this.toDate,
    this.page = 1,
    this.pageSize = 10,
    this.sortBy,
    this.sortDirection = 'desc',
  });

  Map<String, dynamic> toQueryParams() {
    final params = <String, dynamic>{
      'page': page,
      'pageSize': pageSize,
    };

    if (postId != null) params['postId'] = postId;
    if (authorId != null) params['authorId'] = authorId;
    if (fromDate != null) params['fromDate'] = fromDate!.toIso8601String();
    if (toDate != null) params['toDate'] = toDate!.toIso8601String();
    if (sortBy != null) params['sortBy'] = sortBy;
    if (sortDirection != null) params['sortDirection'] = sortDirection;

    return params;
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is CommentFilterParams &&
          runtimeType == other.runtimeType &&
          postId == other.postId &&
          authorId == other.authorId &&
          fromDate == other.fromDate &&
          toDate == other.toDate &&
          page == other.page &&
          pageSize == other.pageSize &&
          sortBy == other.sortBy &&
          sortDirection == other.sortDirection;

  @override
  int get hashCode =>
      postId.hashCode ^
      authorId.hashCode ^
      fromDate.hashCode ^
      toDate.hashCode ^
      page.hashCode ^
      pageSize.hashCode ^
      sortBy.hashCode ^
      sortDirection.hashCode;
}