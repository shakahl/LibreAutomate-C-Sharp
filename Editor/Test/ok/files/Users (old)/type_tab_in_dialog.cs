\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4 5"
str c3Che e4 rea5
ShowDialog("type_tab_in_dialog" &type_tab_in_dialog &controls 0 1)
MessageLoop
out rea5


 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54012003 0x0 8 6 48 12 "Check"
 4 Edit 0x54231044 0x200 6 26 96 48 ""
 5 RichEdit20A 0x54233044 0x200 104 26 96 48 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010703 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG DT_Init(hDlg lParam); ret 1
	case WM_DESTROY
	DT_DeleteData(hDlg)
	PostMessage 0 2000 0 0
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
