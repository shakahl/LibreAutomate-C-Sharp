/Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 This function allows to execute code whenever some event
 (dialog created, button clicked, etc) occurs in dialog.
 The code must follow appropriate case statements.
 To add case statements for various messages (events), you
 can use Events button in Dialog Editor. Read more in Help.

if(!ShowDialog("Dialog3" &Dialog3)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 16 16 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2010300 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG
	Tray- t.Add
	DT_Init(hDlg lParam); ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
int ctrlid=wParam&0xFFFF; message=wParam>>16
sel wParam
	case 3 end
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
