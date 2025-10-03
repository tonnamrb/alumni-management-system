# Deployment Guide: Alumni Backend ไป Staging Database

## ⚠️ สำคัญ: Staging Database Schema Integration

Staging database ถูกสร้างและจัดการโดยทีม backoffice แล้ว มีโครงสร้างและข้อมูล user อยู่แล้ว

### 🎯 ขั้นตอนการ Deploy

#### 1. **Selective Migration** บน Staging Environment

**⚠️ ปัญหา:** Staging มี `Users` และ `Roles` tables แล้ว แต่ขาด social features tables

**✅ แนวทาง:**
```bash
# Option A: Custom Migration Script (แนะนำ)
# สร้างเฉพาะ tables ที่ขาดหายไป
psql -h staging-host -U staging_user -d staging_alumni_db -f create_missing_tables.sql

# Option B: EF Core Migration with Conditional Logic  
ASPNETCORE_ENVIRONMENT=Staging dotnet ef database update --verbose

# Option C: Manual Database Setup
# ใช้เฉพาะเมื่อ Options อื่นไม่ได้ผล
```

#### 2. **Environment Variables สำหรับ Staging**
```bash
export ASPNETCORE_ENVIRONMENT=Staging
export ConnectionStrings__DefaultConnection="Host=staging-host;Port=5432;Database=staging_alumni_db;Username=staging_user;Password=staging_password"
```

#### 3. **การทดสอบ Connection**
```bash
# ทดสอบการเชื่อมต่อก่อน deploy
dotnet run --environment Staging
```

### 📋 Checklist ก่อน Deploy

#### Phase 1: Database Preparation
- [ ] ✅ ได้รับ connection string จากทีม backoffice
- [ ] ✅ ยืนยันว่า Users และ Roles tables มีอยู่แล้วใน staging
- [ ] ✅ รัน `create_missing_tables.sql` เพื่อสร้าง social features tables
- [ ] ✅ ทดสอบ connection และ table structures

#### Phase 2: Application Setup
- [ ] ✅ ทดสอบ Entity mappings กับ staging schema
- [ ] ✅ ทดสอบ authentication ด้วยข้อมูล user ที่มีอยู่
- [ ] ✅ ตั้งค่า environment เป็น "Staging"
- [ ] ✅ ยืนยัน JWT และ configuration settings

#### Phase 3: Feature Testing
- [ ] ✅ ทดสอบ user login/authentication
- [ ] ✅ ทดสอบ social features (posts, comments, likes)
- [ ] ✅ ทดสอบ admin functions

### 🔍 การทดสอบหลัง Deploy

1. **Test Database Connection & Tables**
```bash
# ทดสอบ API health
curl https://staging-api/health

# ตรวจสอบ tables (ใน database)
psql -h staging-host -U staging_user -d staging_alumni_db -c "
SELECT table_name, table_rows 
FROM information_schema.tables 
WHERE table_name IN ('Users', 'Roles', 'Posts', 'Comments', 'AlumniProfiles');"
```

2. **Test User Authentication**
```bash
curl -X POST https://staging-api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@example.com",
    "password": "12345678"
  }'
```

3. **Test Social Features**
```bash
# Create a test post
curl -X POST https://staging-api/posts \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "content": "Test post from API",
    "type": 1
  }'

# Get posts feed  
curl https://staging-api/posts \
  -H "Authorization: Bearer {token}"
```

### 🚨 หากเกิดปัญหา

1. **Connection Error**
   - ตรวจสอบ connection string
   - ยืนยันว่า database server accessible

2. **Schema Mismatch Error**
   - ตรวจสอบ Entity definitions
   - ยืนยันกับทีม backoffice เรื่อง schema changes

3. **Authentication Error**
   - ตรวจสอบ password hashing (BCrypt)
   - ยืนยัน user data ใน database

### 📞 Contact
- **Backend Team**: [Your Contact]
- **Backoffice Team**: [Backoffice Contact]
- **DevOps Team**: [DevOps Contact]