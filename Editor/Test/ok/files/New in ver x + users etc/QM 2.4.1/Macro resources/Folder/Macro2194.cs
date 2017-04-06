 /exe
 int w=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
 scan "Macro2194.bmp" child("Menu Bar" "MsoCommandBar" w) 0 1|2|16 ;;menu bar 'Menu Bar'

 int w1=win("app" "CabinetWClass")
 scan "color:0x62E3FF" id(100 w1) 0 1|2|16 8 ;;outline 'Namespace Tree Control'

 int w=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
 scan "image:hFBDB67A2" child("Menu Bar" "MsoCommandBar" w) 0 1|2|16 ;;menu bar 'Menu Bar'

 Function1 "image:hFBDB67A2"

 int w=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
 scan "Macro2194.bmp" child("Menu Bar" "MsoCommandBar" w) 0 1|2|16 ;;menu bar 'Menu Bar'

 scan "Macro2194.bmp" 0 0 1|2

 int w=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
 scan "image:h75E892A1" child("Output" "GenericPane" w) 0 1|2 ;; 'Output'

 int w=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
 scan "image:kkkkkkkkk" child("Menu Bar" "MsoCommandBar" w) 0 1|2|16 ;;menu bar 'Menu Bar'

 int w=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
 scan "image:zzzzzzzzz" child("Menu Bar" "MsoCommandBar" w) 0 1|2|16 ;;menu bar 'Menu Bar'

 int w=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
 scan "image:aaaaaaaaaa" child("Menu Bar" "MsoCommandBar" w) 0 1|2|16 ;;menu bar 'Menu Bar'

 scan "aaaaaaaaaa.bmp" 0 0 1|2

Function1 "image:aaaaaaaaaa"

 BEGIN PROJECT
 main_function  Macro2194
 exe_file  $my qm$\Macro2194.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {038157A9-9F5D-472B-BD0C-80B11990D22F}
 END PROJECT

act "ddddddddddddd"
