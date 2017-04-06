 /exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4"
str e4
if(!ShowDialog("Dialog10" &Dialog10 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 8 8 24 14 "SF"
 4 Edit 0x54030080 0x200 8 32 96 14 ""
 5 Button 0x54032000 0x0 42 8 24 14 "RX"
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "" ""

ret
 messages

sel message
	case WM_INITDIALOG
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	 case 3 if(SpecFoldersMenu(hDlg &_s)) _s.setwintext(id(4 hDlg))
	case 5 RegExpMenu id(4 hDlg)
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog10
 exe_file  $my qm$\Dialog10.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {060F774F-4F40-4476-86BE-A66A9DB5DA4B}
 END PROJECT
