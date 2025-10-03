# Alumni Management System - Frontend API Integration Specification

## 📋 Overview

เอกสารนี้เป็น Integration Specification สำหรับการเชื่อมต่อ Frontend (Flutter) กับ Backend API (.NET Core) ของระบบ Alumni Management System

## 🏗️ Architecture Overview

```
Flutter App (Frontend) ↔ REST API (Backend)
├── Authentication Service
├── Posts Service
├── Comments Service
├── Users Service
├── Profile Service
├── Upload Service
└── Reports Service
```

## 🔑 Authentication & Authorization

### Base URL Configuration
```dart
// Production
static const String BASE_URL = 'https://api.alumni.app';

// Development
static const String BASE_URL = 'http://localhost:5000';
```

### JWT Token Management
```dart
class ApiClient {
  static const String _tokenKey = 'auth_token';
  String? _token;
  
  // Headers สำหรับ Authenticated requests
  Map<String, String> get headers => {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
    if (_token != null) 'Authorization': 'Bearer $_token',
  };
}
```

## 📡 API Endpoints Mapping

### 🔐 Authentication Endpoints

#### 1. Check Authentication Status
```dart
// GET /api/v1/auth/status
Future<AuthStatusResponse> getAuthStatus() async {
  final response = await http.get(
    Uri.parse('$BASE_URL/api/v1/auth/status'),
    headers: headers,
  );
  return AuthStatusResponse.fromJson(jsonDecode(response.body));
}

// Response Model
class AuthStatusResponse {
  final bool authenticationEnabled;
  final List<String> supportedMethods;
  final bool registrationOpen;
  final PasswordRequirements passwordRequirements;
  
  AuthStatusResponse({
    required this.authenticationEnabled,
    required this.supportedMethods,
    required this.registrationOpen,
    required this.passwordRequirements,
  });
  
  factory AuthStatusResponse.fromJson(Map<String, dynamic> json) => 
    AuthStatusResponse(
      authenticationEnabled: json['data']['authenticationEnabled'],
      supportedMethods: List<String>.from(json['data']['supportedMethods']),
      registrationOpen: json['data']['registrationOpen'],
      passwordRequirements: PasswordRequirements.fromJson(
        json['data']['passwordRequirements']
      ),
    );
}
```

#### 2. Login (Email/Password)
```dart
// POST /api/v1/auth/login
Future<LoginResponse> login(LoginRequest request) async {
  final response = await http.post(
    Uri.parse('$BASE_URL/api/v1/auth/login'),
    headers: headers,
    body: jsonEncode(request.toJson()),
  );
  
  if (response.statusCode == 200) {
    final loginResponse = LoginResponse.fromJson(jsonDecode(response.body));
    _token = loginResponse.data.accessToken;
    await _saveToken(_token!);
    return loginResponse;
  } else {
    throw ApiException.fromResponse(response);
  }
}

// Request Model
class LoginRequest {
  final String email;
  final String password;
  
  LoginRequest({required this.email, required this.password});
  
  Map<String, dynamic> toJson() => {
    'email': email,
    'password': password,
  };
}

// Response Model
class LoginResponse extends ApiResponse<LoginData> {
  LoginResponse({required bool success, LoginData? data, String? error, String? message})
    : super(success: success, data: data, error: error, message: message);
    
  factory LoginResponse.fromJson(Map<String, dynamic> json) => 
    LoginResponse(
      success: json['success'],
      data: json['data'] != null ? LoginData.fromJson(json['data']) : null,
      error: json['error'],
      message: json['message'],
    );
}

class LoginData {
  final String accessToken;
  final String refreshToken;
  final DateTime expiresAt;
  final UserData user;
  
  factory LoginData.fromJson(Map<String, dynamic> json) => LoginData(
    accessToken: json['accessToken'],
    refreshToken: json['refreshToken'],
    expiresAt: DateTime.parse(json['expiresAt']),
    user: UserData.fromJson(json['user']),
  );
}
```

