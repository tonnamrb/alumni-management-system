#!/usr/bin/env python3
"""
Agent Visual Workflow Tool - เครื่องมือช่วย Agent ในการร้องขอภาพและจัดการ workflow

เมื่อ Agent ต้องการสร้างหน้าจอ SC-XX หรือ WG-XX:
1. Agent ใช้ tool นี้เพื่อร้องขอให้ user attach ภาพ
2. Agent วิเคราะห์ภาพด้วย visual capability
3. Agent ใช้ layout_helper.py เพื่อสร้าง Flutter code

Usage:
    python agent_visual_workflow.py --request-image SC-09
    python agent_visual_workflow.py --prepare-layout SC-09 --elements-detected "profile_image,textfield,button"
"""

import json
import argparse
from pathlib import Path
from typing import List, Dict, Any, Optional
import yaml

class AgentVisualWorkflow:
    """Helper class สำหรับ Agent visual workflow"""
    
    def __init__(self, manifest_path: Optional[str] = None):
        self.manifest_path = manifest_path
        self.manifest_data = None
        
        if manifest_path and Path(manifest_path).exists():
            with open(manifest_path, 'r', encoding='utf-8') as f:
                self.manifest_data = yaml.safe_load(f)
    
    def get_screen_info(self, screen_id: str) -> Dict[str, Any]:
        """ดึงข้อมูล screen จาก manifest"""
        if not self.manifest_data:
            return {"error": "No manifest loaded"}
        
        # หา screen ใน manifest
        for screen in self.manifest_data.get("screens", []):
            if screen["id"] == screen_id:
                return screen
        
        return {"error": f"Screen {screen_id} not found in manifest"}
    
    def get_widget_info(self, widget_id: str) -> Dict[str, Any]:
        """ดึงข้อมูล widget จาก manifest"""
        if not self.manifest_data:
            return {"error": "No manifest loaded"}
        
        # หา widget ใน manifest
        for widget in self.manifest_data.get("widgets", []):
            if widget["id"] == widget_id:
                return widget
        
        return {"error": f"Widget {widget_id} not found in manifest"}
    
    def generate_image_request_prompt(self, screen_id: str) -> str:
        """สร้าง prompt สำหรับร้องขอภาพจาก user"""
        screen_info = self.get_screen_info(screen_id)
        
        if "error" in screen_info:
            return f"""
🖼️ **Image Request for {screen_id}**

I need to see the wireframe image for **{screen_id}** to create the Flutter implementation.

Please attach the wireframe image file for {screen_id} so I can:
1. Analyze the layout and positioning of elements
2. Identify all UI components (buttons, text fields, images, etc.)
3. Generate accurate Flutter code that matches the design

**Expected file:** {screen_id}.png or {screen_id}.jpg

Once you provide the image, I'll use my visual capabilities to analyze it and create the appropriate Flutter widgets with correct positioning and styling.
"""
        
        screen_name = screen_info.get("name", screen_id)
        expected_path = screen_info.get("path", f"{screen_id}.png")
        
        return f"""
🖼️ **Image Request for {screen_id}**

I need to see the wireframe image for **{screen_id} - {screen_name}** to create the Flutter implementation.

Please attach the wireframe image file so I can:
1. Analyze the layout and positioning of elements  
2. Identify all UI components (buttons, text fields, images, etc.)
3. Generate accurate Flutter code that matches the design exactly

**Expected file path in manifest:** `{expected_path}`
**Screen description:** {screen_name}

Once you provide the image, I'll use my visual capabilities to:
- ✅ Read all text labels and button names accurately
- ✅ Identify the correct layout structure (Column, Row, Stack)
- ✅ Detect proper spacing and alignment
- ✅ Generate Flutter code with 100% accuracy to your wireframe

Please attach the {screen_id} wireframe image now.
"""
    
    def generate_widget_request_prompt(self, widget_id: str) -> str:
        """สร้าง prompt สำหรับร้องขอภาพ widget"""
        widget_info = self.get_widget_info(widget_id)
        
        if "error" in widget_info:
            return f"""
🖼️ **Widget Image Request for {widget_id}**

I need to see the wireframe image for **{widget_id}** to create the Flutter widget implementation.

Please attach the wireframe image file for {widget_id} so I can analyze the design and generate the appropriate Flutter widget code.
"""
        
        widget_name = widget_info.get("name", widget_id)
        expected_path = widget_info.get("path", f"{widget_id}.png")
        
        return f"""
🖼️ **Widget Image Request for {widget_id}**

I need to see the wireframe image for **{widget_id} - {widget_name}** to create the Flutter widget implementation.

Please attach the wireframe image file so I can:
1. Analyze the widget design and layout
2. Identify the correct styling and positioning
3. Generate accurate Flutter widget code

**Expected file path in manifest:** `{expected_path}`
**Widget description:** {widget_name}

Please attach the {widget_id} wireframe image now.
"""
    
    def prepare_layout_analysis_guide(self, screen_id: str) -> Dict[str, Any]:
        """เตรียม guide สำหรับ Agent ในการวิเคราะห์ layout"""
        return {
            "screen_id": screen_id,
            "analysis_checklist": [
                "Identify the main layout type (Column, Row, Stack, or custom)",
                "List all UI elements from top to bottom, left to right",
                "Note the positioning and alignment of each element",
                "Identify spacing between elements",
                "Check for any tabs or multi-state components",
                "Note any special styling or decorations",
                "Identify required vs optional fields",
                "Check for any icons or images"
            ],
            "common_elements_to_look_for": [
                "Profile image (circular)",
                "Upload buttons",
                "Text fields with labels",
                "Required field indicators (*)",
                "Primary/secondary buttons",
                "Tab navigation",
                "Form sections",
                "Headers and titles"
            ],
            "flutter_mapping": {
                "circular_image": "CircleAvatar",
                "text_field": "TextFormField",
                "button_primary": "ElevatedButton", 
                "button_secondary": "OutlinedButton",
                "text_label": "Text with style",
                "column_layout": "Column",
                "row_layout": "Row",
                "tabs": "TabBar + TabBarView"
            }
        }
    
    def generate_layout_structure_template(self, elements_detected: List[str]) -> Dict[str, Any]:
        """สร้าง template structure จาก elements ที่ Agent detect ได้"""
        containers = []
        
        # Group elements into logical containers
        current_container = {
            "type": "column",
            "elements": [],
            "spacing": 16.0,
            "alignment": "stretch"
        }
        
        for element in elements_detected:
            element_config = {
                "type": element,
                "id": f"{element}_{len(current_container['elements'])}",
                "properties": {}
            }
            
            # Add element-specific properties
            if element == "profile_image":
                element_config["properties"] = {
                    "shape": "circle",
                    "size": 80,
                    "placeholder": "person"
                }
            elif element == "textfield":
                element_config["properties"] = {
                    "label": "Field Label*",
                    "required": True,
                    "validation": "required"
                }
            elif element == "button":
                element_config["properties"] = {
                    "text": "Button Text",
                    "style": "primary"
                }
            
            current_container["elements"].append(element_config)
        
        containers.append(current_container)
        
        return {
            "layout_structure": containers,
            "next_steps": [
                "Agent should replace placeholder values with actual content from the wireframe",
                "Adjust spacing and alignment based on visual analysis",
                "Add specific styling and properties as needed",
                "Generate Flutter code using layout_helper.py"
            ]
        }

