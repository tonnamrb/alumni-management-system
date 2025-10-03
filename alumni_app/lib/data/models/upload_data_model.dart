/// Data models for Upload API responses

enum FileType { 
  image, 
  video, 
  document, 
  audio, 
  archive, 
  other 
}

enum UploadStatus { 
  pending, 
  processing, 
  completed, 
  failed, 
  deleted 
}

class UploadDataModel {
  final int id;
  final String fileName;
  final String originalFileName;
  final String fileExtension;
  final String mimeType;
  final int fileSize;
  final FileType fileType;
  final UploadStatus status;
  final String url;
  final String? thumbnailUrl;
  final String? category;
  final String? description;
  final int uploadedByUserId;
  final String uploadedByUserName;
  final DateTime createdAt;
  final DateTime updatedAt;
  final Map<String, dynamic>? metadata;
  final bool isPublic;
  final int downloadCount;
  final DateTime? expiresAt;

  UploadDataModel({
    required this.id,
    required this.fileName,
    required this.originalFileName,
    required this.fileExtension,
    required this.mimeType,
    required this.fileSize,
    required this.fileType,
    required this.status,
    required this.url,
    this.thumbnailUrl,
    this.category,
    this.description,
    required this.uploadedByUserId,
    required this.uploadedByUserName,
    required this.createdAt,
    required this.updatedAt,
    this.metadata,
    required this.isPublic,
    required this.downloadCount,
    this.expiresAt,
  });

  factory UploadDataModel.fromJson(Map<String, dynamic> json) {
    return UploadDataModel(
      id: json['id'] as int,
      fileName: json['fileName'] as String,
      originalFileName: json['originalFileName'] as String,
      fileExtension: json['fileExtension'] as String,
      mimeType: json['mimeType'] as String,
      fileSize: json['fileSize'] as int,
      fileType: FileType.values.firstWhere(
        (e) => e.name == json['fileType'],
        orElse: () => FileType.other,
      ),
      status: UploadStatus.values.firstWhere(
        (e) => e.name == json['status'],
        orElse: () => UploadStatus.pending,
      ),
      url: json['url'] as String,
      thumbnailUrl: json['thumbnailUrl'] as String?,
      category: json['category'] as String?,
      description: json['description'] as String?,
      uploadedByUserId: json['uploadedByUserId'] as int,
      uploadedByUserName: json['uploadedByUserName'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      updatedAt: DateTime.parse(json['updatedAt'] as String),
      metadata: json['metadata'] as Map<String, dynamic>?,
      isPublic: json['isPublic'] as bool? ?? false,
      downloadCount: json['downloadCount'] as int? ?? 0,
      expiresAt: json['expiresAt'] != null 
          ? DateTime.parse(json['expiresAt'] as String)
          : null,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'fileName': fileName,
      'originalFileName': originalFileName,
      'fileExtension': fileExtension,
      'mimeType': mimeType,
      'fileSize': fileSize,
      'fileType': fileType.name,
      'status': status.name,
      'url': url,
      'thumbnailUrl': thumbnailUrl,
      'category': category,
      'description': description,
      'uploadedByUserId': uploadedByUserId,
      'uploadedByUserName': uploadedByUserName,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
      'metadata': metadata,
      'isPublic': isPublic,
      'downloadCount': downloadCount,
      'expiresAt': expiresAt?.toIso8601String(),
    };
  }

  String get fileSizeFormatted {
    if (fileSize < 1024) return '$fileSize B';
    if (fileSize < 1024 * 1024) return '${(fileSize / 1024).toStringAsFixed(1)} KB';
    if (fileSize < 1024 * 1024 * 1024) return '${(fileSize / (1024 * 1024)).toStringAsFixed(1)} MB';
    return '${(fileSize / (1024 * 1024 * 1024)).toStringAsFixed(1)} GB';
  }

  bool get isImage => fileType == FileType.image;
  bool get isVideo => fileType == FileType.video;
  bool get isDocument => fileType == FileType.document;
  bool get isExpired => expiresAt != null && DateTime.now().isAfter(expiresAt!);

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is UploadDataModel &&
          runtimeType == other.runtimeType &&
          id == other.id;

  @override
  int get hashCode => id.hashCode;

  UploadDataModel copyWith({
    int? id,
    String? fileName,
    String? originalFileName,
    String? fileExtension,
    String? mimeType,
    int? fileSize,
    FileType? fileType,
    UploadStatus? status,
    String? url,
    String? thumbnailUrl,
    String? category,
    String? description,
    int? uploadedByUserId,
    String? uploadedByUserName,
    DateTime? createdAt,
    DateTime? updatedAt,
    Map<String, dynamic>? metadata,
    bool? isPublic,
    int? downloadCount,
    DateTime? expiresAt,
  }) {
    return UploadDataModel(
      id: id ?? this.id,
      fileName: fileName ?? this.fileName,
      originalFileName: originalFileName ?? this.originalFileName,
      fileExtension: fileExtension ?? this.fileExtension,
      mimeType: mimeType ?? this.mimeType,
      fileSize: fileSize ?? this.fileSize,
      fileType: fileType ?? this.fileType,
      status: status ?? this.status,
      url: url ?? this.url,
      thumbnailUrl: thumbnailUrl ?? this.thumbnailUrl,
      category: category ?? this.category,
      description: description ?? this.description,
      uploadedByUserId: uploadedByUserId ?? this.uploadedByUserId,
      uploadedByUserName: uploadedByUserName ?? this.uploadedByUserName,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
      metadata: metadata ?? this.metadata,
      isPublic: isPublic ?? this.isPublic,
      downloadCount: downloadCount ?? this.downloadCount,
      expiresAt: expiresAt ?? this.expiresAt,
    );
  }
}

/// Model for paginated upload lists
class UploadListDataModel {
  final List<UploadDataModel> uploads;
  final int totalCount;
  final int totalPages;
  final int currentPage;
  final int pageSize;
  final bool hasNextPage;
  final bool hasPreviousPage;
  final int totalSize;
  final Map<FileType, int> typeBreakdown;

