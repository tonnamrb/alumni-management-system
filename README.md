# ระบบจัดการศิษย์เก่า (Alumni Management Platform)

โปรเจกต์นี้พัฒนาระบบดูแลศิษย์เก่าแบบครบวงจร ประกอบด้วยแอปมือถือ/เว็บที่สร้างด้วย Flutter และ Backend API ด้วย .NET 8 เพื่อรองรับการยืนยันตัวตนด้วยเบอร์โทรศัพท์ การจัดการโปรไฟล์ ฟีดกิจกรรม และการซิงก์ข้อมูลจากระบบภายนอก

## งานที่ทำแล้ว

### Mobile App (Flutter)
- วางโครงสร้างเลเยอร์ app/core/domain/data/presentation/shared เพื่อรองรับ Clean Architecture ฝั่งโมบายล์
- ตั้งค่า GetX สำหรับ dependency injection, state management และ routing (`AppInjection`, `AppRoutes`)
- พัฒนาฟลว์ยืนยันตัวตนด้วยเบอร์โทรศัพท์ครบทั้ง login, OTP, password setup และ session (`AuthController`, `AuthRepositoryImpl`)
- ออกแบบสกรีนหลัก เช่น splash, การนำทางแบบ bottom tabs และฟีดโซเชียลโดยใช้ mock data พร้อม action กด like/comment
- จัดการธีมและภาษาผ่าน `ThemeService`, `LocaleService` และระบบโหลดไฟล์ i18n บน `GetMaterialApp`

### Backend API (.NET 8)
- จัดโครงสร้าง Clean Architecture ครบสี่เลเยอร์ (Domain/Application/Infrastructure/Api) พร้อมระบบ Dependency Injection
- เสริม Mobile Authentication ชุด OTP สำหรับเบอร์ไทย รวมการ normalize หมายเลข, DTO และ controller `/api/v1/auth`
- พัฒนา External Data Integration service ครบชุดสำหรับ bulk import, validation, single sync และสถิติสรุป
- เพิ่ม migration ปรับ schema ผู้ใช้, สร้างดัชนีประสิทธิภาพ และอัปเดต repository ให้รองรับ pagination
- เขียน unit test 41 รายการครอบคลุม business logic หลัก พร้อมสคริปต์ `test-api.ps1` สำหรับรันอัตโนมัติ

### Database & DevOps
- เตรียมสคริปต์ SQL เช่น `create_missing_tables.sql` และ `add_test_user.sql` สำหรับตั้งค่าข้อมูลเริ่มต้นและการทดสอบ
- จัดทำเอกสารสรุปงาน migration, ขั้นตอนดีพลอย และคู่มือฟีเจอร์เพื่อเตรียมสภาพแวดล้อม staging
- สร้างสคริปต์ PowerShell (`start-api.ps1`, `validate_migration.ps1`) ช่วยรัน API, ทดสอบ และตรวจสอบฐานข้อมูล
- กำหนดค่าคอนฟิกหลายสภาพแวดล้อมผ่าน `appsettings.{Environment}.json` พร้อมตัวอย่าง connection string สำหรับ PostgreSQL

## ฟีเจอร์หลัก

### การยืนยันตัวตนด้วยมือถือ
- รองรับสมัครสมาชิกและเข้าสู่ระบบด้วยเบอร์โทรศัพท์ โดยมี OTP verification ครบขั้นตอน
- มีฟอร์ม login/OTP/password setup ฝั่ง Flutter พร้อมการ validate เบอร์ไทยและแสดงผลสถานะ
- Backend มีบริการ `AuthenticationService` จัดการ normalize หมายเลข, ตรวจสอบ OTP และออก JWT token

### การจัดการโปรไฟล์และเซสชัน
- GetX controller จัดการสถานะผู้ใช้ปัจจุบัน, เก็บ token และข้อมูลโปรไฟล์ผ่าน `UserSessionService`
- มีสกรีนโปรไฟล์และการนำทางเข้าสู่รายละเอียดศิษย์เก่า (mock data) เพื่อเตรียมเชื่อมต่อ backend
- Endpoint `/api/v1/auth/profile` ส่งข้อมูลผู้ใช้หลังยืนยันตัวตนเรียบร้อย

