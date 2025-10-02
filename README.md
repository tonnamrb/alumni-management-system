# 📝 คู่มือการขึ้น UC ใหม่ และการสั่ง Agent Build Flutter Project

## 1) การสร้าง UC ใหม่
1. ไปที่โฟลเดอร์ `specs/`  
   - สร้างไฟล์ใหม่ตามรูปแบบชื่อ: `UC-xx.md` (xx = เลข UC)  
   - ใช้ไฟล์ `UC-template.md` เป็นแม่แบบในการเขียน  

2. ไปที่โฟลเดอร์ `wireframes/`  
   - สร้างโฟลเดอร์ใหม่ตาม UC เช่น `UC-02/`  
   - ภายในต้องมี:
     - `SC/` → เก็บภาพหน้าจอ (Screen: SC-01.png, SC-02.png …)  
     - `WG/` → เก็บภาพ widget (Widget: WG-01.png, WG-02.png …)  
     - `wireframes-manifest.yml` (manifest ของ UC นั้น ๆ)  

3. Export wireframe จาก Figma เป็น `.png`  
   - ตั้งชื่อไฟล์ให้ตรงตามรหัส SC/WG ใน UC  

---

## 2) สร้างไฟล์ Manifest ใหม่
1. ที่ root ของ UC (เช่น `wireframes/UC-02/`)  
2. สร้างไฟล์ `wireframes-manifest.yml`  
3. เพิ่มข้อมูล SC และ WG ของ UC นั้น เช่น:

```yaml
screens:
  - id: SC-01
    name: Welcome
    path: SC/SC-01.png
  - id: SC-02
    name: Register by Email
    path: SC/SC-02.png

widgets:
  - id: WG-01
    name: Toast – อีเมลไม่ถูกต้อง
    path: WG/WG-01.png

## Prompt สำหรับสั่ง Agent
## ไฟล์ที่ใช้ Atth สำหรับ prompt 
## -------------------------------------------
## default.instructions.md / constitution-wireframe.md / specs/UC-xx.md / wireframes/UC-xx/wireframes-manifest.yml 
## Recheck รวมทั้งหมด 4 ไฟล์
## -------------------------------------------

## Mark : Prompt Start -------------------------------------------------------------------------
Continue UC-01.1 build
references:
  - constitution-wireframe.md
  - specs/UC-01.1.md
  - wireframes-manifest.yml

rules:
    - Do NOT create a new Flutter project (no `flutter create`)
    - Update-only inside project_root
    - Do NOT delete or overwrite non-agent files

Scope
- Splash screen must load first on app start.
- After Splash → navigate to SC-01 (the login/entry screen).
- Build ALL screens in manifest.screens (MUST-BUILD)
- Build ALL widgets in manifest.widgets (MUST-BUILD)
- Do NOT add anything beyond the manifest
- Respect SC-xx (screens) & WG-xx (widgets) codes
- Missing/uncertain spec → mark INSUFFICIENT_SPEC (no guessing)

Outputs
- /reports/coverage.json
- /reports/coverage.md  (each SC/WG: BUILT | SKIPPED | FAILED | INSUFFICIENT_SPEC)
- /reports/findings.json (errors only)
- /reports/insufficient_spec.json (warnings)
- /mapping/sc_wg_to_ac.json (Given/When/Then mapping)

Insufficient Spec
- OCR threshold: 0.85
- Action: mark INSUFFICIENT_SPEC and record details in /reports/insufficient_spec.json

Quality gates
- Use constitution-wireframe.md for all enforcement
- FAIL if any manifest SC/WG is not BUILT

## End Prompt -------------------------------------------------------------------------


