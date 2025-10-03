# UI Integration Specification: Login/Register & Feed System
## การปรับ UI สำหรับ Login/Register ด้วยเบอร์โทร + Feed/Post/Comment API Integration

### 📋 Project Overview
เอกสารนี้กำหนดรายละเอียดการปรับปรุง Alumni App UI เพื่อรองรับ:
- **Mobile Phone Authentication** (Login/Register ด้วยเบอร์โทรศัพท์)
- **Role-based Access Control** (Admin/Member roles)
- **Real API Integration** สำหรับ Feed, Posts, Comments
- **Complete User Journey** ตั้งแต่ Registration จนถึง Social Features

---

## 🎯 Phase 1: Mobile Authentication System

### 1.1 New Authentication Flow

#### 📱 Mobile Phone Registration Flow
```
1. Welcome Screen → Enter Mobile Phone Number
2. OTP Verification → Enter 6-digit code  
3. Password Setup → Create password
4. Profile Setup → Fill basic info (Optional)
5. Home Feed → Ready to use
```

#### 🔐 Mobile Phone Login Flow  
```
1. Login Screen → Enter Mobile Phone + Password
2. Authentication → API validation
3. Role Detection → Admin/Member routing
4. Home Feed → Role-appropriate interface
```

### 1.2 UI Components Structure

#### AuthenticationPages/
```
lib/presentation/pages/auth/
├── welcome_page.dart              # เลือก Login/Register
├── phone_input_page.dart          # กรอกเบอร์โทร
├── otp_verification_page.dart     # ยืนยัน OTP
├── password_setup_page.dart       # ตั้งรหัสผ่าน (Register only)
├── login_page.dart               # Login ด้วยเบอร์ + รหัสผ่าน
└── profile_setup_page.dart       # ข้อมูลเบื้องต้น (Optional)
```

#### AuthenticationWidgets/
```
lib/presentation/widgets/auth/
├── phone_number_input.dart        # Input field พร้อม country code
├── otp_input_widget.dart         # 6 ช่อง OTP input
├── password_strength_indicator.dart # แสดงความแข็งแรงรหัสผ่าน
├── auth_button.dart              # ปุ่มสำหรับ auth actions
└── loading_overlay.dart          # Loading state สำหรับ API calls
```

### 1.3 Authentication API Integration

#### AuthService Implementation
```dart
class AuthService {
  // ตรวจสอบว่าเบอร์นี้สมัครได้หรือไม่
  Future<Result<bool>> canRegisterWithPhone(String phone);
  
  // ส่ง OTP สำหรับลงทะเบียน
  Future<Result<void>> requestRegistrationOtp(String phone);
  
  // ตรวจสอบ OTP
  Future<Result<bool>> verifyOtp(String phone, String otp);
  
  // เสร็จสิ้นการลงทะเบียน
  Future<Result<AuthResult>> completeRegistration(
    String phone, String password);
  
  // เข้าสู่ระบบ
  Future<Result<AuthResult>> loginWithPhone(String phone, String password);
  
  // ตรวจสอบสถานะการเข้าสู่ระบบ
  Future<Result<User>> getCurrentUser();
  
  // ออกจากระบบ
  Future<void> logout();
}
```

#### Authentication Models
```dart
class AuthResult {
  final String accessToken;
  final String refreshToken;
  final DateTime expiresAt;
  final User user;
}

class User {
  final int id;
  final String email;
  final String firstname;
  final String lastname;
  final String fullName;
  final String? mobilePhone;
  final int roleId;
  final String roleName;
  final bool isMember;
  final bool isAdmin;
  final AlumniProfile? alumniProfile;
}
```

### 1.4 UI Design Specifications

#### 🎨 Phone Input Page
```dart
// Key Features:
- Country code selector (default: +66 Thailand)
- Phone number formatting (0812345678 → 081-234-5678)  
- Real-time validation
- "Next" button เปิด/ปิดตาม validation
- Error messages ภาษาไทย
```

#### 🔢 OTP Verification Page  
```dart
// Key Features:
- 6 input boxes แยกจากกัน
- Auto-focus ไปช่องถัดไป
- Countdown timer (60 seconds)
- "Resend OTP" button
- Auto-submit เมื่อครบ 6 หลัก
```

