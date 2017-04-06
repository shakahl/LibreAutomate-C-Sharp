 /Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 This function allows to execute code whenever some event
 (dialog created, button clicked, etc) occurs in dialog.
 The code must follow appropriate case statements.
 To add case statements for various messages (events), you
 can use Events button in Dialog Editor. Read more in Help.

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 276 70 "Quick Words"
 3 Edit 0x54030080 0x200 0 0 274 18 ""
 4 Button 0x54032000 0x0 0 20 48 14 "Play"
 5 Button 0x54032000 0x0 48 20 48 14 "Stop"
 6 Edit 0x54231044 0x200 0 36 274 32 ""
 7 Button 0x54032000 0x0 226 20 48 14 "Paste"
 END DIALOG
 DIALOG EDITOR: "" 0x2010400 "" ""

ret
 messages
sel message
	case WM_INITDIALOG DT_Init(hDlg lParam); ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
int ctrlid=wParam&0xFFFF; message=wParam>>16
sel wParam
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
