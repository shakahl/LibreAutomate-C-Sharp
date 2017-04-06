 int h=OnScreenDisplay("Text" 5.5 -1 0 "" 50 0xff 0 "osd1")
 1
 clo h

 OnScreenDisplay "Text"
 OnScreenDisplay "Line1[]Line2[]Line3" 1.5 0 -1 "" 50 0xff0000 0 "osd1"

 int i
 for(i 1 1000 1) OnScreenDisplay "text" 1 i -1 "" 0 0 1 "osd1"; 0.01