#### 🔐 Password Setup Page
```dart  
// Key Features:
- Password strength indicator
- Show/hide password toggle
- Confirm password field  
- Minimum requirements display
- "Create Account" button
```

---

## 🎯 Phase 2: Role-Based Access Control

### 2.1 Role Management System

#### UserRole Types
```dart
enum UserRole {
  member(id: 1, name: 'Member'),
  admin(id: 2, name: 'Administrator');
  
  const UserRole({required this.id, required this.name});
  final int id;
  final String name;
}
```

#### Role-Based Navigation
```dart
class RoleBasedRouter {
  static Widget getHomePageForRole(UserRole role) {
    switch (role) {
      case UserRole.admin:
        return AdminHomePage();
      case UserRole.member:
        return MemberHomePage();
    }
  }
}
```

### 2.2 Permission System

#### PermissionService
```dart
class PermissionService {
  // ตรวจสอบสิทธิ์การดูโพสต์
  bool canViewPost(User user, Post post);
  
  // ตรวจสอบสิทธิ์การสร้างโพสต์
  bool canCreatePost(User user);
  
  // ตรวจสอบสิทธิ์การแก้ไขโพสต์
  bool canEditPost(User user, Post post);
  
  // ตรวจสอบสิทธิ์การลบโพสต์
  bool canDeletePost(User user, Post post);
  
  // ตรวจสอบสิทธิ์การ moderate (Admin only)
  bool canModerateContent(User user);
  
  // ตรวจสอบสิทธิ์การดูรายงาน
  bool canViewReports(User user);
}
```

### 2.3 UI Components by Role

#### 👤 Member Interface
```dart
// Bottom Navigation (Members)
- Home Feed (ดูโพสต์ทั้งหมด)
- Create Post (สร้างโพสต์ใหม่)  
- Profile (โปรไฟล์ส่วนตัว)
- Settings (ตั้งค่า)
```

#### 👑 Admin Interface
```dart  
// Bottom Navigation (Admins)
- Home Feed (ดูโพสต์ทั้งหมด + Admin controls)
- Create Post (สร้างโพสต์ + Pinned posts)
- Reports (จัดการรายงาน)
- Users (จัดการผู้ใช้)
- Profile (โปรไฟล์ส่วนตัว)
```

---

## 🎯 Phase 3: Feed & Posts System

### 3.1 Feed Architecture

#### FeedService Implementation
```dart
class FeedService {
  // ดึงโพสต์ทั้งหมดแบบ pagination
  Future<Result<PostList>> getPosts({
    int page = 1,
    int pageSize = 10,
    PostType? type,
  });
  
  // ดึงโพสต์ที่ pin ไว้
  Future<Result<List<Post>>> getPinnedPosts();
  
  // สร้างโพสต์ใหม่
  Future<Result<Post>> createPost(CreatePostRequest request);
  
  // แก้ไขโพสต์
  Future<Result<Post>> updatePost(int postId, UpdatePostRequest request);
  
  // ลบโพสต์
  Future<Result<void>> deletePost(int postId);
  
  // กดไลก์/ยกเลิกไลก์
  Future<Result<void>> toggleLike(int postId);
  
  // รายงานโพสต์
  Future<Result<void>> reportPost(int postId, String reason);
}
```

#### Feed Models
```dart
class Post {
  final int id;
  final String content;
  final List<String>? mediaUrls;
  final PostType type;
  final bool isPinned;
  final DateTime createdAt;
  final DateTime updatedAt;
  
  // Author information
  final int authorId;
  final String authorName;
  final String? authorPictureUrl;
  
  // Interaction data
  final int likesCount;
  final int commentsCount;
  final bool isLikedByCurrentUser;
  final List<Comment> comments;
}

enum PostType {
  announcement('announcement'),
  general('general'),
  event('event');
}

class PostList {
  final List<Post> posts;
  final int currentPage;
  final int totalPages;
  final int totalPosts;
  final bool hasNextPage;
}
```