def main():
    parser = argparse.ArgumentParser(description="Agent Visual Workflow Tool")
    parser.add_argument("--request-image", help="Generate image request prompt for screen/widget ID")
    parser.add_argument("--request-widget", help="Generate widget image request prompt")
    parser.add_argument("--prepare-layout", help="Prepare layout analysis guide for screen ID")
    parser.add_argument("--elements-detected", help="Comma-separated list of detected elements")
    parser.add_argument("--manifest", help="Path to wireframes manifest file")
    parser.add_argument("--output", help="Output file path")
    
    args = parser.parse_args()
    
    workflow = AgentVisualWorkflow(args.manifest)
    
    if args.request_image:
        prompt = workflow.generate_image_request_prompt(args.request_image)
        
        if args.output:
            Path(args.output).write_text(prompt, encoding='utf-8')
            print(f"Image request prompt saved to {args.output}")
        else:
            print(prompt)
    
    elif args.request_widget:
        prompt = workflow.generate_widget_request_prompt(args.request_widget)
        
        if args.output:
            Path(args.output).write_text(prompt, encoding='utf-8')
            print(f"Widget request prompt saved to {args.output}")
        else:
            print(prompt)
    
    elif args.prepare_layout:
        guide = workflow.prepare_layout_analysis_guide(args.prepare_layout)
        
        if args.elements_detected:
            elements = [e.strip() for e in args.elements_detected.split(",")]
            layout_template = workflow.generate_layout_structure_template(elements)
            guide["layout_template"] = layout_template
        
        output = json.dumps(guide, indent=2, ensure_ascii=False)
        
        if args.output:
            Path(args.output).write_text(output, encoding='utf-8')
            print(f"Layout analysis guide saved to {args.output}")
        else:
            print(output)
    
    else:
        parser.print_help()

if __name__ == "__main__":
    main()