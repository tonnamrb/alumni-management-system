class CreatePostRequest {
  final String title;
  final String content;
  final String? imageUrl;
  final List<String>? tags;
  
  CreatePostRequest({
    required this.title,
    required this.content,
    this.imageUrl,
    this.tags,
  });

  Map<String, dynamic> toJson() {
    return {
      'title': title,
      'content': content,
      if (imageUrl != null) 'imageUrl': imageUrl,
      if (tags != null) 'tags': tags,
    };
  }
}