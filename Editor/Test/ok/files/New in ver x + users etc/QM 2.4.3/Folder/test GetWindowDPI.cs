 /exe
dll- user32 #GetWindowDPI hwnd

 int w=win("Registry Editor" "RegEdit_RegEdit")
int w=win("Quick Macros - ok - [Macro2590]" "QM_Editor")
outw w
out GetWindowDPI(w)

 BEGIN PROJECT
 main_function  Macro2590
 exe_file  $my qm$\Macro2590.qmm
 flags  6
 guid  {BA97A2F4-23F6-41D7-A460-3DB0DE856D13}
 END PROJECT
