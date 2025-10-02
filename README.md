# 🎓 Alumni Management System

ระบบจัดการศิษย์เก่าที่ครอบคลุม ประกอบด้วย Mobile Application (Flutter) และ Backend API (.NET Core) สำหรับการจัดการข้อมูลศิษย์เก่า การโพสต์ข่าวสาร และการรายงานข้อมูล

## 📋 คุณสมบัติหลัก

### 📱 Mobile Application (Flutter)
- **Authentication**: เข้าสู่ระบบด้วย Email/Password และ JWT Token
- **Profile Management**: จัดการข้อมูลส่วนตัวและข้อมูลศิษย์เก่า
- **Social Features**: โพสต์ข่าวสาร, คอมเมนต์, ถูกใจ
- **Feed System**: ดูโพสต์และกิจกรรมต่างๆ
- **Multi-platform**: รองรับ Android, iOS, Web, และ Windows

### 🖥️ Backend API (.NET Core)
- **RESTful API**: API ที่สมบูรณ์สำหรับการจัดการข้อมูล
- **JWT Authentication**: ระบบความปลอดภัยด้วย JWT Token
- **Database Integration**: เชื่อมต่อกับ PostgreSQL
- **Reports System**: ระบบรายงานและส่งออกข้อมูล
- **Clean Architecture**: โครงสร้างโค้ดที่เป็นระเบียบและขยายได้

## 🏗️ โครงสร้างโปรเจค

```
alumni_v1/
├── alumni_app/                 # Flutter Mobile Application
│   ├── lib/
│   │   ├── app/               # App configuration & routes
│   │   ├── core/              # Core services & theme
│   │   ├── presentation/      # UI pages & controllers
│   │   └── shared/            # Shared models & widgets
│   ├── assets/                # Images, icons, i18n files
│   └── test/                  # Unit & widget tests
├── alumni_backend/            # .NET Core Backend API
│   ├── src/
│   │   ├── Api/              # API Controllers & endpoints
│   │   ├── Application/      # Business logic & services
│   │   ├── Domain/           # Domain entities & models
│   │   └── Infrastructure/   # Data access & external services
│   └── tests/                # Unit & integration tests
├── specs/                    # Project specifications
└── wireframes/              # UI wireframes & design tools
```

## 🚀 การติดตั้งและการใช้งาน

### Prerequisites
- **Flutter SDK** (3.0+)
- **.NET Core SDK** (8.0+)
- **PostgreSQL** (13+)
- **Git**

### 📱 Flutter App Setup
```bash
cd alumni_app
flutter pub get
flutter run
```

### 🖥️ Backend API Setup
```bash
cd alumni_backend/src/Api
dotnet restore
dotnet run
```

API จะทำงานที่: `https://localhost:5001`

### 🗄️ Database Setup
1. ติดตั้ง PostgreSQL
2. สร้าง database ใหม่
3. อัพเดท connection string ในไฟล์ `appsettings.json`
4. รัน migration:
```bash
cd alumni_backend/src/Infrastructure
dotnet ef database update
```

## 📊 API Documentation

API endpoints ที่สำคัญ:

### Authentication
- `POST /api/auth/login` - เข้าสู่ระบบ
- `POST /api/auth/register` - สมัครสมาชิก
- `POST /api/auth/refresh` - รีเฟรช token

### User Management
- `GET /api/users/profile` - ดูข้อมูลโปรไฟล์
- `PUT /api/users/profile` - อัพเดทโปรไฟล์
- `GET /api/users` - ดูรายชื่อผู้ใช้ (Admin)

### Posts & Social
- `GET /api/posts` - ดูโพสต์ทั้งหมด
- `POST /api/posts` - สร้างโพสต์ใหม่
- `POST /api/comments` - แสดงความคิดเห็น
- `POST /api/likes` - ถูกใจโพสต์

### Reports
- `GET /api/reports/users` - รายงานผู้ใช้
- `GET /api/reports/posts` - รายงานโพสต์
- `GET /api/reports/export` - ส่งออกรายงาน

## 🛠️ การพัฒนา

### Code Structure Guidelines
- ใช้ **GetX** สำหรับ state management ใน Flutter
- ใช้ **Clean Architecture** ในส่วน Backend
- ใช้ **Repository Pattern** สำหรับการเข้าถึงข้อมูล
- ใช้ **AutoMapper** สำหรับ object mapping

### Coding Standards
- ตั้งชื่อ variables/methods เป็นภาษาอังกฤษ
- เขียน comments เป็นภาษาไทย (ถ้าจำเป็น)
- ใช้ async/await pattern
- เขียน unit tests สำหรับ business logic

## 📝 Contributing

1. Fork repository นี้
2. สร้าง feature branch (`git checkout -b feature/amazing-feature`)
3. Commit การเปลี่ยนแปลง (`git commit -m 'Add amazing feature'`)
4. Push ไปยัง branch (`git push origin feature/amazing-feature`)
5. เปิด Pull Request

## 📄 License

โปรเจคนี้อยู่ภายใต้ MIT License - ดูรายละเอียดในไฟล์ [LICENSE](LICENSE)

## 👥 Team

- **Frontend (Flutter)**: Mobile application development
- **Backend (.NET)**: API และ database development
- **DevOps**: Deployment และ infrastructure

## 🔗 Links

- **Repository**: https://github.com/tonnamrb/alumni-management-system
- **API Documentation**: Coming soon
- **Live Demo**: Coming soon

---

📧 **Contact**: สำหรับข้อสงสัยเพิ่มเติม กรุณาติดต่อผ่าน GitHub Issues


