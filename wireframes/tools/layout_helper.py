#!/usr/bin/env python3
"""
Layout Helper Tool - เครื่องมือช่วยจัด layout elements สำหรับ Flutter UI
เน้นการวาง positioning, spacing, alignment แทนการวิเคราะห์ภาพ

Usage:
    python layout_helper.py --generate-structure
    python layout_helper.py --create-spacing-guide
    python layout_helper.py --generate-flutter-layout --elements "button,textfield,image"
"""

import json
import argparse
from dataclasses import dataclass, asdict
from typing import List, Dict, Any, Optional
from pathlib import Path

@dataclass
class LayoutElement:
    """Element สำหรับการจัด layout"""
    type: str  # button, textfield, text, image, container, etc.
    id: str
    x: float = 0.0
    y: float = 0.0
    width: float = 100.0
    height: float = 40.0
    alignment: str = "center"  # left, center, right, top, bottom
    margin: Dict[str, float] = None
    padding: Dict[str, float] = None
    properties: Dict[str, Any] = None

    def __post_init__(self):
        if self.margin is None:
            self.margin = {"top": 8.0, "bottom": 8.0, "left": 16.0, "right": 16.0}
        if self.padding is None:
            self.padding = {"top": 12.0, "bottom": 12.0, "left": 16.0, "right": 16.0}
        if self.properties is None:
            self.properties = {}

@dataclass
class LayoutContainer:
    """Container สำหรับจัดกลุ่ม elements"""
    type: str  # column, row, stack, container, card
    elements: List[LayoutElement]
    spacing: float = 16.0
    alignment: str = "center"
    padding: Dict[str, float] = None

    def __post_init__(self):
        if self.padding is None:
            self.padding = {"top": 16.0, "bottom": 16.0, "left": 16.0, "right": 16.0}

