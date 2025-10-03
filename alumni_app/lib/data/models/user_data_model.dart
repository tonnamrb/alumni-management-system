/// Base API response model
class ApiResponseModel<T> {
  const ApiResponseModel({
    required this.success,
    this.data,
    this.error,
    this.message,
  });
  
  final bool success;
  final T? data;
  final String? error;
  final String? message;
  
  factory ApiResponseModel.fromJson(
    Map<String, dynamic> json,
    T Function(dynamic)? dataParser,
  ) {
    return ApiResponseModel<T>(
      success: json['success'] as bool,
      data: json['data'] != null && dataParser != null
          ? dataParser(json['data'])
          : json['data'] as T?,
      error: json['error'] as String?,
      message: json['message'] as String?,
    );
  }
  
  Map<String, dynamic> toJson() => {
    'success': success,
    'data': data,
    'error': error,
    'message': message,
  };
}

/// User data model for API responses
class UserDataModel {
  const UserDataModel({
    required this.id,
    required this.name,
    required this.email,
    this.mobilePhone,
    this.avatar,
    required this.role,
    required this.isActive,
    required this.createdAt,
    this.updatedAt,
  });
  
  final int id;
  final String name;
  final String email;
  final String? mobilePhone;
  final String? avatar;
  final String role;
  final bool isActive;
  final String createdAt;
  final String? updatedAt;
  
  factory UserDataModel.fromJson(Map<String, dynamic> json) {
    return UserDataModel(
      id: json['id'] as int,
      name: json['name'] as String,
      email: json['email'] as String,
      mobilePhone: json['mobilePhone'] as String?,
      avatar: json['avatar'] as String?,
      role: json['role'] as String,
      isActive: json['isActive'] as bool,
      createdAt: json['createdAt'] as String,
      updatedAt: json['updatedAt'] as String?,
    );
  }
  
  Map<String, dynamic> toJson() => {
    'id': id,
    'name': name,
    'email': email,
    'mobilePhone': mobilePhone,
    'avatar': avatar,
    'role': role,
    'isActive': isActive,
    'createdAt': createdAt,
    'updatedAt': updatedAt,
  };
}

/// Model for paginated user lists
class UserListDataModel {
  final List<UserDataModel> users;
  final int totalCount;
  final int totalPages;
  final int currentPage;
  final int pageSize;
  final bool hasNextPage;
  final bool hasPreviousPage;

  UserListDataModel({
    required this.users,
    required this.totalCount,
    required this.totalPages,
    required this.currentPage,
    required this.pageSize,
    required this.hasNextPage,
    required this.hasPreviousPage,
  });

  factory UserListDataModel.fromJson(Map<String, dynamic> json) {
    return UserListDataModel(
      users: (json['items'] as List<dynamic>?)
              ?.map((item) => UserDataModel.fromJson(item as Map<String, dynamic>))
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
      'items': users.map((user) => user.toJson()).toList(),
      'totalCount': totalCount,
      'totalPages': totalPages,
      'currentPage': currentPage,
      'pageSize': pageSize,
      'hasNextPage': hasNextPage,
      'hasPreviousPage': hasPreviousPage,
    };
  }
}

/// Model for user statistics
class UserStatsDataModel {
  final int totalPosts;
  final int totalComments;
  final int totalLikes;
  final int profileViews;
  final DateTime? lastActivityAt;

  UserStatsDataModel({
    required this.totalPosts,
    required this.totalComments,
    required this.totalLikes,
    required this.profileViews,
    this.lastActivityAt,
  });

  factory UserStatsDataModel.fromJson(Map<String, dynamic> json) {
    return UserStatsDataModel(
      totalPosts: json['totalPosts'] as int? ?? 0,
      totalComments: json['totalComments'] as int? ?? 0,
      totalLikes: json['totalLikes'] as int? ?? 0,
      profileViews: json['profileViews'] as int? ?? 0,
      lastActivityAt: json['lastActivityAt'] != null
          ? DateTime.parse(json['lastActivityAt'] as String)
          : null,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'totalPosts': totalPosts,
      'totalComments': totalComments,
      'totalLikes': totalLikes,
      'profileViews': profileViews,
      'lastActivityAt': lastActivityAt?.toIso8601String(),
    };
  }
}