#### 3. Login (Mobile Phone)
```dart
// POST /api/v1/auth/login/mobile
Future<AuthResponse> loginWithMobile(MobileLoginRequest request) async {
  final response = await http.post(
    Uri.parse('$BASE_URL/api/v1/auth/login/mobile'),
    headers: headers,
    body: jsonEncode(request.toJson()),
  );
  return AuthResponse.fromJson(jsonDecode(response.body));
}

class MobileLoginRequest {
  final String mobilePhone;
  final String password;
  
  Map<String, dynamic> toJson() => {
    'mobilePhone': mobilePhone,
    'password': password,
  };
}
```

#### 4. Register (Email)
```dart
// POST /api/v1/auth/register
Future<RegisterResponse> register(RegisterRequest request) async {
  final response = await http.post(
    Uri.parse('$BASE_URL/api/v1/auth/register'),
    headers: headers,
    body: jsonEncode(request.toJson()),
  );
  return RegisterResponse.fromJson(jsonDecode(response.body));
}

class RegisterRequest {
  final String name;
  final String email;
  final String password;
  
  Map<String, dynamic> toJson() => {
    'name': name,
    'email': email,
    'password': password,
  };
}
```

#### 5. Register (Mobile Phone)
```dart
// POST /api/v1/auth/register/mobile
Future<ApiResponse<UserData>> registerWithMobile(MobileRegisterRequest request) async {
  final response = await http.post(
    Uri.parse('$BASE_URL/api/v1/auth/register/mobile'),
    headers: headers,
    body: jsonEncode(request.toJson()),
  );
  return ApiResponse<UserData>.fromJson(
    jsonDecode(response.body),
    (data) => UserData.fromJson(data)
  );
}

class MobileRegisterRequest {
  final String mobilePhone;
  final String name;
  final String password;
  final String? email;
  
  Map<String, dynamic> toJson() => {
    'mobilePhone': mobilePhone,
    'name': name,
    'password': password,
    if (email != null) 'email': email,
  };
}
```

#### 6. Get Profile
```dart
// GET /api/v1/auth/profile
Future<ApiResponse<UserData>> getProfile() async {
  final response = await http.get(
    Uri.parse('$BASE_URL/api/v1/auth/profile'),
    headers: headers,
  );
  return ApiResponse<UserData>.fromJson(
    jsonDecode(response.body),
    (data) => UserData.fromJson(data)
  );
}
```

#### 7. Change Password
```dart
// POST /api/v1/auth/change-password
Future<ApiResponse<bool>> changePassword(ChangePasswordRequest request) async {
  final response = await http.post(
    Uri.parse('$BASE_URL/api/v1/auth/change-password'),
    headers: headers,
    body: jsonEncode(request.toJson()),
  );
  return ApiResponse<bool>.fromJson(jsonDecode(response.body), (data) => data);
}

class ChangePasswordRequest {
  final String currentPassword;
  final String newPassword;
  final String confirmPassword;
  
  Map<String, dynamic> toJson() => {
    'currentPassword': currentPassword,
    'newPassword': newPassword,
    'confirmPassword': confirmPassword,
  };
}
```

#### 8. Logout
```dart
// POST /api/v1/auth/logout
Future<ApiResponse<bool>> logout() async {
  final response = await http.post(
    Uri.parse('$BASE_URL/api/v1/auth/logout'),
    headers: headers,
  );
  
  // Clear local token regardless of response
  _token = null;
  await _clearToken();
  
  return ApiResponse<bool>.fromJson(jsonDecode(response.body), (data) => data);
}
```

#### 9. OTP Operations (Future Implementation)
```dart
// POST /api/v1/auth/request-otp
Future<ApiResponse<bool>> requestOtp(RequestOtpRequest request) async {
  final response = await http.post(
    Uri.parse('$BASE_URL/api/v1/auth/request-otp'),
    headers: headers,
    body: jsonEncode(request.toJson()),
  );
  return ApiResponse<bool>.fromJson(jsonDecode(response.body), (data) => data);
}

// POST /api/v1/auth/verify-otp
Future<ApiResponse<bool>> verifyOtp(OtpVerificationRequest request) async {
  final response = await http.post(
    Uri.parse('$BASE_URL/api/v1/auth/verify-otp'),
    headers: headers,
    body: jsonEncode(request.toJson()),
  );
  return ApiResponse<bool>.fromJson(jsonDecode(response.body), (data) => data);
}
```

