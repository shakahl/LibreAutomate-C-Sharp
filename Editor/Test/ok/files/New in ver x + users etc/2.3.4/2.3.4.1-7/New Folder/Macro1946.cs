/exe

 int w=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
  scan ":7 $my qm$\Macro1946.bmp" child("Solution Explorer" "SysTreeView32" w) 0 1|2|16 ;;outline
 scan ":7 Macro1946.bmp" child("Solution Explorer" "SysTreeView32" w) 0 1|2|16 ;;outline

int w=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
scan "macro:Macro1946.bmp" child("Solution Explorer" "SysTreeView32" w) 0 1|2|16 ;;outline

 BEGIN PROJECT
 main_function  Macro1946
 exe_file  $my qm$\Macro1946.qmm
 flags  22
 guid  {85657AFF-AF6C-4BDD-A856-82C87213BA18}
 END PROJECT
