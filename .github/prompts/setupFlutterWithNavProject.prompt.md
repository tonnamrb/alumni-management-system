````prompt
---
mode: agent
---

# 🚀 Flutter Project Setup Guide with Navigation

สร้างโปรเจค Flutter (iOS/Android/Web) ใหม่ตามมาตรฐาน พร้อม Bottom Navigation Bar และ Settings

## 📋 Setup Requirements

**Project Info:**
- **Name**: ถามผู้ใช้จาก input chat
- **Package**: ใช้ snake_case จาก project name (เช่น my_awesome_app)
- **Platform**: iOS, Android, Web
- **Command**: `flutter create [project_name]` เสมอ

**Basic Dependencies:**
- GetX (state management + routing + DI)
- Dio (HTTP client)
- SharedPreferences, SecureStorage (local storage)
- flutter_i18n (internationalization)
- flutter_lints (code quality)

## ⚠️ Setup Rules

1. **Splash Screen**: ทุกครั้งเปิดแอปต้องมี Splash Screen ก่อนไปหน้าหลัก
2. **Navigation Flow**: Splash → Main (with BottomNavBar) → 5 tabs
3. **New Project Only**: สร้างโปรเจคใหม่เสมอ ห้ามแก้ไขโปรเจคเดิม
4. **Quality Check**: รัน `flutter analyze` และเช็คตาม `checklist.md`
5. **Follow Architecture**: ยึดตาม `flutter_default.instructions.md` และ `flutter_structure.instructions.md`
6. **Platform Testing**: Test บน mobile ก่อน web (Flutter Web debug มีปัญหา)

## 🗂️ Initial Project Structure with Navigation

สร้างโครงสร้างตาม `flutter_structure.instructions.md` พร้อม navigation features:

```
lib/
├── app/                # Entry point, routes, DI
├── core/               # Shared utilities, theme, network
│   └── services/       # Cross-feature services
│       ├── theme_service.dart    # Dark/Light mode
│       └── locale_service.dart   # Language management
├── shared/             # Reusable widgets
│   └── widgets/
│       ├── bottom_nav_bar/
│       │   └── bottom_nav_bar_widget.dart
│       └── app_bar/
│           └── settings_app_bar_widget.dart
├── presentation/       # All UI features
│   ├── splash/         # Required: Splash screen
│   ├── main_nav/       # Main navigation controller
│   ├── home/           # Tab 1: Home
│   ├── feed/           # Tab 2: Feed
│   ├── chat/           # Tab 3: Chat
│   ├── live/           # Tab 4: Live
│   ├── profile/        # Tab 5: Profile (with settings)
│   └── settings/       # Settings pages
│       ├── theme_settings/     # Dark/Light mode
│       └── language_settings/  # Language selection
└── main.dart
```

## 🧭 Navigation Requirements

**Bottom Navigation (5 Tabs):**
1. **Home** - หน้าแรก/Dashboard
2. **Feed** - ฟีดข้อมูล/โพสต์
3. **Chat** - แชท/ข้อความ
4. **Live** - ไลฟ์สด/สตรีม
5. **Profile** - โปรไฟล์ + Settings button

**Settings Features:**
- **Dark/Light Mode**: Switch toggle ใน Profile tab (AppBar)
- **Language**: Menu เลือกภาษา (TH/EN)
- **Location**: Settings button ที่ AppBar ของ Profile tab (ขวาบน)

## 🎨 UI Specifications

**Bottom Navigation:**
- ใช้ iOS-style design (ตาม theme.instructions.md)
- Icons: Home, Feed, Chat, Live, Profile
- Selected state with primary color
- Smooth transitions between tabs

**Settings AppBar:**
- แสดงใน Profile tab เท่านั้น
- Settings icon (gear/cog) ที่มุมขวาบน
- เมื่อกด: เปิด Settings Menu/BottomSheet

**Settings Menu:**
```
Settings
├── 🌙 Dark Mode        [Switch Toggle]
├── 🌍 Language         [TH/EN Selection] →
└── ... (extensible)
```