### 👤 Users Management

#### 1. Get User by ID
```dart
// GET /api/v1/users/{id}
Future<ApiResponse<UserData>> getUser(int userId) async {
  final response = await http.get(
    Uri.parse('$BASE_URL/api/v1/users/$userId'),
    headers: headers,
  );
  return ApiResponse<UserData>.fromJson(
    jsonDecode(response.body),
    (data) => UserData.fromJson(data)
  );
}
```

#### 2. Get Active Users
```dart
// GET /api/v1/users/active
Future<ApiResponse<List<UserData>>> getActiveUsers() async {
  final response = await http.get(
    Uri.parse('$BASE_URL/api/v1/users/active'),
    headers: headers,
  );
  return ApiResponse<List<UserData>>.fromJson(
    jsonDecode(response.body),
    (data) => (data as List).map((item) => UserData.fromJson(item)).toList()
  );
}
```

#### 3. Update User Profile
```dart
// PUT /api/v1/users/{id}
Future<ApiResponse<UserData>> updateUser(int userId, UpdateUserRequest request) async {
  final response = await http.put(
    Uri.parse('$BASE_URL/api/v1/users/$userId'),
    headers: headers,
    body: jsonEncode(request.toJson()),
  );
  return ApiResponse<UserData>.fromJson(
    jsonDecode(response.body),
    (data) => UserData.fromJson(data)
  );
}

class UpdateUserRequest {
  final String name;
  final String email;
  
  Map<String, dynamic> toJson() => {
    'name': name,
    'email': email,
  };
}
```

### 📝 Posts Management

#### 1. Get Posts Feed
```dart
// GET /api/v1/posts?page=1&pageSize=10&type=Text
Future<ApiResponse<PostListData>> getPosts({
  int page = 1,
  int pageSize = 10,
  PostType? type,
}) async {
  var uri = Uri.parse('$BASE_URL/api/v1/posts').replace(queryParameters: {
    'page': page.toString(),
    'pageSize': pageSize.toString(),
    if (type != null) 'type': type.name,
  });
  
  final response = await http.get(uri, headers: headers);
  return ApiResponse<PostListData>.fromJson(
    jsonDecode(response.body),
    (data) => PostListData.fromJson(data)
  );
}

class PostListData {
  final List<PostData> posts;
  final int totalCount;
  final int page;
  final int pageSize;
  final bool hasNextPage;
  final bool hasPreviousPage;
  
  factory PostListData.fromJson(Map<String, dynamic> json) => PostListData(
    posts: (json['posts'] as List).map((item) => PostData.fromJson(item)).toList(),
    totalCount: json['totalCount'],
    page: json['page'],
    pageSize: json['pageSize'],
    hasNextPage: json['hasNextPage'],
    hasPreviousPage: json['hasPreviousPage'],
  );
}
```

#### 2. Get Post by ID
```dart
// GET /api/v1/posts/{id}
Future<ApiResponse<PostData>> getPost(int postId) async {
  final response = await http.get(
    Uri.parse('$BASE_URL/api/v1/posts/$postId'),
    headers: headers,
  );
  return ApiResponse<PostData>.fromJson(
    jsonDecode(response.body),
    (data) => PostData.fromJson(data)
  );
}
```

