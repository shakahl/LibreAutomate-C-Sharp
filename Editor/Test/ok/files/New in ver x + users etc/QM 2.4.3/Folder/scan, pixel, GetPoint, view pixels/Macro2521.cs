int w=win("Microsoft Visual Studio" "wndclass_desked_gsk")
 scan "image:h6159C3E5" child("Solution Explorer" "SysTreeView32" w) 0 1|2|16 ;;outline
 scan "image:h6159C3E5" w 0 2|16 ;;outline
scan "image:h6159C3E5" w 0 3|16|0x1000 ;;outline
 scan "image:h6159C3E5" 0 0 2|16 ;;outline

str scanFiles=
 image:h4005160D
 image:h6159C3E5

 scan scanFiles child("Solution Explorer" "SysTreeView32" w) 0 1|2|16 ;;outline
 scan scanFiles child("Solution Explorer" "SysTreeView32" w) 0 1|2|16|0x800 ;;outline

 scan "color:0x75D6FA" child("Solution Explorer" "SysTreeView32" w) 0 1|2|16 ;;outline
