\Dialog_Editor

 Shows a simple way of how to launch another thread from a dialog and wait for it.
 Usually using a separate thread is needed to execute code that would
 cause the dialog to not respond for some time if executed in dialog's thread.

function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4"
str rea4
if(!ShowDialog("dlg_run_other_thread" &dlg_run_other_thread &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 2 8 96 16 "Test"
 4 RichEdit20A 0x54233044 0x200 2 30 96 48 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030108 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	int h=id(3 hDlg)
	EnableWindow h 0; _s="Please wait 5 s"; _s.setwintext(h)
	
	opt waitmsg 1 ;;process messages while waiting
	wait 0 H mac("other_thread") ;;run other_thread in separate thread and wait
	
	EnableWindow h 1; _s="Test"; _s.setwintext(h)
	case IDOK
	case IDCANCEL
ret 1