#### 3. Create Post
```dart
// POST /api/v1/posts
Future<ApiResponse<PostData>> createPost(CreatePostRequest request) async {
  final response = await http.post(
    Uri.parse('$BASE_URL/api/v1/posts'),
    headers: headers,
    body: jsonEncode(request.toJson()),
  );
  return ApiResponse<PostData>.fromJson(
    jsonDecode(response.body),
    (data) => PostData.fromJson(data)
  );
}

class CreatePostRequest {
  final String content;
  final PostType type;
  final String? imageUrl;
  final List<String>? mediaUrls;
  final bool isPinned;
  
  Map<String, dynamic> toJson() => {
    'content': content,
    'type': type.name,
    if (imageUrl != null) 'imageUrl': imageUrl,
    if (mediaUrls != null) 'mediaUrls': mediaUrls,
    'isPinned': isPinned,
  };
}
```

#### 4. Update Post
```dart
// PUT /api/v1/posts/{id}
Future<ApiResponse<PostData>> updatePost(int postId, UpdatePostRequest request) async {
  final response = await http.put(
    Uri.parse('$BASE_URL/api/v1/posts/$postId'),
    headers: headers,
    body: jsonEncode(request.toJson()),
  );
  return ApiResponse<PostData>.fromJson(
    jsonDecode(response.body),
    (data) => PostData.fromJson(data)
  );
}
```

#### 5. Delete Post
```dart
// DELETE /api/v1/posts/{id}
Future<ApiResponse<bool>> deletePost(int postId) async {
  final response = await http.delete(
    Uri.parse('$BASE_URL/api/v1/posts/$postId'),
    headers: headers,
  );
  return ApiResponse<bool>.fromJson(jsonDecode(response.body), (data) => data);
}
```

#### 6. Like/Unlike Post
```dart
// POST /api/v1/posts/{id}/like
Future<ApiResponse<LikeResponse>> likePost(int postId) async {
  final response = await http.post(
    Uri.parse('$BASE_URL/api/v1/posts/$postId/like'),
    headers: headers,
  );
  return ApiResponse<LikeResponse>.fromJson(
    jsonDecode(response.body),
    (data) => LikeResponse.fromJson(data)
  );
}

// DELETE /api/v1/posts/{id}/like
Future<ApiResponse<bool>> unlikePost(int postId) async {
  final response = await http.delete(
    Uri.parse('$BASE_URL/api/v1/posts/$postId/like'),
    headers: headers,
  );
  return ApiResponse<bool>.fromJson(jsonDecode(response.body), (data) => data);
}
```

### 💬 Comments Management

#### 1. Get Comments for Post
```dart
// GET /api/v1/comments/post/{postId}?page=1&pageSize=10
Future<ApiResponse<CommentListData>> getComments(
  int postId, {
  int page = 1,
  int pageSize = 10,
}) async {
  var uri = Uri.parse('$BASE_URL/api/v1/comments/post/$postId').replace(
    queryParameters: {
      'page': page.toString(),
      'pageSize': pageSize.toString(),
    },
  );
  
  final response = await http.get(uri, headers: headers);
  return ApiResponse<CommentListData>.fromJson(
    jsonDecode(response.body),
    (data) => CommentListData.fromJson(data)
  );
}
```

#### 2. Create Comment
```dart
// POST /api/v1/comments
Future<ApiResponse<CommentData>> createComment(CreateCommentRequest request) async {
  final response = await http.post(
    Uri.parse('$BASE_URL/api/v1/comments'),
    headers: headers,
    body: jsonEncode(request.toJson()),
  );
  return ApiResponse<CommentData>.fromJson(
    jsonDecode(response.body),
    (data) => CommentData.fromJson(data)
  );
}

class CreateCommentRequest {
  final int postId;
  final String content;
  final int? parentCommentId; // For nested comments
  
  Map<String, dynamic> toJson() => {
    'postId': postId,
    'content': content,
    if (parentCommentId != null) 'parentCommentId': parentCommentId,
  };
}
```

#### 3. Update Comment
```dart
// PUT /api/v1/comments/{id}
Future<ApiResponse<CommentData>> updateComment(int commentId, UpdateCommentRequest request) async {
  final response = await http.put(
    Uri.parse('$BASE_URL/api/v1/comments/$commentId'),
    headers: headers,
    body: jsonEncode(request.toJson()),
  );
  return ApiResponse<CommentData>.fromJson(
    jsonDecode(response.body),
    (data) => CommentData.fromJson(data)
  );
}
```

