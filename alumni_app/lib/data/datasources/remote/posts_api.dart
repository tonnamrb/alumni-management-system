import 'package:dio/dio.dart';
import '../../../core/result/result.dart';
import '../../../domain/entities/post.dart';
import '../../../models_request/create_post_request.dart';
import '../../../models_request/update_post_request.dart';

class PostsApi {
  final Dio _dio;
  
  PostsApi(this._dio);

  Future<Result<PostsResponse>> getPosts({
    int page = 1,
    int limit = 10,
    int? pageSize,
  }) async {
    try {
      final actualPageSize = pageSize ?? limit;
      final response = await _dio.get(
        '/api/v1/posts',
        queryParameters: {
          'page': page,
          'pageSize': actualPageSize,
        },
      );
      
      if (response.statusCode == 200) {
        final data = response.data['data'] as Map<String, dynamic>;
        final posts = (data['posts'] as List)
            .map((json) => _mapFromJson(json as Map<String, dynamic>))
            .toList();
        
        return Result.success(PostsResponse(
          posts: posts,
          totalCount: data['totalCount'] ?? 0,
          page: data['page'] ?? page,
          pageSize: data['pageSize'] ?? actualPageSize,
          hasNextPage: data['hasNextPage'] ?? false,
          hasPreviousPage: data['hasPreviousPage'] ?? false,
        ));
      } else {
        return Result.failure('Failed to fetch posts: ${response.statusCode}');
      }
    } catch (e) {
      return Result.failure('Error fetching posts: $e');
    }
  }

  Future<Result<Post>> getPost(int postId) async {
    try {
      final response = await _dio.get('/api/v1/posts/$postId');
      
      if (response.statusCode == 200) {
        final data = response.data['data'] as Map<String, dynamic>;
        return Result.success(_mapFromJson(data));
      } else {
        return Result.failure('Failed to fetch post: ${response.statusCode}');
      }
    } catch (e) {
      return Result.failure('Error fetching post: $e');
    }
  }

  Future<Result<Post>> createPost(CreatePostRequest request) async {
    try {
      final response = await _dio.post(
        '/api/v1/posts',
        data: request.toJson(),
      );
      
      if (response.statusCode == 201 || response.statusCode == 200) {
        final data = response.data['data'] as Map<String, dynamic>;
        return Result.success(_mapFromJson(data));
      } else {
        return Result.failure('Failed to create post: ${response.statusCode}');
      }
    } catch (e) {
      return Result.failure('Error creating post: $e');
    }
  }

  Future<Result<Post>> updatePost(int postId, UpdatePostRequest request) async {
    try {
      final response = await _dio.put(
        '/api/v1/posts/$postId',
        data: request.toJson(),
      );
      
      if (response.statusCode == 200) {
        final data = response.data['data'] as Map<String, dynamic>;
        return Result.success(_mapFromJson(data));
      } else {
        return Result.failure('Failed to update post: ${response.statusCode}');
      }
    } catch (e) {
      return Result.failure('Error updating post: $e');
    }
  }

  Future<Result<bool>> deletePost(int postId) async {
    try {
      final response = await _dio.delete('/api/v1/posts/$postId');
      
      if (response.statusCode == 200 || response.statusCode == 204) {
        return Result.success(true);
      } else {
        return Result.failure('Failed to delete post: ${response.statusCode}');
      }
    } catch (e) {
      return Result.failure('Error deleting post: $e');
    }
  }

  Future<Result<bool>> likePost(int postId) async {
    try {
      final response = await _dio.post('/api/v1/posts/$postId/like');
      
      if (response.statusCode == 200) {
        return Result.success(true);
      } else {
        return Result.failure('Failed to like post: ${response.statusCode}');
      }
    } catch (e) {
      return Result.failure('Error liking post: $e');
    }
  }

  Future<Result<bool>> unlikePost(int postId) async {
    try {
      final response = await _dio.delete('/api/v1/posts/$postId/like');
      
      if (response.statusCode == 200) {
        return Result.success(true);
      } else {
        return Result.failure('Failed to unlike post: ${response.statusCode}');
      }
    } catch (e) {
      return Result.failure('Error unliking post: $e');
    }
  }

  Future<Result<PostsResponse>> searchPosts(String query, {
    int page = 1,
    int limit = 10,
    int? pageSize,
  }) async {
    try {
      final actualPageSize = pageSize ?? limit;
      final response = await _dio.get(
        '/api/v1/posts',
        queryParameters: {
          'page': page,
          'pageSize': actualPageSize,
          // Add search functionality when backend supports it
        },
      );
      
      if (response.statusCode == 200) {
        final data = response.data['data'] as Map<String, dynamic>;
        final posts = (data['posts'] as List)
            .map((json) => _mapFromJson(json as Map<String, dynamic>))
            .toList();
        
        return Result.success(PostsResponse(
          posts: posts,
          totalCount: data['totalCount'] ?? 0,
          page: data['page'] ?? page,
          pageSize: data['pageSize'] ?? actualPageSize,
          hasNextPage: data['hasNextPage'] ?? false,
          hasPreviousPage: data['hasPreviousPage'] ?? false,
        ));
      } else {
        return Result.failure('Failed to search posts: ${response.statusCode}');
      }
    } catch (e) {
      return Result.failure('Error searching posts: $e');
    }
  }

  // Map JSON from API to Post entity
  Post _mapFromJson(Map<String, dynamic> json) {
    return Post(
      id: json['id'] ?? 0,
      title: json['content'] ?? '', // Use content as title for now
      content: json['content'] ?? '',
      authorId: json['userId'] ?? 0,
      authorName: json['userName'] ?? '',
      authorProfileImageUrl: json['userAvatar'],
      postType: json['type']?.toString() ?? 'Text',
      categoryId: null,
      categoryName: null,
      tags: [], // Tags not available in current API
      attachments: json['mediaUrls'] != null 
          ? List<String>.from(json['mediaUrls'])
          : [],
      likesCount: json['likesCount'] ?? 0,
      commentsCount: json['commentsCount'] ?? 0,
      isLiked: json['isLikedByCurrentUser'] ?? false,
      isAnonymous: false, // Not available in current API
      createdAt: json['createdAt'] != null 
          ? DateTime.parse(json['createdAt'])
          : DateTime.now(),
      updatedAt: json['updatedAt'] != null 
          ? DateTime.parse(json['updatedAt'])
          : DateTime.now(),
    );
  }
}

class PostsResponse {
  final List<Post> posts;
  final int totalCount;
  final int page;
  final int pageSize;
  final bool hasNextPage;
  final bool hasPreviousPage;
  
  PostsResponse({
    required this.posts,
    this.totalCount = 0,
    this.page = 1,
    this.pageSize = 10,
    this.hasNextPage = false,
    this.hasPreviousPage = false,
  });
}