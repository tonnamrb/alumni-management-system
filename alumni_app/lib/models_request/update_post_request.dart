class UpdatePostRequest {
  final String? title;
  final String? content;
  final String? imageUrl;
  final List<String>? tags;
  
  UpdatePostRequest({
    this.title,
    this.content,
    this.imageUrl,
    this.tags,
  });

  Map<String, dynamic> toJson() {
    return {
      if (title != null) 'title': title,
      if (content != null) 'content': content,
      if (imageUrl != null) 'imageUrl': imageUrl,
      if (tags != null) 'tags': tags,
    };
  }
}