#### 4. Delete Comment
```dart
// DELETE /api/v1/comments/{id}
Future<ApiResponse<bool>> deleteComment(int commentId) async {
  final response = await http.delete(
    Uri.parse('$BASE_URL/api/v1/comments/$commentId'),
    headers: headers,
  );
  return ApiResponse<bool>.fromJson(jsonDecode(response.body), (data) => data);
}
```

### 📤 File Upload Management

#### 1. Upload Image/File
```dart
// POST /api/v1/upload/image
Future<ApiResponse<UploadResponse>> uploadImage(File imageFile) async {
  var request = http.MultipartRequest(
    'POST',
    Uri.parse('$BASE_URL/api/v1/upload/image'),
  );
  
  request.headers.addAll(headers);
  request.files.add(
    await http.MultipartFile.fromPath('file', imageFile.path),
  );
  
  final streamedResponse = await request.send();
  final response = await http.Response.fromStream(streamedResponse);
  
  return ApiResponse<UploadResponse>.fromJson(
    jsonDecode(response.body),
    (data) => UploadResponse.fromJson(data)
  );
}

class UploadResponse {
  final String url;
  final String fileName;
  final int fileSize;
  
  factory UploadResponse.fromJson(Map<String, dynamic> json) => UploadResponse(
    url: json['url'],
    fileName: json['fileName'],
    fileSize: json['fileSize'],
  );
}
```

### 📊 Reports Management

#### 1. Create Report
```dart
// POST /api/v1/reports
Future<ApiResponse<ReportData>> createReport(CreateReportRequest request) async {
  final response = await http.post(
    Uri.parse('$BASE_URL/api/v1/reports'),
    headers: headers,
    body: jsonEncode(request.toJson()),
  );
  return ApiResponse<ReportData>.fromJson(
    jsonDecode(response.body),
    (data) => ReportData.fromJson(data)
  );
}

class CreateReportRequest {
  final int? postId;
  final int? commentId;
  final ReportReason reason;
  final String? description;
  
  Map<String, dynamic> toJson() => {
    if (postId != null) 'postId': postId,
    if (commentId != null) 'commentId': commentId,
    'reason': reason.name,
    if (description != null) 'description': description,
  };
}
```

#### 2. Get My Reports
```dart
// GET /api/v1/reports/my-reports?page=1&pageSize=10
Future<ApiResponse<ReportListData>> getMyReports({
  int page = 1,
  int pageSize = 10,
}) async {
  var uri = Uri.parse('$BASE_URL/api/v1/reports/my-reports').replace(
    queryParameters: {
      'page': page.toString(),
      'pageSize': pageSize.toString(),
    },
  );
  
  final response = await http.get(uri, headers: headers);
  return ApiResponse<ReportListData>.fromJson(
    jsonDecode(response.body),
    (data) => ReportListData.fromJson(data)
  );
}
```

## 🔄 Common Response Models

### Base API Response
```dart
class ApiResponse<T> {
  final bool success;
  final T? data;
  final String? error;
  final String? message;
  
  ApiResponse({
    required this.success,
    this.data,
    this.error,
    this.message,
  });
  
  factory ApiResponse.fromJson(
    Map<String, dynamic> json,
    T Function(dynamic) dataParser,
  ) => ApiResponse<T>(
    success: json['success'],
    data: json['data'] != null ? dataParser(json['data']) : null,
    error: json['error'],
    message: json['message'],
  );
}
```

### User Data Model
```dart
class UserData {
  final int id;
  final String name;
  final String email;
  final String? mobilePhone;
  final String? avatar;
  final String role;
  final bool isActive;
  final DateTime createdAt;
  final DateTime? updatedAt;
  
  factory UserData.fromJson(Map<String, dynamic> json) => UserData(
    id: json['id'],
    name: json['name'],
    email: json['email'],
    mobilePhone: json['mobilePhone'],
    avatar: json['avatar'],
    role: json['role'],
    isActive: json['isActive'],
    createdAt: DateTime.parse(json['createdAt']),
    updatedAt: json['updatedAt'] != null ? DateTime.parse(json['updatedAt']) : null,
  );
}
```

