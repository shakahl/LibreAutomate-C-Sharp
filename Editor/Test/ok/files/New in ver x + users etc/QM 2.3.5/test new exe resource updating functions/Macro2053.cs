 /exe
AddTrayIcon

int w=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
scan ":5 Macro2053.bmp" child("Menu Bar" "MsoCommandBar" w) 0 1|2|16 ;;menu bar 'Menu Bar'

5

 BEGIN PROJECT
 main_function  Macro2053
 exe_file  $my qm$\Macro2053.qmm
 icon  $system$\shell32.dll,21
 flags  23
 guid  {ACDE44F5-1515-49F2-9EB3-10FB307AC3B4}
 END PROJECT
