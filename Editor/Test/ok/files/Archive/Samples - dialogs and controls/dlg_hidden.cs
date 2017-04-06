\Dialog_Editor

 Note: This is for QM < 2.3.3. In QM 2.3.3 an later, simply use flag 128 with ShowDialog.

 How to run a dialog invisible?
 Removing WS_VISIBLE style does not work, because Windows always shows modal dialogs.
 The answer: the dialog must be modeless.
 However working with modeless dialogs is more difficult.

 1. It must have dialog procedure. Like this function.
 2. Remove WS_VISIBLE and DS_SETFOREGROUND styles. You can do it in Dialog Editor.
 3. It must be modeless. Use flag 1 with ShowDialog.
 4. The macro must not exit immediately. It should wait until the dialog is closed.
 5. The thread must process messages while waiting. opt waitmsg 1.
 6. If need dialog variables, on IDOK call DT_GetControls.
 7. If need to know whether OK pressed, on IDOK set a variable.


function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
int OK
hDlg=ShowDialog("dlg_hidden" &dlg_hidden &controls 0 1 0 0 &OK)
opt waitmsg 1
wait 0 -WC hDlg
if(!OK) ret
out e3

 BEGIN DIALOG
 0 "" 0x80C808C8 0x0 0 0 223 135 "Dialog"
 3 Edit 0x54030080 0x200 6 6 96 14 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 3000 0 ;;show itself after 3s
	
	case WM_TIMER
	sel wParam
		case 1
		KillTimer hDlg wParam
		act hDlg
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	DT_GetControls hDlg
	int& rOK=+DT_GetParam(hDlg); rOK=1
	case IDCANCEL
ret 1
