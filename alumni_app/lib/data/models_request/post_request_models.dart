import '../../../models_request/create_post_request.dart';
import '../../../models_request/update_post_request.dart';

/// Request models for posts API
class CreatePostRequestModel {
  final String content;
  final String type;
  final String? imageUrl;
  final List<String>? mediaUrls;
  final bool isPinned;
  
  CreatePostRequestModel({
    required this.content,
    required this.type,
    this.imageUrl,
    this.mediaUrls,
    this.isPinned = false,
  });
  
  Map<String, dynamic> toJson() {
    return {
      'content': content,
      'type': type,
      if (imageUrl != null) 'imageUrl': imageUrl,
      if (mediaUrls != null) 'mediaUrls': mediaUrls,
      'isPinned': isPinned,
    };
  }
}

/// Update post request model
class UpdatePostRequestModel {
  final String? content;
  final String? type;
  final String? imageUrl;
  final List<String>? mediaUrls;
  final bool? isPinned;
  
  UpdatePostRequestModel({
    this.content,
    this.type,
    this.imageUrl,
    this.mediaUrls,
    this.isPinned,
  });
  
  Map<String, dynamic> toJson() {
    final map = <String, dynamic>{};
    
    if (content != null) map['content'] = content;
    if (type != null) map['type'] = type;
    if (imageUrl != null) map['imageUrl'] = imageUrl;
    if (mediaUrls != null) map['mediaUrls'] = mediaUrls;
    if (isPinned != null) map['isPinned'] = isPinned;
    
    return map;
  }
}

/// Filter parameters for getting posts
class PostFilterParams {
  final int page;
  final int pageSize;
  final String? type;
  final int? userId;
  final bool? isPinned;
  
  PostFilterParams({
    this.page = 1,
    this.pageSize = 10,
    this.type,
    this.userId,
    this.isPinned,
  });
  
  Map<String, dynamic> toQueryParams() {
    final params = <String, dynamic>{
      'page': page,
      'pageSize': pageSize,
    };
    
    if (type != null) params['type'] = type;
    if (userId != null) params['userId'] = userId;
    if (isPinned != null) params['isPinned'] = isPinned;
    
    return params;
  }
}