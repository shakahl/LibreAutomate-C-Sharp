\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog98" &Dialog98 0 0 0 0 0 0 0 0 0 "dialog98")) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030306 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	MenuSetString GetMenu(hDlg) 1010 "Save"
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
out wParam
sel wParam
	case [1010,11010]
	case IDCANCEL
ret 1

 BEGIN MENU
 >&File
	 &Publications : 1001
	 &Advertisers : 1002
	 <
 >&Help
	 &View Help : 1005 0 0 F1
	 -
	 &About : 1006
	 <
 Save : 1010 0 0 Cs
 END MENU

	 &Save : 11010 0 0 Cs
