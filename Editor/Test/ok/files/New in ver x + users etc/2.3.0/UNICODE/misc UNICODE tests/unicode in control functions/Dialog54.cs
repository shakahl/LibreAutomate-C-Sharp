/exe
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4 5"
str cb4 lb5
cb4="ᶚᶚᶚᶚᶚ[]ᶚᶚᶚᶚᶚ"
lb5="ᶚᶚᶚᶚᶚ[]ᶚᶚᶚᶚᶚ"
if(!ShowDialog("Dialog54" &Dialog54 &controls)) ret
out cb4
out lb5

 BEGIN DIALOG
 1 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 msctls_statusbar32 0x54030000 0x0 0 121 223 14 ""
 4 ComboBox 0x54230643 0x0 6 8 96 213 ""
 5 ListBox 0x54230101 0x200 106 8 96 48 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030000 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SendMessage id(3 hDlg) SB_SETTEXTW 0 @"ᶚᶚᶚᶚ"
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1


 BEGIN PROJECT
 main_function  Dialog54
 exe_file  $my qm$\Dialog54.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {98C8594E-D055-4071-9AD6-EADF5EEB508A}
 END PROJECT
