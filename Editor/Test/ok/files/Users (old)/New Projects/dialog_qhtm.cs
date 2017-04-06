\Dialog_Editor

dll "$program files$\gipsysoft\components\qhtm.dll" #QHTM_Initialize hInst
QHTM_Initialize _hinst

str controls = "3"
str qhtm3
qhtm3="<font color=#0000ff>hello <b>world</b></font>"
 IntGetFile "http://www.google.com" qhtm3
if(!ShowDialog("dialog_qhtm" 0 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QHTM_Window_Class_001 0x54030000 0x0 0 0 224 114 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020103 "" "" ""