### 3.2 Feed UI Components

#### 📝 Post Card Widget
```dart
class PostCardWidget extends StatelessWidget {
  // Features:
  - Author avatar + name + timestamp
  - Content text (expandable)
  - Media carousel (images/videos)
  - Like/Comment/Share buttons
  - Comments preview (first 2 comments)
  - "View all comments" button
  - Admin controls (pin/delete) if applicable
}
```

#### 📱 Create Post Page
```dart
class CreatePostPage extends StatefulWidget {
  // Features:
  - Text content input
  - Media picker (images/videos)
  - Post type selector (Admin only)
  - Privacy settings
  - Pin option (Admin only)
  - Preview mode
  - Draft save/load
}
```

#### 🔄 Pull-to-Refresh & Pagination
```dart
class FeedListWidget extends StatefulWidget {
  // Features:
  - Pull-to-refresh
  - Infinite scrolling
  - Loading states
  - Empty state
  - Error handling
  - Cache management
}
```

### 3.3 Media Upload System

#### UploadService Implementation
```dart
class UploadService {
  // อัพโหลดรูปภาพ/วิดีโอ
  Future<Result<List<String>>> uploadMedia(List<File> files);
  
  // อัพโหลดรูปโปรไฟล์
  Future<Result<String>> uploadProfilePicture(File file);
  
  // ลบไฟล์ที่อัพโหลด
  Future<Result<void>> deleteUploadedFile(String fileUrl);
}
```

#### Media Picker Widget
```dart
class MediaPickerWidget extends StatelessWidget {
  // Features:
  - Camera capture
  - Gallery selection
  - Multi-select support
  - Image/video preview
  - Compression settings
  - Upload progress
}
```

---

## 🎯 Phase 4: Comments System

### 4.1 Comments Architecture

#### CommentService Implementation
```dart
class CommentService {
  // ดึงความคิดเห็นของโพสต์
  Future<Result<List<Comment>>> getPostComments(int postId, {
    int page = 1,
    int pageSize = 20,
  });
  
  // สร้างความคิดเห็นใหม่
  Future<Result<Comment>> createComment(int postId, String content);
  
  // ตอบกลับความคิดเห็น
  Future<Result<Comment>> replyToComment(int commentId, String content);
  
  // แก้ไขความคิดเห็น
  Future<Result<Comment>> updateComment(int commentId, String content);
  
  // ลบความคิดเห็น
  Future<Result<void>> deleteComment(int commentId);
  
  // รายงานความคิดเห็น
  Future<Result<void>> reportComment(int commentId, String reason);
}
```

#### Comment Models
```dart
class Comment {
  final int id;
  final String content;
  final DateTime createdAt;
  final DateTime? updatedAt;
  
  // Author information
  final int authorId;
  final String authorName;
  final String? authorPictureUrl;
  
  // Reply system
  final int? parentCommentId;
  final List<Comment> replies;
  final int repliesCount;
  
  // Moderation
  final bool isDeleted;
  final String? deletedReason;
}
```

### 4.2 Comments UI Components

#### 💬 Comments Sheet
```dart
class CommentsBottomSheet extends StatefulWidget {
  // Features:
  - Full-screen bottom sheet
  - Comments list with replies
  - New comment input
  - Reply functionality  
  - Load more comments
  - Admin moderation tools
}
```

#### 🗨️ Comment Widget
```dart
class CommentWidget extends StatelessWidget {
  // Features:
  - Author info + timestamp
  - Comment content
  - Reply button
  - Nested replies (max 2 levels)
  - Edit/Delete options (own comments)
  - Report option
  - Admin moderation options
}
```

#### ✏️ Comment Input Widget
```dart
class CommentInputWidget extends StatefulWidget {
  // Features:
  - Text input with placeholder
  - Send button (enabled when text exists)
  - Character counter
  - Emoji picker
  - Reply indicator (when replying)
  - Loading state during submission
}
```

---

## 🎯 Phase 5: Profile System

### 5.1 Profile Management

