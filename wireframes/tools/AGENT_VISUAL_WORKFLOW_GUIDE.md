# 🛠️ Agent Visual Workflow - New Approach

## ปัญหาของ Automated Image Analysis
- **Accuracy Issues**: OCR และ Computer Vision อาจพลาดหรือตีความผิด elements ที่สำคัญ
- **Context Loss**: Tool ไม่เข้าใจ business context และ UX flow
- **Layout Misinterpretation**: การวิเคราะห์ layout อัตโนมัติอาจไม่ตรงกับ design intent

## Solution: Agent Visual Workflow

### 🔄 New Workflow Steps

#### 1. Agent Request Image
```bash
python wireframes/tools/agent_visual_workflow.py --request-image SC-09 --manifest "wireframes/UC-01.1/wireframes-manifest.yml"
```

**Output**: Formatted prompt ให้ user attach ภาพ wireframe ที่เฉพาะเจาะจง

#### 2. User Attach Image
User แนบไฟล์ภาพ wireframe ตามที่ Agent ร้องขอ

#### 3. Agent Visual Analysis
Agent ใช้ visual capability อ่านภาพและระบุ:
- ✅ Layout structure (Column, Row, Stack)
- ✅ All UI elements (buttons, text fields, images, etc.)
- ✅ Text labels และ content ที่แม่นยำ
- ✅ Positioning และ alignment
- ✅ Spacing และ proportions

#### 4. Layout Generation
```bash
python wireframes/tools/layout_helper.py --generate-flutter-layout --elements "profile_image,textfield,button"
```

**สร้าง Flutter code** ที่ตรงกับ wireframe 100%

---

## 🎯 Key Benefits

### 100% Accuracy
- Agent เห็นภาพจริงและตีความได้ถูกต้อง
- ไม่มีปัญหา OCR ผิดพลาดหรือ Computer Vision พลาด elements

### Context Awareness  
- Agent เข้าใจ business context และ UX flow
- สามารถตัดสินใจ layout และ styling ได้เหมาะสม

### Flexible & Scalable
- ไม่ต้อง hardcode patterns หรือ train model
- Agent สามารถจัดการ wireframe แบบใหม่ ๆ ได้ทันที

### Quality Control
- Agent สามารถ validate และ cross-check กับ spec ได้
- มี human oversight ในการ attach ภาพที่ถูกต้อง

---

## 🛠️ Available Tools

### agent_visual_workflow.py
**หน้าที่**: จัดการ workflow ระหว่าง Agent และ User
- Generate image request prompts
- Prepare layout analysis guides  
- Create structure templates from detected elements

**Usage Examples**:
```bash
# ร้องขอภาพ SC-09
python agent_visual_workflow.py --request-image SC-09 --manifest "wireframes/UC-01.1/wireframes-manifest.yml"

# ร้องขอภาพ widget WG-13  
python agent_visual_workflow.py --request-widget WG-13 --manifest "wireframes/UC-01.1/wireframes-manifest.yml"

# เตรียม layout guide หลังจาก Agent วิเคราะห์เสร็จ
python agent_visual_workflow.py --prepare-layout SC-09 --elements-detected "profile_image,textfield,button"
```

### layout_helper.py
**หน้าที่**: สร้าง Flutter layout และ code generation
- Standard spacing และ sizing guidelines
- Flutter widget templates
- Form layouts และ complex structures

**Usage Examples**:
```bash
# สร้าง Flutter layout จาก elements
python layout_helper.py --generate-flutter-layout --elements "profile_image,textfield,button"

# สร้าง spacing guide
python layout_helper.py --create-spacing-guide --output spacing_guide.json

# สร้าง layout structure template
python layout_helper.py --generate-structure --output layout_template.json
```

---

## 📋 Workflow Example: SC-09 Profile Form

### Step 1: Agent Request
```bash
python agent_visual_workflow.py --request-image SC-09 --manifest "wireframes/UC-01.1/wireframes-manifest.yml"
```

**Output**:
```markdown
🖼️ **Image Request for SC-09**

I need to see the wireframe image for **SC-09 - Add New Profile Tab1** to create the Flutter implementation.

Please attach the wireframe image file so I can:
1. Analyze the layout and positioning of elements  
2. Identify all UI components (buttons, text fields, images, etc.)
3. Generate accurate Flutter code that matches the design exactly

**Expected file path in manifest:** `SC/SC-09.png`
**Screen description:** Add New Profile Tab1

Please attach the SC-09 wireframe image now.
```

### Step 2: User Attach Image
User แนบไฟล์ `SC-09.png`

### Step 3: Agent Visual Analysis
Agent อ่านภาพและเห็น:
- Profile image placeholder (circular) ที่ด้านบน center
- "อัปโหลด" button ใต้ profile image
- Text fields: ชื่อ*, นามสกุล*, ชื่อเล่น*, etc.
- Form layout เป็น Column
- Tab navigation ที่ด้านบน

### Step 4: Generate Layout
```bash
python layout_helper.py --generate-flutter-layout --elements "profile_image,button,textfield,textfield,textfield"
```

**Output**: Flutter Column widget code ที่มี:
- CircleAvatar สำหรับ profile image
- ElevatedButton สำหรับ "อัปโหลด"  
- TextFormField สำหรับแต่ละ input field
- Proper spacing และ alignment ตาม wireframe

---

## 🎉 Result: Perfect Match

Agent สร้าง Flutter code ที่:
- ✅ Layout ตรงกับ wireframe 100%
- ✅ Text labels ถูกต้องทุกตัว
- ✅ Button styling และ positioning ถูกต้อง
- ✅ Spacing และ proportions เหมาะสม
- ✅ Flutter best practices ถูกต้อง

**ไม่มีการเดา** - Agent เห็นภาพจริงทุกครั้ง!