class LayoutHelper:
    """Helper class สำหรับการจัด layout"""
    
    # Standard spacing values ตาม iOS design guidelines
    SPACING = {
        "xs": 4.0,
        "sm": 8.0,
        "md": 16.0,
        "lg": 24.0,
        "xl": 32.0,
        "xxl": 48.0
    }
    
    # Standard element sizes
    ELEMENT_SIZES = {
        "button": {"width": "match_parent", "height": 48.0},
        "textfield": {"width": "match_parent", "height": 56.0},
        "text": {"width": "wrap_content", "height": "wrap_content"},
        "image": {"width": 120.0, "height": 120.0},
        "profile_image": {"width": 80.0, "height": 80.0},
        "icon": {"width": 24.0, "height": 24.0},
        "card": {"width": "match_parent", "height": "wrap_content"}
    }
    
    @classmethod
    def create_element(cls, element_type: str, element_id: str, **kwargs) -> LayoutElement:
        """สร้าง LayoutElement ตาม type"""
        size = cls.ELEMENT_SIZES.get(element_type, {"width": 100.0, "height": 40.0})
        
        return LayoutElement(
            type=element_type,
            id=element_id,
            width=size["width"] if isinstance(size["width"], (int, float)) else 100.0,
            height=size["height"] if isinstance(size["height"], (int, float)) else 40.0,
            **kwargs
        )
    
    @classmethod
    def create_form_layout(cls, fields: List[Dict[str, str]]) -> LayoutContainer:
        """สร้าง form layout จาก list ของ fields"""
        elements = []
        
        for i, field in enumerate(fields):
            field_type = field.get("type", "textfield")
            field_id = field.get("id", f"field_{i}")
            field_label = field.get("label", "")
            
            # เพิ่ม label ถ้ามี
            if field_label:
                label_element = cls.create_element(
                    "text", 
                    f"{field_id}_label",
                    properties={
                        "text": field_label,
                        "style": "label",
                        "font_weight": "w600"
                    }
                )
                elements.append(label_element)
            
            # เพิ่ม field
            field_element = cls.create_element(
                field_type,
                field_id,
                properties=field.get("properties", {})
            )
            elements.append(field_element)
        
        return LayoutContainer(
            type="column",
            elements=elements,
            spacing=cls.SPACING["md"]
        )
    
    @classmethod
    def create_button_row(cls, buttons: List[Dict[str, str]]) -> LayoutContainer:
        """สร้าง row ของปุ่ม"""
        elements = []
        
        for button in buttons:
            button_element = cls.create_element(
                "button",
                button.get("id", "button"),
                properties={
                    "text": button.get("text", "Button"),
                    "style": button.get("style", "primary"),
                    **button.get("properties", {})
                }
            )
            elements.append(button_element)
        
        return LayoutContainer(
            type="row",
            elements=elements,
            spacing=cls.SPACING["md"],
            alignment="spaceEvenly" if len(buttons) > 1 else "center"
        )
    
    @classmethod
    def create_profile_header(cls, has_upload: bool = True) -> LayoutContainer:
        """สร้าง profile header layout (รูปโปรไฟล์ + upload button)"""
        elements = []
        
        # Profile image
        profile_image = cls.create_element(
            "profile_image",
            "profile_image",
            properties={
                "shape": "circle",
                "placeholder": "person_placeholder"
            }
        )
        elements.append(profile_image)
        
        # Upload button ถ้ามี
        if has_upload:
            upload_button = cls.create_element(
                "button",
                "upload_button",
                width=100.0,
                height=32.0,
                properties={
                    "text": "อัปโหลด",
                    "style": "secondary",
                    "icon": "upload"
                }
            )
            elements.append(upload_button)
        
        return LayoutContainer(
            type="column",
            elements=elements,
            spacing=cls.SPACING["sm"],
            alignment="center"
        )
    
    @classmethod
    def create_tab_layout(cls, tabs: List[str]) -> Dict[str, Any]:
        """สร้าง tab layout structure"""
        return {
            "type": "tab_container",
            "tabs": [
                {
                    "id": f"tab_{i}",
                    "title": tab,
                    "active": i == 0
                }
                for i, tab in enumerate(tabs)
            ],
            "properties": {
                "indicator_color": "primary",
                "label_style": "tab_label"
            }
        }
    
    @classmethod
    def generate_flutter_column(cls, container: LayoutContainer) -> str:
        """Generate Flutter Column widget code"""
        children = []
        
        for element in container.elements:
            if element.type == "textfield":
                widget_code = f"""TextFormField(
  decoration: InputDecoration(
    labelText: '{element.properties.get("label", "")}',
    hintText: '{element.properties.get("hint", "")}',
  ),
),"""
            elif element.type == "button":
                style = element.properties.get("style", "primary")
                button_type = "ElevatedButton" if style == "primary" else "OutlinedButton"
                widget_code = f"""{button_type}(
  onPressed: () {{}},
  child: Text('{element.properties.get("text", "Button")}'),
),"""
            elif element.type == "text":
                weight = element.properties.get("font_weight", "normal")
                style_suffix = f".copyWith(fontWeight: FontWeight.{weight})" if weight != "normal" else ""
                widget_code = f"""Text(
  '{element.properties.get("text", "")}',
  style: Theme.of(context).textTheme.bodyMedium{style_suffix},
),"""
            elif element.type == "profile_image":
                widget_code = f"""CircleAvatar(
  radius: {element.width / 2},
  backgroundImage: _profileImage != null 
    ? FileImage(_profileImage!) 
    : null,
  child: _profileImage == null 
    ? Icon(Icons.person, size: {element.width * 0.6}) 
    : null,
),"""
            else:
                widget_code = f"// TODO: Implement {element.type} widget"
            
            children.append(widget_code)
        
        spacing_widget = f"SizedBox(height: {container.spacing})," if container.spacing > 0 else ""
        children_str = f",\n      {spacing_widget}\n      ".join(children)
        
        return f"""Column(
  crossAxisAlignment: CrossAxisAlignment.stretch,
  children: [
      {children_str}
  ],
)"""

    @classmethod
    def generate_flutter_widgets(cls, layout_data: Dict[str, Any]) -> str:
        """Generate complete Flutter widget code from layout data"""
        # Extract layout containers
        containers = layout_data.get("containers", [])
        
        widget_code = []
        
        for container_data in containers:
            if isinstance(container_data, dict):
                container = LayoutContainer(**container_data)
                widget_code.append(cls.generate_flutter_column(container))
        
        return "\n\n".join(widget_code)

