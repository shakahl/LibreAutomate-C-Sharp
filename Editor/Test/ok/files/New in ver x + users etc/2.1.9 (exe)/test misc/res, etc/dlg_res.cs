\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

AddTrayIcon ":133"

str controls = "9 10 11"
str sb9 si10 si11

sb9=":2"
si10=":133"
si11="&:133"

if(!ShowDialog("dlg_res" &dlg_res &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 9 Static 0x5400100E 0x20000 86 30 34 16 ""
 10 Static 0x54000003 0x0 86 52 16 16 ""
 11 Static 0x54000003 0x0 86 74 28 24 ""
 3 Button 0x54032000 0x0 4 4 48 14 "mes"
 4 Button 0x54032000 0x0 4 32 48 14 "&A menu 1"
 5 Button 0x54032000 0x0 4 50 48 14 "&B menu 2"
 6 Button 0x54032000 0x0 4 68 48 14 "&C menu res"
 7 Button 0x54032000 0x0 4 86 48 14 "&D menu remove"
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "" ""

 BEGIN MENU
 >&File
	 &Open : 101 0
	 -
	 >&Recent
		 Empty : 102 0 3
		 <
	 <
 >&Edit
	 Cu&t[9]Ctrl+X : 103 0
	 -
	 Select &All : 104 0
	 <
 &Help : 105 0
 END MENU

ret
 messages
if(message=WM_INITDIALOG) DT_Init(hDlg lParam)
 int param=DT_GetParam(hDlg)

sel message
	case WM_INITDIALOG
	int hmenu haccel
	hmenu=DT_CreateMenu("dlg_res3" haccel)
	DT_SetMenu hDlg hmenu haccel
	ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3 mes "WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW" "" "q"
	
	case 4
	DT_SetMenu hDlg DT_CreateMenu("Menu6")
	case 5
	DT_SetMenu hDlg DT_CreateMenu("Menu7")
	case 6
	DT_SetMenu hDlg DT_CreateMenu(":1")
	case 7
	DT_SetMenu hDlg DT_CreateMenu("") 0 1

	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
