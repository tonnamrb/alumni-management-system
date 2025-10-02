# 📑 PRD – Alumni App (MVP v1)

## 1. Overview
ระบบ Alumni App เวอร์ชันแรก เป็นแอปพลิเคชันสำหรับศิษย์เก่า (Alumni) โดยโฟกัสการสร้าง **Home Feed + Profile แบบ Instagram style** เพื่อให้ผู้ใช้สามารถแชร์รูป/วิดีโอ, กดไลก์, คอมเมนต์ และดูโปรไฟล์ตัวเองได้

---

## 2. Scope
- Splash screen (5s loading bar)
- Register / Login (mock flow → เข้า Home ได้เลย)
- Role system (admin / user – default = user)
- Home Feed (mock data + IG-like interaction)
- Profile page (grid layout + profile feed)

---

## 3. Features & Requirements

### 3.1 Splash Screen
- แสดงโลโก้แอปพร้อม **progress bar (5 วินาที)**
- หลังโหลดเสร็จ → redirect ไปหน้า **Register/Login**

### 3.2 Authentication (MVP Mock)
- หน้า Register/Login
- ยังไม่ต้องเชื่อม backend จริง → ให้กด **Login → เข้า Home** ได้ทันที
- Role system  
  - `Admin` (reserved, ยังไม่เปิดใช้)  
  - `User` (default ทุกการล็อกอิน)  

### 3.3 Home Feed
- **Navbar (bottom)**:  
  - Icon Feed  
  - Icon Profile  
- **Feed List** (mock data)  
  - รูปภาพ/วิดีโอโพสต์ (mock assets)  
  - ฟังก์ชัน interaction:
    - Like (toggle)  
    - Comment (popup / inline)  
    - Upload (mock, เลือกรูป/วิดีโอจากเครื่อง)  
- **Behavior**:  
  - Scroll feed ได้แบบ IG  
  - Interaction กดแล้วเปลี่ยน state ได้ทันที (mock, no backend)

### 3.4 Profile Page
- แสดงข้อมูลโปรไฟล์ผู้ใช้ (mock):  
  - Avatar  
  - Username  
  - Bio (mock text)  
- **Grid Layout** (เหมือน IG)  
  - แสดงโพสต์ของผู้ใช้เป็น grid (รูป/วิดีโอ)  
- **Post Detail**  
  - เมื่อกดที่รูป/วิดีโอ → เปิด **Profile Feed Detail**  
  - layout คล้าย feed แต่แสดงเฉพาะโพสต์ของผู้ใช้  

---

## 4. Roles & Permissions
- **User (default)**  
  - เข้าสู่ระบบ → Home Feed, Profile  
  - ทำได้: like, comment, upload (mock)  
- **Admin (reserved, future)**  
  - Scope นอกเหนือ MVP  

---

## 5. Mock Data & Assets
- Feed จะใช้ **mock JSON data** เช่น:
```json
{
  "posts": [
    {
      "id": 1,
      "author": "User1",
      "media": "image1.jpg",
      "likes": 12,
      "comments": [
        {"user": "User2", "text": "Nice pic!"}
      ]
    }
  ]
}
