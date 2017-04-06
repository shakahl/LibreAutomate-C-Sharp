/exe
 \Dialog_Editor

sub.Test

#sub Test
str controls = "3"
str ax3SHD
if(!ShowDialog("" 0 &controls 0 0 0 0 0 0 0 "" "Macro2289")) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 96 48 "SHDocVw.WebBrowser"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040104 "*" "" "" ""

 BEGIN MENU
 >&File
	 &Test : 3 0 0 Cm
	 <
 END MENU

 BEGIN PROJECT
 main_function  Macro2289
 exe_file  $my qm$\Macro2289.qmm
 flags  7
 guid  {1371924B-BD6C-4600-9A9B-89C25093982E}
 END PROJECT
