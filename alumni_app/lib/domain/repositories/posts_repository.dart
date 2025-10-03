import '../../core/result/result.dart';
import '../entities/post.dart';

/// Posts repository interface for domain layer
abstract class PostsRepository {
  Future<Result<List<Post>>> getPosts({
    int page = 1,
    int pageSize = 10,
  });

  Future<Result<Post>> getPost(int postId);

  Future<Result<Post>> createPost(CreatePostRequest request);

  Future<Result<Post>> updatePost(int postId, UpdatePostRequest request);

  Future<Result<bool>> deletePost(int postId);

  Future<Result<bool>> likePost(int postId);

  Future<Result<bool>> unlikePost(int postId);

  Future<Result<List<Post>>> searchPosts(String query, {
    int page = 1,
    int pageSize = 10,
  });
}

/// Request classes for domain layer
class CreatePostRequest {
  final String title;
  final String content;
  final String postType;
  final int? categoryId;
  final List<String> tags;
  final List<String> attachments;
  final bool isAnonymous;

  CreatePostRequest({
    required this.title,
    required this.content,
    required this.postType,
    this.categoryId,
    required this.tags,
    required this.attachments,
    required this.isAnonymous,
  });
}

class UpdatePostRequest {
  final String? title;
  final String? content;
  final int? categoryId;
  final List<String>? tags;
  final List<String>? attachments;

  UpdatePostRequest({
    this.title,
    this.content,
    this.categoryId,
    this.tags,
    this.attachments,
  });
}