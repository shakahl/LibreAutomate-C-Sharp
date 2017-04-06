\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 act "jjjjjj"

 out ShowDialog("dlg_res3" 0)
 if(!ShowDialog("dlg_res3" &dlg_res3)) ret
if(!ShowDialog("dlg_res3" &dlg_res3 0 0 0 0 0 0 0 0 ":131" ":109")) ret
 if(!ShowDialog("dlg_res3" &dlg_res3 0 0 0 0 0 0 0 0 "" ":109")) ret
 if(!ShowDialog("dlg_res3" &dlg_res3 0 0 0 0 0 0 0 0 "" "Menu6")) ret
 out ShowDialog("dlg_res3" 0 0 0 0 0 0 0 0 0 "" "Menu6")
 if(!ShowDialog("dlg_res3" &dlg_res3 0 0 0 0 0 0 0 0 "" "Sample menu")) ret

 int h=ShowDialog("dlg_res3" &dlg_res3 0 0 1 0 0 0 0 0 ":131" ":109")
 int h=ShowDialog("dlg_res3" &dlg_res3 0 0 1 0 0 0 0 0 "" "Menu6")
 opt waitmsg 1
 0 WD h

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 12 Button 0x54032000 0x0 4 6 48 14 "change menu"
 13 Button 0x54032000 0x0 4 36 48 14 "remove menu"
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "*" ""

 BEGIN MENU
 >A
	 B : 1000 0 0 Ch
 END MENU

ret
 messages
 int param=DT_GetParam(hDlg)

sel message
	case WM_INITDIALOG
	out "init"
	case WM_DESTROY
	out "destr"
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 12
	 str s.all(100)
	 s.fix(LoadString(GetExeResHandle 1170 s s.nc))
	 mes s
	 mac "Function33"
	int hmenu haccel
	hmenu=DT_CreateMenu("dlg_res3" haccel)
	DT_SetMenu hDlg hmenu haccel
	case 13
	DT_SetMenu hDlg 0
	
	case IDOK
	case IDCANCEL
	
	case else out wParam
ret 1
