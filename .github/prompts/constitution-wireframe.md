# 📑 Wireframe & Constitution (v3.2, Clean & Concise)

**Stop work if image reference is required. Always request SC/WG image files from user for layout/visual analysis before generating Flutter UI. If the user replies without attaching an image, continue based on specification only.**

---

## Wireframe Codes
- **SC (Screen Code)**: Full UI screen. Example: SC-01 (Onboarding), SC-02 (Sign up).  
- **WG (Widget Code)**: Sub-UI widget. Used for popup, toast, modal, inline error. Example: WG-01 (Popup), WG-02 (Toast).

---

## WG Rules

### Ownership
- Controlled only by **Orchestrator**.  
- SC/WG cannot open themselves.  
- No spam: only one instance at a time.

### Toast
- Single instance only. New replaces old.  
- No queue.  
- Merge similar messages.  
- Dismiss per spec.  
- Must use GetX snackbar (not Flutter Overlay).

### Modal
- One at a time. New replaces old.  
- Stack allowed only if AC/UC specifies:  
  - Visual hierarchy (z-index, dim levels).  
  - Back/ESC closes top first.  
  - Focus trap top layer.  
  - Max 2 layers unless otherwise stated.  
- Prefer state switch in one modal.  
- Focus returns correctly after close.  
- Close behavior per AC.

### Popup
- Merge similar messages.  
- Priority: critical > warning > info.  
- No rapid fire.

### Inline
- Update in-place only.  
- No queue. Show immediately.

---

## Specification Rules

### Type Required
Every WG must define type: `toast | popup | modal | inline`.

### Sub-elements
- `WG-xx.msg`: main message  
- `WG-xx.btn-<id>`: buttons per spec  
- `WG-xx.icon`: placeholder unless specified  
- `WG-xx.placement`: fixed (top, center-overlay, inline)

### Rendering
- toast: top, auto-dismiss  
- popup: overlay + actions  
- modal: overlay blocks interaction  
- inline: under related field

### Buttons
- Text, position, order per spec.  
- Actions only from AC. If missing → mark `INSUFFICIENT_SPEC`.

---

## Source Priority
Order: **AC > UC > Wireframe > Constitution**.  

Conflicts:  
- Label mismatch → `CONFLICT_LABEL`.  
- Always resolve per priority.

---

## Completeness & Order
- All SC must contain fields/buttons/WG as spec.  
  - Missing → `FIELD_MISSING`  
  - Extra → `FIELD_EXTRA`  
- Order: left→right, top→bottom. Mismatch → `ORDER_MISMATCH`.

---

## Acceptance Criteria Binding
- Map all behavior to AC.  
- Error handling uses WG defined by AC.  
- AC background actions count as flow.  
- If AC doesn’t define → render disabled + dev-toast `INSUFFICIENT_SPEC`.

---

## Multi-State / Multi-Tab Enforcement
- **SC multi-state**: many SC → 1 screen with states.  
- **SC multi-tab**: many SC → 1 screen with tabs.  
- **WG multi-state**: one widget with states (OTP idle/countdown/ready/error).  
- **WG multi-tab**: one widget with tabs.  
- Must render in one container.  
- meta.refs must include all related codes.

**Examples:**  
- SC-02~05 → Register (multi-state)  
- SC-06~07 → OTP (multi-state)  
- SC-09~10 → Add Profile (multi-tab)  

---

## WG Multi-State Model
- States: idle → active → ready → error → success/closed  
- Events: OPEN, TRIGGER, TICK, DONE, CLICK, FAIL, CLOSE  
- Only one primary CTA per state.  
- If AC undefined → render disabled + `INSUFFICIENT_SPEC`.

**OTP Example:**  
idle → countdown → ready → submitting → error/success

---

## Rendering Enforcement

### Strict SC
Render only requested SC. No contamination.

### Layout Safety (Critical)
🚨 **No layout crashes**.  
✅ Safe: Container, Scaffold+SafeArea+ScrollView, Column/Row with min size, Flexible not Expanded.  
❌ Dangerous: Unconstrained widgets, Expanded in ScrollView, missing physics.  

Scroll: AlwaysScrollable for main views. NeverScrollable only for nested.  
Test with small viewport.

### Fidelity
- Layout must match wireframe.  
- Columns centered ~360–420dp.  
- Do not change control type.  
- If in wireframe but not in AC → still render.  
- No extra elements.

### Back Control
- Only render if drawn.  
- If AppBar back → custom AppBar.  
- If inside screen → widget only.  
- System back allowed unless AC blocks.

---

## Dependency Injection (GetX)
- Each Page = `GetView<Controller>` with Binding.  
- Core service → `Get.put(permanent: true)`  
- Controller → `Get.lazyPut` (fenix if needed)  
- No Get.put/find inside Widget.build.

---

## Error Handling
- DI errors → log/dev-toast only.  
- Never expose dev codes to users.

---

## Reports
- `/reports/findings.json`: errors (FIELD_MISSING, BUTTON_MISSING, etc.)  
- `/reports/insufficient_spec.json`: warnings  
- Missing actions → disabled + dev-toast `INSUFFICIENT_SPEC`.

---

## Wireframe Analysis Integration
- Always request image if needed.  
- Visual analysis → 100% truth. No guessing.  
- Use `layout_helper.py` to generate Flutter structure.  
- Follow UC/AC/Wireframe strictly.  
- Compliance check before code.

---

## Specification-Driven Development
- Analyze flow diagram first.  
- Map SC to flow states.  
- Cross-check UC → AC → Wireframe.  
- Report inconsistencies.  
- Never invent content.

---

## Theme Compliance
- No hardcoded colors. Only theme variables.  
- Mandatory testing for every screen.

---

## QA Rules
- Process: Read spec → Analyze flow → Analyze wireframe → Cross-check → Code.  
- Errors: stop immediately, report, correct.

---

## Agent Working Rules
1. Read specs first.  
2. Request images.  
3. Visual analysis.  
4. Layout generation.  
5. Cross-check.  
6. Report.  
7. Implement.  
8. Validate.  

- Spec is highest authority.  
- No assumptions.  
- Conservative choices only.  
- Always cite source.  

---

## Safety & Penalties
- Must test UI before commit.  
- Layout crash = stop + fix immediately.  
- Retest mandatory after fixes.

---

## Error Handling
- No image → request from user.  
- Blurry → request clearer.  
- Missing manifest → use basic structure.  
- Spec conflict → report.  

🚨 Layout crash (e.g. “Cannot hit test render box”) = stop + fix immediately.  
