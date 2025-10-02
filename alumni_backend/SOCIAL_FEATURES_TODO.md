## Missing API Endpoints - Social Features Checklist

### ROLE: USER Features (ยังขาด API)

#### 1. Posts Management
- [ ] `POST /api/v1/posts` - Create new post
- [ ] `PUT /api/v1/posts/{id}` - Update own post
- [ ] `DELETE /api/v1/posts/{id}` - Delete own post
- [ ] `POST /api/v1/posts/{id}/like` - Toggle like post
- [ ] `POST /api/v1/posts/{id}/report` - Report post (description only)

#### 2. Comments Management  
- [ ] `POST /api/v1/posts/{id}/comments` - Add comment to post
- [ ] `POST /api/v1/comments/{id}/replies` - Reply to comment
- [ ] `POST /api/v1/comments/{id}/like` - Toggle like comment
- [ ] `DELETE /api/v1/comments/{id}` - Delete own comment

#### 3. Media Upload
- [ ] `POST /api/v1/upload/image` - Upload single image ✅ (มีแล้ว)
- [ ] `POST /api/v1/upload/album` - Upload multiple images
- [ ] `POST /api/v1/upload/video` - Upload video file

### ROLE: ADMIN Features (ยังขาด API)

#### 1. Content Moderation
- [ ] `DELETE /api/v1/admin/posts/{id}` - Delete any user's post
- [ ] `DELETE /api/v1/admin/comments/{id}` - Delete any user's comment

#### 2. Pin Management
- [ ] `POST /api/v1/admin/posts/{id}/pin` - Pin post (max 5, date ordered)
- [ ] `DELETE /api/v1/admin/posts/{id}/unpin` - Unpin post
- [ ] `GET /api/v1/admin/posts/pinned` - Get current pinned posts

#### 3. Admin Media Upload
- [ ] `POST /api/v1/admin/upload/media` - Upload media for admin posts

### Business Rules ที่ต้อง Implement:

#### Pin Posts Logic:
- Maximum 5 pinned posts at any time
- Pinned posts ordered by date (newest first)
- Always appear at top of feed and grid
- Only admins can pin/unpin

#### Permissions:
- Users can only delete their own posts/comments
- Admins can delete any posts/comments
- Report system stores description only

### Current Status:
✅ **Domain Entities**: Complete
✅ **Database Schema**: Complete  
❌ **API Controllers**: Missing most endpoints
❌ **Business Logic**: Missing implementation
❌ **Authorization**: Missing role-based permissions