### Post Data Model
```dart
class PostData {
  final int id;
  final int userId;
  final String userName;
  final String? userAvatar;
  final String content;
  final PostType type;
  final String? imageUrl;
  final List<String>? mediaUrls;
  final int mediaCount;
  final bool isPinned;
  final int likesCount;
  final int commentsCount;
  final bool isLikedByCurrentUser;
  final bool isOwnPost;
  final DateTime createdAt;
  final DateTime updatedAt;
  
  factory PostData.fromJson(Map<String, dynamic> json) => PostData(
    id: json['id'],
    userId: json['userId'],
    userName: json['userName'],
    userAvatar: json['userAvatar'],
    content: json['content'],
    type: PostType.values.firstWhere((e) => e.name == json['type']),
    imageUrl: json['imageUrl'],
    mediaUrls: json['mediaUrls']?.cast<String>(),
    mediaCount: json['mediaCount'] ?? 0,
    isPinned: json['isPinned'],
    likesCount: json['likesCount'],
    commentsCount: json['commentsCount'],
    isLikedByCurrentUser: json['isLikedByCurrentUser'],
    isOwnPost: json['isOwnPost'],
    createdAt: DateTime.parse(json['createdAt']),
    updatedAt: DateTime.parse(json['updatedAt']),
  );
}

enum PostType { Text, Image, Video, Link }
```

### Comment Data Model
```dart
class CommentData {
  final int id;
  final int postId;
  final int userId;
  final String userName;
  final String? userAvatar;
  final String content;
  final int? parentCommentId;
  final bool isOwnComment;
  final DateTime createdAt;
  final DateTime updatedAt;
  
  factory CommentData.fromJson(Map<String, dynamic> json) => CommentData(
    id: json['id'],
    postId: json['postId'],
    userId: json['userId'],
    userName: json['userName'],
    userAvatar: json['userAvatar'],
    content: json['content'],
    parentCommentId: json['parentCommentId'],
    isOwnComment: json['isOwnComment'],
    createdAt: DateTime.parse(json['createdAt']),
    updatedAt: DateTime.parse(json['updatedAt']),
  );
}
```

## 🛠️ Service Implementation Example

### API Service Class
```dart
class ApiService {
  static const String BASE_URL = 'http://localhost:5000'; // Change for production
  static const String _tokenKey = 'auth_token';
  String? _token;
  
  // Initialize token from storage
  Future<void> initialize() async {
    _token = await _getStoredToken();
  }
  
  // HTTP Headers
  Map<String, String> get headers => {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
    if (_token != null) 'Authorization': 'Bearer $_token',
  };
  
  // Token management
  Future<void> _saveToken(String token) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_tokenKey, token);
    _token = token;
  }
  
  Future<String?> _getStoredToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString(_tokenKey);
  }
  
  Future<void> _clearToken() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_tokenKey);
    _token = null;
  }
  
  // Authentication Methods
  Future<LoginResponse> login(LoginRequest request) async {
    final response = await http.post(
      Uri.parse('$BASE_URL/api/v1/auth/login'),
      headers: headers,
      body: jsonEncode(request.toJson()),
    );
    
    if (response.statusCode == 200) {
      final loginResponse = LoginResponse.fromJson(jsonDecode(response.body));
      if (loginResponse.success && loginResponse.data != null) {
        await _saveToken(loginResponse.data!.accessToken);
      }
      return loginResponse;
    } else {
      throw ApiException.fromResponse(response);
    }
  }
  
  // Posts Methods
  Future<ApiResponse<PostListData>> getPosts({
    int page = 1,
    int pageSize = 10,
    PostType? type,
  }) async {
    var queryParams = {
      'page': page.toString(),
      'pageSize': pageSize.toString(),
    };
    
    if (type != null) {
      queryParams['type'] = type.name;
    }
    
    final uri = Uri.parse('$BASE_URL/api/v1/posts').replace(
      queryParameters: queryParams,
    );
    
    final response = await http.get(uri, headers: headers);
    
    if (response.statusCode == 200) {
      return ApiResponse<PostListData>.fromJson(
        jsonDecode(response.body),
        (data) => PostListData.fromJson(data),
      );
    } else {
      throw ApiException.fromResponse(response);
    }
  }
  
  // Add other methods...
}
```