def main():
    parser = argparse.ArgumentParser(description="Layout Helper Tool")
    parser.add_argument("--generate-structure", action="store_true", help="Generate layout structure template")
    parser.add_argument("--create-spacing-guide", action="store_true", help="Create spacing guide")
    parser.add_argument("--generate-flutter-layout", action="store_true", help="Generate Flutter layout")
    parser.add_argument("--elements", help="Comma-separated list of elements")
    parser.add_argument("--output", help="Output file path")
    
    args = parser.parse_args()
    
    helper = LayoutHelper()
    
    if args.generate_structure:
        # Generate sample layout structure
        structure = {
            "screen_id": "SC-09",
            "layout_type": "form_with_tabs",
            "containers": [
                {
                    "type": "column",
                    "elements": [
                        asdict(helper.create_element("profile_image", "profile_image")),
                        asdict(helper.create_element("button", "upload_button", properties={"text": "อัปโหลด", "style": "secondary"})),
                        asdict(helper.create_element("textfield", "first_name", properties={"label": "ชื่อ*", "required": True})),
                        asdict(helper.create_element("textfield", "last_name", properties={"label": "นามสกุล*", "required": True})),
                    ],
                    "spacing": 16.0,
                    "alignment": "center",
                    "padding": {"top": 16.0, "bottom": 16.0, "left": 16.0, "right": 16.0}
                }
            ]
        }
        
        output = json.dumps(structure, indent=2, ensure_ascii=False)
        
        if args.output:
            Path(args.output).write_text(output, encoding='utf-8')
            print(f"Layout structure saved to {args.output}")
        else:
            print(output)
    
    elif args.create_spacing_guide:
        # Create spacing guide
        guide = {
            "spacing_values": helper.SPACING,
            "element_sizes": helper.ELEMENT_SIZES,
            "usage_examples": {
                "form_spacing": "Use 'md' (16px) between form fields",
                "button_spacing": "Use 'sm' (8px) between buttons in a row",
                "section_spacing": "Use 'lg' (24px) between major sections",
                "container_padding": "Use 'md' (16px) for container padding"
            }
        }
        
        output = json.dumps(guide, indent=2, ensure_ascii=False)
        
        if args.output:
            Path(args.output).write_text(output, encoding='utf-8')
            print(f"Spacing guide saved to {args.output}")
        else:
            print(output)
    
    elif args.generate_flutter_layout and args.elements:
        # Generate Flutter layout for specified elements
        elements = args.elements.split(",")
        
        # Create sample layout based on elements
        layout_elements = []
        for i, element_type in enumerate(elements):
            element = helper.create_element(
                element_type.strip(),
                f"{element_type.strip()}_{i}",
                properties={"label": f"Sample {element_type}", "text": f"Sample {element_type}"}
            )
            layout_elements.append(element)
        
        container = LayoutContainer(
            type="column",
            elements=layout_elements,
            spacing=helper.SPACING["md"]
        )
        
        flutter_code = helper.generate_flutter_column(container)
        
        if args.output:
            Path(args.output).write_text(flutter_code, encoding='utf-8')
            print(f"Flutter layout code saved to {args.output}")
        else:
            print(flutter_code)
    
    else:
        parser.print_help()

if __name__ == "__main__":
    main()