#### ProfileService Implementation
```dart
class ProfileService {
  // ดึงโปรไฟล์ผู้ใช้
  Future<Result<UserProfile>> getUserProfile(int userId);
  
  // ดึงโปรไฟล์ตัวเอง
  Future<Result<UserProfile>> getMyProfile();
  
  // อัพเดทโปรไฟล์
  Future<Result<UserProfile>> updateProfile(UpdateProfileRequest request);
  
  // อัพโหลดรูปโปรไฟล์
  Future<Result<String>> updateProfilePicture(File imageFile);
  
  // ดึงโพสต์ของผู้ใช้
  Future<Result<List<Post>>> getUserPosts(int userId, {
    int page = 1,
    int pageSize = 12,
  });
}
```

#### Profile Models
```dart
class UserProfile {
  final User user;
  final AlumniProfile? alumniProfile;
  final ProfileStats stats;
  final List<Post> recentPosts;
}

class AlumniProfile {
  final String? bio;
  final String? profilePictureUrl;
  final int? graduationYear;
  final String? major;
  final String? currentJobTitle;
  final String? currentCompany;
  final String? phoneNumber;
  final bool isProfilePublic;
  
  // Alumni-specific fields
  final String? memberID;
  final String? nameInYearbook;
  final String? groupID;
  final String? lineID;
  final String? facebook;
}

class ProfileStats {
  final int postsCount;
  final int followersCount; // Future feature
  final int followingCount; // Future feature
}
```

### 5.2 Profile UI Components

#### 👤 Profile Page
```dart
class ProfilePage extends StatefulWidget {
  // Sections:
  - Profile Header (avatar, name, bio)
  - Stats Row (posts, followers, following)
  - Action Buttons (edit profile, follow)
  - Posts Grid (Instagram-style)
  - Admin Badge (if applicable)
}
```

#### ✏️ Edit Profile Page
```dart
class EditProfilePage extends StatefulWidget {
  // Features:
  - Profile picture editor
  - Basic info form (name, bio)
  - Contact information
  - Privacy settings
  - Alumni-specific fields
  - Save/Cancel buttons
}
```

#### 📸 Profile Picture Editor
```dart
class ProfilePictureEditor extends StatefulWidget {
  // Features:
  - Current picture display
  - Camera/Gallery picker
  - Image cropping
  - Preview before save
  - Remove picture option
}
```

---

## 🎯 Phase 6: Admin Features

### 6.1 Content Moderation

#### AdminService Implementation
```dart
class AdminService {
  // ดึงรายงานที่รอการตรวจสอบ
  Future<Result<List<Report>>> getPendingReports({
    int page = 1,
    int pageSize = 20,
  });
  
  // จัดการรายงาน (อนุมัติ/ปฏิเสธ)
  Future<Result<void>> resolveReport(int reportId, ReportResolution resolution);
  
  // Pin/Unpin โพสต์
  Future<Result<void>> togglePinPost(int postId);
  
  // ลบโพสต์ (Admin)
  Future<Result<void>> deletePostAsAdmin(int postId, String reason);
  
  // ดูสถิติระบบ
  Future<Result<SystemStats>> getSystemStats();
}
```

#### Report Models
```dart
class Report {
  final int id;
  final ReportType type;
  final String reason;
  final DateTime createdAt;
  final ReportStatus status;
  
  // Reported content
  final int? postId;
  final int? commentId;
  final Post? reportedPost;
  final Comment? reportedComment;
  
  // Reporter info
  final int reporterId;
  final String reporterName;
  
  // Resolution
  final int? resolvedByUserId;
  final String? resolvedByUserName;
  final DateTime? resolvedAt;
  final String? resolutionNote;
}

enum ReportType { post, comment, user }
enum ReportStatus { pending, approved, rejected }
```

### 6.2 Admin UI Components

#### 🛠️ Admin Dashboard
```dart
class AdminDashboardPage extends StatefulWidget {
  // Sections:
  - Quick Stats Cards
  - Pending Reports List
  - Recent Activities
  - System Health Status
  - Quick Actions Menu
}
```

#### 📋 Reports Management Page
```dart
class ReportsManagementPage extends StatefulWidget {
  // Features:
  - Reports list with filters
  - Report detail modal
  - Bulk actions
  - Resolution workflow
  - Search and sorting
}
```