### ฟีดกิจกรรมและโซเชียล
- ฟีดตัวอย่างพร้อมโพสต์, คอมเมนต์, การกดถูกใจ และตอบกลับ โดยรองรับ toggle like/comment ผ่าน GetX
- เตรียมโมเดลโพสต์ (`PostModel`, `CommentModel`) และ controller สำหรับเชื่อม API จริงในลำดับต่อไป
- Backend มีโครงสร้าง controller (`PostsController`, `CommentsController`, `ReportsController`) และ service สำหรับต่อยอด

### การซิงก์ข้อมูลภายนอก
- `ExternalDataIntegrationService` รองรับ bulk import, การตรวจสอบข้อมูล, sync รายคน และสถิติความคืบหน้า
- ใช้ FluentValidation และ helper เฉพาะสำหรับตรวจสอบรูปแบบข้อมูลก่อนบันทึกลงระบบ
- มีการจัดการ duplicate, normalization หมายเลขโทรศัพท์ และเขียน log/สถิติการซิงก์

### การรายงานและสิทธิ์การเข้าถึง
- ระบบกำหนด Role (Admin, SystemIntegrator) สำหรับงานดูแลข้อมูลและสร้างรายงาน
- เพิ่มดัชนีฐานข้อมูลเพื่อรองรับการค้นหาผู้ใช้, โปรไฟล์, และโพสต์อย่างรวดเร็ว
- เตรียม controller ฝั่ง Admin สำหรับตรวจคำร้อง, จัดการโพสต์ และสถิติการรายงาน

## สถาปัตยกรรม

### Mobile App
- `app/` กำหนด dependency injection, routes และ environment configuration
- `core/` รวม service ระดับระบบ เช่น theme, locale และ user session
- `domain/` เก็บ entity, error model และ interface ของ repository ตามหลัก Clean Architecture
- `data/` จัดการ data source, dio client, mapper และ implementation ของ repository
- `presentation/` แยกสกรีนตามโมดูล (auth/feed/profile/main_nav/splash) พร้อม bindings
- `shared/` รวม widget, model และยูทิลิตีที่ใช้ข้ามโมดูล

### Backend API
- `Domain` ประกาศ entity, value object และกฎธุรกิจพื้นฐาน
- `Application` รวม DTO, Services, CQRS Commands/Queries และ FluentValidation
- `Infrastructure` ดูแล EF Core DbContext, Repository, การเชื่อมต่อภายนอก (AWS S3, Email, JWT)
- `Api` ให้บริการ controller, middleware, response helper และตั้งค่า Swagger/DI
- `tests/` เก็บ unit/integration tests พร้อมแฟ้ม fixture และ runner เฉพาะทาง
- การ register dependency ถูกแยกผ่าน `DependencyInjection.cs` ในแต่ละเลเยอร์เพื่อความชัดเจน

### Cross-System Practices
- ใช้ JWT เป็นมาตรฐาน authentication ระหว่างแอปกับ API และเตรียม refresh token flow สำหรับการต่อยอด
- สร้างรูปแบบ response เดียวผ่าน `ApiResponseHelper` และ mapper ฝั่งแอป (`Result<AuthResult, AppError>`)
- คุม environment configuration ผ่าน `EnvConfigImpl` (Flutter) และ `appsettings.*` (Backend) เพื่อสลับ dev/staging/prod
- จัดการ error แบบมีชนิดผ่าน `AppError`, FluentValidation และ middleware ดักจับข้อผิดพลาดกลาง

## เทคโนโลยีหลัก
- Flutter 3, Dart, GetX, Dio, intl_phone_field, flutter_i18n
- ASP.NET Core 8 Web API, MediatR, AutoMapper, FluentValidation
- Entity Framework Core, PostgreSQL 13+, และสคริปต์ EF migrations
- PowerShell tooling สำหรับการรัน/ทดสอบ และ AWS S3 service stub สำหรับอัปโหลดสื่อ
- ระบบทดสอบ: xUnit + FluentAssertions สำหรับ backend, widget/unit test scaffold บน Flutter