### Error Handling
```dart
class ApiException implements Exception {
  final int statusCode;
  final String message;
  final Map<String, dynamic>? errorDetails;
  
  ApiException({
    required this.statusCode,
    required this.message,
    this.errorDetails,
  });
  
  factory ApiException.fromResponse(http.Response response) {
    final body = jsonDecode(response.body);
    
    return ApiException(
      statusCode: response.statusCode,
      message: body['message'] ?? 'Unknown error occurred',
      errorDetails: body,
    );
  }
  
  @override
  String toString() => 'ApiException: $message (Status: $statusCode)';
}
```

## 📱 GetX Service Integration

### Authentication Service
```dart
class AuthService extends GetxService {
  final ApiService _apiService = Get.find<ApiService>();
  final RxBool isLoggedIn = false.obs;
  final Rx<UserData?> currentUser = Rx<UserData?>(null);
  
  @override
  Future<void> onInit() async {
    super.onInit();
    await _checkAuthStatus();
  }
  
  Future<void> _checkAuthStatus() async {
    try {
      final response = await _apiService.getProfile();
      if (response.success && response.data != null) {
        currentUser.value = response.data;
        isLoggedIn.value = true;
      }
    } catch (e) {
      isLoggedIn.value = false;
      currentUser.value = null;
    }
  }
  
  Future<bool> login(String email, String password) async {
    try {
      final request = LoginRequest(email: email, password: password);
      final response = await _apiService.login(request);
      
      if (response.success && response.data != null) {
        currentUser.value = response.data!.user;
        isLoggedIn.value = true;
        return true;
      }
      return false;
    } catch (e) {
      return false;
    }
  }
  
  Future<void> logout() async {
    try {
      await _apiService.logout();
    } finally {
      currentUser.value = null;
      isLoggedIn.value = false;
    }
  }
}
```

### Posts Service
```dart
class PostsService extends GetxService {
  final ApiService _apiService = Get.find<ApiService>();
  final RxList<PostData> posts = <PostData>[].obs;
  final RxBool isLoading = false.obs;
  final RxInt currentPage = 1.obs;
  final RxBool hasMore = true.obs;
  
  Future<void> loadPosts({bool refresh = false}) async {
    if (isLoading.value) return;
    
    isLoading.value = true;
    
    try {
      final page = refresh ? 1 : currentPage.value;
      final response = await _apiService.getPosts(page: page);
      
      if (response.success && response.data != null) {
        if (refresh) {
          posts.clear();
          currentPage.value = 1;
        }
        
        posts.addAll(response.data!.posts);
        hasMore.value = response.data!.hasNextPage;
        
        if (!refresh) {
          currentPage.value++;
        }
      }
    } catch (e) {
      // Handle error
    } finally {
      isLoading.value = false;
    }
  }
  
  Future<bool> createPost(String content, PostType type) async {
    try {
      final request = CreatePostRequest(
        content: content,
        type: type,
        isPinned: false,
      );
      
      final response = await _apiService.createPost(request);
      
      if (response.success && response.data != null) {
        posts.insert(0, response.data!);
        return true;
      }
      return false;
    } catch (e) {
      return false;
    }
  }
}
```

## 🔄 State Management Integration