#### ⚡ Quick Moderation Actions
```dart
class QuickModerationWidget extends StatelessWidget {
  // Features:
  - Pin/Unpin post
  - Delete post/comment
  - Ban user (Future)
  - Feature post (Future)
}
```

---

## 🎯 Phase 7: Error Handling & UX

### 7.1 Error Management

#### ErrorHandlingService
```dart
class ErrorHandlingService {
  // แสดง error message แบบ user-friendly
  static void showError(BuildContext context, AppError error);
  
  // จัดการ network errors
  static AppError handleNetworkError(DioError error);
  
  // จัดการ authentication errors
  static AppError handleAuthError(int statusCode);
  
  // แสดง retry dialog
  static Future<bool> showRetryDialog(BuildContext context);
}
```

#### Error Types
```dart
class AppError {
  final ErrorType type;
  final String message;
  final String? technicalDetails;
  final bool canRetry;
}

enum ErrorType {
  network,
  authentication, 
  authorization,
  validation,
  server,
  unknown
}
```

### 7.2 Loading States

#### Loading Widgets
```dart
// Shimmer loading for feed
class FeedShimmerWidget extends StatelessWidget {}

// Loading overlay for actions
class ActionLoadingOverlay extends StatelessWidget {}

// Pull-to-refresh indicator
class CustomRefreshIndicator extends StatelessWidget {}

// Pagination loading indicator  
class PaginationLoader extends StatelessWidget {}
```

### 7.3 Offline Support

#### OfflineService
```dart
class OfflineService {
  // Cache posts for offline viewing
  Future<void> cachePosts(List<Post> posts);
  
  // Get cached posts
  Future<List<Post>> getCachedPosts();
  
  // Queue actions for when online
  Future<void> queueAction(OfflineAction action);
  
  // Sync queued actions
  Future<void> syncQueuedActions();
}
```

---

## 🎯 Phase 8: State Management

### 8.1 GetX State Management

#### Controllers Structure
```dart
// Authentication
class AuthController extends GetxController {
  final Rx<AuthState> authState = AuthState.initial().obs;
  final Rx<User?> currentUser = Rx<User?>(null);
}

// Feed
class FeedController extends GetxController {
  final RxList<Post> posts = <Post>[].obs;
  final Rx<FeedState> feedState = FeedState.initial().obs;
}

// Profile  
class ProfileController extends GetxController {
  final Rx<UserProfile?> userProfile = Rx<UserProfile?>(null);
  final Rx<ProfileState> profileState = ProfileState.initial().obs;
}

// Admin
class AdminController extends GetxController {
  final RxList<Report> pendingReports = <Report>[].obs;
  final Rx<AdminState> adminState = AdminState.initial().obs;
}
```

### 8.2 State Classes
```dart
class AuthState {
  final bool isLoading;
  final bool isAuthenticated;
  final AppError? error;
}

class FeedState {
  final bool isLoading;
  final bool isRefreshing;
  final bool hasMore;
  final AppError? error;
}
```

---

## 🎯 Phase 9: Navigation & Routing

### 9.1 Route Management

#### App Routes
```dart
class AppRoutes {
  // Authentication routes
  static const welcome = '/welcome';
  static const phoneInput = '/phone-input';
  static const otpVerification = '/otp-verification';
  static const passwordSetup = '/password-setup';
  static const login = '/login';
  
  // Main routes
  static const home = '/home';
  static const profile = '/profile';
  static const editProfile = '/edit-profile';
  static const createPost = '/create-post';
  static const postDetail = '/post/:id';
  
  // Admin routes
  static const adminDashboard = '/admin/dashboard';
  static const reportsManagement = '/admin/reports';
  static const userManagement = '/admin/users';
}
```

#### Navigation Guards
```dart
class AuthGuard extends GetMiddleware {
  @override
  RouteSettings? redirect(String? route) {
    final authController = Get.find<AuthController>();
    if (!authController.isAuthenticated) {
      return const RouteSettings(name: AppRoutes.welcome);
    }
    return null;
  }
}

class AdminGuard extends GetMiddleware {
  @override
  RouteSettings? redirect(String? route) {
    final authController = Get.find<AuthController>();
    final user = authController.currentUser.value;
    if (user?.isAdmin != true) {
      return const RouteSettings(name: AppRoutes.home);
    }
    return null;
  }
}
```