## 📱 User Flow

```
Splash Screen (2-3s)
    ↓
Main Navigation (Bottom Nav)
    ├── Home Tab
    ├── Feed Tab  
    ├── Chat Tab
    ├── Live Tab
    └── Profile Tab
            └── Settings (AppBar) →
                ├── Dark/Light Toggle
                └── Language Selection
```

## � Setup Checklist

### Basic Setup
1. ✅ Run `flutter create [project_name]`
2. ✅ Setup dependencies in `pubspec.yaml`
3. ✅ Create folder structure ตาม instructions
4. ✅ Setup theme ตาม `theme.instructions.md`

### Navigation Features
5. ✅ Create Splash Screen feature
6. ✅ Create MainNavigation controller (GetX)
7. ✅ Create BottomNavBar widget with 5 tabs
8. ✅ Create 5 tab pages (Home, Feed, Chat, Live, Profile)
9. ✅ Add Settings button in Profile AppBar
10. ✅ Create Settings pages (Theme, Language)

### Services & State Management
11. ✅ Create ThemeService (Dark/Light mode with GetX)
12. ✅ Create LocaleService (TH/EN with flutter_i18n)
13. ✅ Setup DI bindings for all controllers/services
14. ✅ Setup routing ด้วย GetX

### Quality Assurance
15. ✅ Setup i18n files (assets/i18n/en.yaml, th.yaml)
16. ✅ Test navigation between all tabs
17. ✅ Test theme switching
18. ✅ Test language switching
19. ✅ Run `flutter analyze` เช็ค errors
20. ✅ Test บน iOS/Android/Web

## 🌍 i18n Keys Required

**Navigation:**
```yaml
bottom_nav:
  home: "Home"
  feed: "Feed"  
  chat: "Chat"
  live: "Live"
  profile: "Profile"

settings:
  title: "Settings"
  dark_mode: "Dark Mode"
  language: "Language"
  
languages:
  thai: "ไทย"
  english: "English"
```

## 🎯 Key Implementation Notes

1. **MainNavController**: จัดการ currentIndex ของ BottomNavBar
2. **Persistent State**: แต่ละ tab เก็บ state ได้ (ไม่รีเซ็ตเมื่อสลับ)
3. **Theme Persistence**: เก็บ dark/light mode ใน SharedPreferences
4. **Locale Persistence**: เก็บภาษาที่เลือกใน SharedPreferences
5. **Settings Access**: สามารถเข้า Settings ได้จาก Profile tab เท่านั้น

## 📱 Platform Testing Priority

⚠️ **IMPORTANT**: Test บน mobile ก่อนเสมอ (Flutter Web debug มีปัญหา)

**Testing Order:**
1. **iOS Simulator First**: `flutter run -d "iPhone 16 Pro Max"`
2. **Android Second**: `flutter run -d "android"`  
3. **Web Last (Release)**: `flutter run -d chrome --release`

**Debug Issues:**
- Flutter Web debug mode มี DebugService errors ให้ใช้ release mode
- ถ้า splash ค้าง: Test บน iOS/Android ก่อน
- Web debug อาจไม่แสดง debugPrint() ให้ใช้ DevTools

## 🔧 Troubleshooting Splash Navigation

**ถ้าแอปค้างที่ splash screen:**
1. ✅ **Test บน iOS**: `flutter run -d "iPhone 16 Pro Max"`  
2. ✅ **ตรวจสอบ routes**: เช็ค `app_routes.dart` 
3. ✅ **ตรวจสอบ bindings**: เช็ค binding registration
4. ✅ **เพิ่ม debug logging**: 
   ```dart
   if (kDebugMode) {
     debugPrint('🚀 SplashController: Navigation step');
   }
   ```
5. ❌ **อย่าพึ่งพา web debug mode**: ใช้ mobile testing

---

**Reference:** Follow `.github/instructions/flutter_default.instructions.md`, `flutter_structure.instructions.md`, `flutter_debugging.instructions.md`, และ `flutter_defaultTheme.instructions.md`
````
