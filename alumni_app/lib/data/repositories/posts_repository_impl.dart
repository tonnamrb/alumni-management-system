import '../../core/result/result.dart';
import '../../domain/entities/post.dart';
import '../../domain/repositories/posts_repository.dart';
import '../datasources/remote/posts_api.dart';
import '../../models_request/create_post_request.dart' as api_models;
import '../../models_request/update_post_request.dart' as api_models;

/// Repository implementation for Posts
class PostsRepositoryImpl implements PostsRepository {
  final PostsApi _postsApi;

  PostsRepositoryImpl({required PostsApi postsApi}) : _postsApi = postsApi;

  @override
  Future<Result<List<Post>>> getPosts({
    int page = 1,
    int pageSize = 10,
  }) async {
    final result = await _postsApi.getPosts(page: page, pageSize: pageSize);
    return result.when(
      success: (data) => Result.success(data.posts),
      failure: (message, code) => Result.failure('Failed to get posts: $message'),
    );
  }

  @override
  Future<Result<Post>> getPost(int postId) async {
    final result = await _postsApi.getPost(postId);
    return result.when(
      success: (data) => Result.success(data),
      failure: (message, code) => Result.failure('Failed to get post: $message'),
    );
  }

  @override
  Future<Result<Post>> createPost(CreatePostRequest request) async {
    // Convert to API request format
    final apiRequest = api_models.CreatePostRequest(
      title: request.title,
      content: request.content,
      imageUrl: null, // No image in domain request
      tags: request.tags,
    );

    final result = await _postsApi.createPost(apiRequest);
    return result.when(
      success: (data) => Result.success(data),
      failure: (message, code) => Result.failure('Failed to create post: $message'),
    );
  }

  @override
  Future<Result<Post>> updatePost(int postId, UpdatePostRequest request) async {
    final apiRequest = api_models.UpdatePostRequest(
      title: request.title,
      content: request.content,
      tags: request.tags,
    );

    final result = await _postsApi.updatePost(postId, apiRequest);
    return result.when(
      success: (data) => Result.success(data),
      failure: (message, code) => Result.failure('Failed to update post: $message'),
    );
  }

  @override
  Future<Result<bool>> deletePost(int postId) async {
    final result = await _postsApi.deletePost(postId);
    return result.when(
      success: (_) => Result.success(true),
      failure: (message, code) => Result.failure('Failed to delete post: $message'),
    );
  }

  @override
  Future<Result<bool>> likePost(int postId) async {
    final result = await _postsApi.likePost(postId);
    return result.when(
      success: (_) => Result.success(true),
      failure: (message, code) => Result.failure('Failed to like post: $message'),
    );
  }

  @override
  Future<Result<bool>> unlikePost(int postId) async {
    final result = await _postsApi.unlikePost(postId);
    return result.when(
      success: (_) => Result.success(true),
      failure: (message, code) => Result.failure('Failed to unlike post: $message'),
    );
  }

  @override
  Future<Result<List<Post>>> searchPosts(String query, {
    int page = 1, 
    int pageSize = 10,
  }) async {
    // Use API search functionality
    final result = await _postsApi.searchPosts(query, page: page, pageSize: pageSize);
    return result.when(
      success: (data) {
        final filtered = data.posts
            .where((post) => post.content.toLowerCase().contains(query.toLowerCase()))
            .toList();
        return Result.success(filtered);
      },
      failure: (message, code) => Result.failure('Failed to search posts: $message'),
    );
  }
}