## โครงสร้างโปรเจกต์โดยย่อ

```
alumni_v1/
├─ alumni_app/
│  ├─ lib/
│  │  ├─ app/
│  │  ├─ core/
│  │  ├─ data/
│  │  ├─ domain/
│  │  ├─ presentation/
│  │  └─ shared/
│  └─ assets/i18n
├─ alumni_backend/
│  ├─ src/
│  │  ├─ Api/
│  │  ├─ Application/
│  │  ├─ Domain/
│  │  └─ Infrastructure/
│  ├─ tests/
│  └─ *.md, *.ps1
├─ specs/
├─ wireframes/
└─ add_test_user.sql
```

## การติดตั้งและการรัน

### Backend API
```powershell
cd alumni_backend/src/Api
dotnet restore
dotnet build
```
ตั้งค่า connection string ใน `appsettings.Development.json` แล้วรัน migration:
```powershell
cd ..\Infrastructure
dotnet ef database update
```
กลับมาที่โฟลเดอร์ `Api` เพื่อรันบริการ:
```powershell
cd ..\Api
dotnet run
```
ค่าเริ่มต้นจะเปิดที่ `https://localhost:5001` (HTTPS) และ `http://localhost:5000` (HTTP)

### Flutter App
```powershell
cd alumni_app
flutter pub get
flutter run --dart-define=ENVIRONMENT=development
```
หากรันบน Android Emulator ให้ตรวจสอบว่า backend เปิดที่ `http://10.0.2.2:5000` ตามค่าใน `EnvConfigImpl`

## สภาพแวดล้อมและข้อมูลทดสอบ
- กำหนด environment ของ Flutter ผ่านค่า `--dart-define=ENVIRONMENT=...` เพื่อสลับ dev/staging/prod
- ตัวอย่างผู้ใช้ทดสอบสามารถสร้างด้วย `add_test_user.sql` (ต้องตั้งค่า `PasswordHash` ให้ตรงกับกลไก hashing ใน backend)
- สคริปต์ `validate_migration.ps1` จะช่วยตรวจสอบ schema และจำนวนตารางหลังจากรัน migration
- Backend รองรับค่า secret เพิ่มเติมใน `appsettings.Development.json` เช่น JWT Key, S3 credential, SMTP สำหรับทดสอบ

## รายละเอียดเพิ่มเติม
- สรุปงานที่เสร็จแล้วของ Mobile Authentication และ External Data Integration รวมถึงรายละเอียดของ OTP flow, service ฝั่ง backend และ mock data บนแอป
- สรุปรายละเอียด migration พร้อมลำดับขั้นการสร้าง schema, การตรวจสอบ post type และคำแนะนำการตรวจสอบผลลัพธ์หลังรัน
- อธิบายขั้นตอนการเตรียมค่า environment, การตั้งค่า secret และกระบวนการดีพลอยขึ้นสภาพแวดล้อม staging แบบเป็นขั้นตอน
- ให้โรดแมปฟีเจอร์โซเชียลที่กำลังพัฒนา รวมถึงประเภทโพสต์, interaction ที่วางแผนเพิ่ม และรายการงานที่ต้องทำต่อ

## แผนถัดไป
- เชื่อมต่อ Flutter feed กับ API จริง แทนการใช้ mock data และเพิ่ม state management สำหรับโพสต์แบบเรียลไทม์
- สร้าง integration tests และ performance tests สำหรับเส้นทาง `/api/v1/auth` และ `/api/v1/external-data`
- ทำให้ refresh token flow สมบูรณ์ทั้งใน backend และฝั่ง Flutter (`AuthRepository.refreshToken`)
- เพิ่มเอกสาร OpenAPI/Swagger และคู่มือผู้ดูแลระบบสำหรับการซิงก์ข้อมูล
- ขยาย DevOps pipeline เพื่อรวมการ deploy อัตโนมัติและระบบ monitoring