### Controllers Example
```dart
class HomeController extends GetxController {
  final PostsService _postsService = Get.find<PostsService>();
  final AuthService _authService = Get.find<AuthService>();
  
  @override
  void onInit() {
    super.onInit();
    _loadInitialData();
  }
  
  Future<void> _loadInitialData() async {
    await _postsService.loadPosts(refresh: true);
  }
  
  Future<void> onRefresh() async {
    await _postsService.loadPosts(refresh: true);
  }
  
  Future<void> loadMore() async {
    await _postsService.loadPosts();
  }
  
  // Getters for UI
  List<PostData> get posts => _postsService.posts;
  bool get isLoading => _postsService.isLoading.value;
  bool get hasMore => _postsService.hasMore.value;
  UserData? get currentUser => _authService.currentUser.value;
}
```

## 🧪 Testing Integration

### API Service Testing
```dart
void main() {
  group('API Service Tests', () {
    late ApiService apiService;
    
    setUp(() {
      apiService = ApiService();
    });
    
    test('should login successfully with valid credentials', () async {
      final request = LoginRequest(
        email: 'test@example.com',
        password: 'password123',
      );
      
      final response = await apiService.login(request);
      
      expect(response.success, isTrue);
      expect(response.data?.accessToken, isNotNull);
    });
    
    test('should get posts with pagination', () async {
      final response = await apiService.getPosts(page: 1, pageSize: 10);
      
      expect(response.success, isTrue);
      expect(response.data?.posts, isNotEmpty);
    });
  });
}
```

## 🔧 Configuration & Environment

### Environment Configuration
```dart
abstract class AppConfig {
  static const String apiBaseUrl = String.fromEnvironment(
    'API_BASE_URL',
    defaultValue: 'http://localhost:5000',
  );
  
  static const bool enableLogging = bool.fromEnvironment(
    'ENABLE_API_LOGGING',
    defaultValue: true,
  );
  
  static const int apiTimeoutSeconds = int.fromEnvironment(
    'API_TIMEOUT_SECONDS',
    defaultValue: 30,
  );
}
```

### HTTP Client Configuration
```dart
class HttpClientConfig {
  static http.Client createClient() {
    return http.Client();
  }
  
  static Duration get timeout => Duration(seconds: AppConfig.apiTimeoutSeconds);
}
```

## 📋 Implementation Checklist

### Phase 1: Core Integration
- [ ] Setup base API service class
- [ ] Implement authentication endpoints
- [ ] Setup token management
- [ ] Implement error handling
- [ ] Create response models
- [ ] Setup GetX services integration

### Phase 2: Content Management
- [ ] Implement posts endpoints
- [ ] Implement comments endpoints
- [ ] Implement file upload
- [ ] Add pagination support
- [ ] Implement caching strategy

### Phase 3: Advanced Features
- [ ] Implement reports system
- [ ] Add offline support
- [ ] Implement push notifications
- [ ] Add real-time updates
- [ ] Performance optimization

### Phase 4: Testing & Documentation
- [ ] Unit tests for API services
- [ ] Integration tests
- [ ] API documentation
- [ ] Error scenarios testing
- [ ] Performance testing

## 🚨 Important Notes

1. **Token Security**: Always store JWT tokens securely using `flutter_secure_storage` for production
2. **Error Handling**: Implement comprehensive error handling for network failures, timeouts, and API errors
3. **Offline Support**: Consider implementing local caching with `sqflite` for offline functionality
4. **Performance**: Use pagination for large datasets and implement pull-to-refresh
5. **Security**: Validate all user inputs and implement proper authentication checks
6. **Monitoring**: Add logging and analytics to track API usage and errors

## 📞 API Base URL Configuration

```dart
// For different environments
class Environment {
  static const String DEVELOPMENT = 'http://localhost:5000';
  static const String STAGING = 'https://staging-api.alumni.app';
  static const String PRODUCTION = 'https://api.alumni.app';
  
  static String get baseUrl {
    const environment = String.fromEnvironment('ENVIRONMENT', defaultValue: 'DEVELOPMENT');
    switch (environment) {
      case 'STAGING':
        return STAGING;
      case 'PRODUCTION':
        return PRODUCTION;
      default:
        return DEVELOPMENT;
    }
  }
}
```

---

**Last Updated**: October 2, 2025
**Version**: 1.0.0
**API Version**: v1