  UploadListDataModel({
    required this.uploads,
    required this.totalCount,
    required this.totalPages,
    required this.currentPage,
    required this.pageSize,
    required this.hasNextPage,
    required this.hasPreviousPage,
    required this.totalSize,
    required this.typeBreakdown,
  });

  factory UploadListDataModel.fromJson(Map<String, dynamic> json) {
    final typeBreakdownMap = <FileType, int>{};
    final typeBreakdownJson = json['typeBreakdown'] as Map<String, dynamic>? ?? {};
    
    for (final entry in typeBreakdownJson.entries) {
      final fileType = FileType.values.firstWhere(
        (e) => e.name == entry.key,
        orElse: () => FileType.other,
      );
      typeBreakdownMap[fileType] = entry.value as int;
    }

    return UploadListDataModel(
      uploads: (json['items'] as List<dynamic>?)
              ?.map((item) => UploadDataModel.fromJson(item as Map<String, dynamic>))
              .toList() 
          ?? [],
      totalCount: json['totalCount'] as int? ?? 0,
      totalPages: json['totalPages'] as int? ?? 0,
      currentPage: json['currentPage'] as int? ?? 1,
      pageSize: json['pageSize'] as int? ?? 10,
      hasNextPage: json['hasNextPage'] as bool? ?? false,
      hasPreviousPage: json['hasPreviousPage'] as bool? ?? false,
      totalSize: json['totalSize'] as int? ?? 0,
      typeBreakdown: typeBreakdownMap,
    );
  }

  Map<String, dynamic> toJson() {
    final typeBreakdownJson = <String, dynamic>{};
    for (final entry in typeBreakdown.entries) {
      typeBreakdownJson[entry.key.name] = entry.value;
    }

    return {
      'items': uploads.map((upload) => upload.toJson()).toList(),
      'totalCount': totalCount,
      'totalPages': totalPages,
      'currentPage': currentPage,
      'pageSize': pageSize,
      'hasNextPage': hasNextPage,
      'hasPreviousPage': hasPreviousPage,
      'totalSize': totalSize,
      'typeBreakdown': typeBreakdownJson,
    };
  }

  String get totalSizeFormatted {
    if (totalSize < 1024) return '$totalSize B';
    if (totalSize < 1024 * 1024) return '${(totalSize / 1024).toStringAsFixed(1)} KB';
    if (totalSize < 1024 * 1024 * 1024) return '${(totalSize / (1024 * 1024)).toStringAsFixed(1)} MB';
    return '${(totalSize / (1024 * 1024 * 1024)).toStringAsFixed(1)} GB';
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is UploadListDataModel &&
          runtimeType == other.runtimeType &&
          uploads == other.uploads &&
          currentPage == other.currentPage;

  @override
  int get hashCode => uploads.hashCode ^ currentPage.hashCode;
}