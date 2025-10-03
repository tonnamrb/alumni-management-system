# Post Type Feature API Testing

## Overview
ระบบ alumni_backend ได้เพิ่มฟีเจอร์ Post Type เพื่อจำแนกประเภทโพสต์แล้ว:

## Post Types
- `0` = Text (เฉพาะข้อความ)  
- `1` = Image (รูปภาพเดี่ยว)
- `2` = Album (หลายรูป/อัลบั้ม)
- `3` = Video (วิดิโอ - อาจมีรูปหลายรูปด้วย)

## API Endpoints

### 1. GET Posts (with type filter)
```bash
# ดึงโพสต์ทั้งหมด
curl -X GET "http://localhost:5000/api/v1/posts?page=1&pageSize=10"

# ดึงเฉพาะโพสต์ข้อความ (type=0)
curl -X GET "http://localhost:5000/api/v1/posts?page=1&pageSize=10&type=0"

# ดึงเฉพาะโพสต์รูปภาพ (type=1)  
curl -X GET "http://localhost:5000/api/v1/posts?page=1&pageSize=10&type=1"

# ดึงเฉพาะโพสต์อัลบั้ม (type=2)
curl -X GET "http://localhost:5000/api/v1/posts?page=1&pageSize=10&type=2"

# ดึงเฉพาะโพสต์วิดิโอ (type=3)
curl -X GET "http://localhost:5000/api/v1/posts?page=1&pageSize=10&type=3"
```

### 2. Create Post (with type)
```bash
# สร้างโพสต์ข้อความ
curl -X POST "http://localhost:5000/api/v1/posts" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "content": "Hello World!",
    "type": 0
  }'

# สร้างโพสต์รูปภาพเดี่ยว
curl -X POST "http://localhost:5000/api/v1/posts" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "content": "Check out this image!",
    "type": 1,
    "imageUrl": "https://example.com/image.jpg"
  }'

# สร้างโพสต์อัลบั้ม (หลายรูป)
curl -X POST "http://localhost:5000/api/v1/posts" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "content": "My vacation photos",
    "type": 2,
    "mediaUrls": [
      "https://example.com/photo1.jpg",
      "https://example.com/photo2.jpg",
      "https://example.com/photo3.jpg"
    ]
  }'

# สร้างโพสต์วิดิโอ
curl -X POST "http://localhost:5000/api/v1/posts" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "content": "Amazing video with photos",
    "type": 3,
    "mediaUrls": [
      "https://example.com/video.mp4",
      "https://example.com/thumbnail1.jpg",
      "https://example.com/thumbnail2.jpg"
    ]
  }'
```

### 3. Update Post
```bash
# อัพเดทโพสต์ (เปลี่ยนจากข้อความเป็นรูปภาพ)
curl -X PUT "http://localhost:5000/api/v1/posts/1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "content": "Updated with image",
    "type": 1,
    "imageUrl": "https://example.com/updated-image.jpg"
  }'
```

## Response Format

### Post DTO Response
```json
{
  "success": true,
  "data": {
    "id": 1,
    "userId": 1,
    "userName": "John Doe",
    "userAvatar": "https://example.com/avatar.jpg",
    "content": "Hello World!",
    "type": 0,
    "imageUrl": null,
    "mediaUrls": [],
    "mediaCount": 0,
    "isPinned": false,
    "likesCount": 0,
    "commentsCount": 0,
    "isLikedByCurrentUser": false,
    "isOwnPost": true,
    "createdAt": "2025-10-02T15:10:02Z",
    "updatedAt": "2025-10-02T15:10:02Z"
  },
  "error": null
}
```

## Database Changes

### New Fields in Posts Table
- `Type` (integer): PostType enum value (0-3)
- `MediaUrls` (varchar(2000)): JSON array of media URLs
- New indexes: `IX_Posts_Type`, `IX_Posts_Type_CreatedAt`

### Migration Applied
- Migration: `20251002151002_AddPostTypeAndMediaUrls`
- Successfully applied to database

## Business Rules

### Post Type Validation
1. **Text (0)**: Can have no media or some media  
2. **Image (1)**: Must have exactly 1 image (imageUrl or mediaUrls[0])
3. **Album (2)**: Must have multiple media (mediaUrls.length > 1)
4. **Video (3)**: Must have at least 1 media (can include video + images)

### Media Handling
- Single image posts: Uses `imageUrl` field
- Multiple media posts: Uses `mediaUrls` JSON array
- `imageUrl` serves as primary/thumbnail for multi-media posts
- `mediaCount` provides quick access to total media count

## Implementation Complete ✅

✅ PostType enum created  
✅ Post entity updated with Type and MediaUrls fields  
✅ Database migration applied  
✅ Repository methods support type filtering  
✅ Service layer handles multiple media  
✅ API endpoints support type parameter  
✅ DTOs updated with new fields  
✅ Validation rules implemented  
✅ Business logic for media handling