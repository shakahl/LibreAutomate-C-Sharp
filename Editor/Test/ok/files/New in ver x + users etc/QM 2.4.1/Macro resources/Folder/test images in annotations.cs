int w=win("app - Microsoft Visual Studio" "wndclass_desked_gsk");

scan "image:hBE9C91E6" child("Solution Explorer" "SysTreeView32" w) 0 1|2|16 ;;outline
 comments
sgan "image:hA5B66F84" 0 0 1|2
if w
	scan "image:h1CCB37B1" 0 0 1|2; scan "image:hE0801C30" 0 0 1|2
	scan "image:h9CCE1BBA" 0 0 1|2
scan "image:hE0801C30" 0 0 1|2
int w1=win("SciLexer - Microsoft Visual Studio" "wndclass_desked_gsk")
scan "image:h7C18441B" child("Menu Bar" "MsoCommandBar" w1) 0 1|2|16 ;;menu bar 'Menu Bar'
scan "image:h9CCE1BBA" 0 0 1|2
scan "macro:Macro1946.bmp" child("Solution Explorer" "SysTreeView32" w) 0 1|2|16 ;;outline
if(!scan("macro:mspaint pencil pressed.bmp" child("" "NetUIHWND" w 0x1) 0 16 10)) ret
 "$qm$\il_de.bmp"