---

## 🎯 Phase 10: Testing Strategy

### 10.1 Testing Structure

#### Unit Tests
```dart
// Test service classes
test/unit/
├── services/
│   ├── auth_service_test.dart
│   ├── feed_service_test.dart
│   ├── profile_service_test.dart
│   └── admin_service_test.dart
├── controllers/
│   ├── auth_controller_test.dart
│   ├── feed_controller_test.dart
│   └── profile_controller_test.dart
└── models/
    ├── user_model_test.dart
    ├── post_model_test.dart
    └── comment_model_test.dart
```

#### Widget Tests
```dart
// Test UI components
test/widget/
├── auth/
│   ├── phone_input_test.dart
│   ├── otp_verification_test.dart
│   └── login_page_test.dart
├── feed/
│   ├── post_card_test.dart
│   ├── feed_list_test.dart
│   └── create_post_test.dart
└── profile/
    ├── profile_page_test.dart
    └── edit_profile_test.dart
```

#### Integration Tests
```dart
// Test complete user flows
test/integration/
├── auth_flow_test.dart
├── post_creation_flow_test.dart
├── comment_flow_test.dart
└── admin_moderation_flow_test.dart
```

### 10.2 Mock Data & Services

#### Mock Services
```dart
class MockAuthService extends Mock implements AuthService {}
class MockFeedService extends Mock implements FeedService {}
class MockProfileService extends Mock implements ProfileService {}
```

---

## 📅 Implementation Timeline

### Week 1-2: Foundation
- ✅ Authentication system (phone-based)
- ✅ Basic UI components
- ✅ API integration setup
- ✅ State management setup

### Week 3-4: Core Features  
- ✅ Feed system implementation
- ✅ Post creation & interaction
- ✅ Comments system
- ✅ Basic profile management

### Week 5-6: Advanced Features
- ✅ Admin features & moderation
- ✅ Role-based access control
- ✅ Media upload system
- ✅ Error handling & offline support

### Week 7-8: Polish & Testing
- ✅ UI/UX improvements
- ✅ Performance optimization
- ✅ Comprehensive testing
- ✅ Bug fixes & stability

---

## 🚀 Deployment Preparation

### Production Checklist
- [ ] API endpoints configuration
- [ ] Authentication flow testing
- [ ] Role permissions validation
- [ ] Media upload testing
- [ ] Performance benchmarking
- [ ] Security audit
- [ ] User acceptance testing

### Configuration Management
```dart
class AppConfig {
  static const bool isProduction = bool.fromEnvironment('PRODUCTION');
  static const String apiBaseUrl = String.fromEnvironment(
    'API_BASE_URL',
    defaultValue: 'http://localhost:5000'
  );
}
```

---

## 🎯 Success Metrics

### Technical Metrics
- Build success rate: 100%
- Test coverage: >80%
- API response time: <2s
- App startup time: <3s
- Crash rate: <1%

### User Experience Metrics
- Login success rate: >95%
- Post creation success rate: >98%  
- Media upload success rate: >90%
- User engagement rate: Track after launch

### Performance Targets
- Feed loading: <2s
- Image upload: <10s
- Navigation smoothness: 60fps
- Memory usage: <200MB

---

## 📚 Additional Resources

### API Documentation
- [Backend API Specification](./integrate-api.md)
- [Authentication Endpoints](./ui-api-integration-spec.md)

### Design Guidelines
- [UI/UX Design System](../wireframes/tools/AGENT_VISUAL_WORKFLOW_GUIDE.md)
- [Component Library](./spec.md)

### Development Setup
- [Flutter Project Structure](../alumni_app/README.md)
- [Backend Integration Guide](../alumni_backend/README.md)

---

**Document Version**: v1.0  
**Last Updated**: October 3, 2025  
**Status**: Ready for Implementation