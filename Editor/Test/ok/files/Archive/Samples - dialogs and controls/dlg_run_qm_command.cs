\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 displays an input field where you can type a QM command
 (for example, run "notepad.exe") and press Enter to execute it


if(getopt(nthreads)>1) act "inp9"; ret

str controls = "3"
str e3
if(!ShowDialog("dlg_run_qm_command" &dlg_run_qm_command &controls)) ret
rep(100) if(win) break; else 0.02

spe -2
sel e3 1
	case "back": key AL
	case "forw": key AR
	case else
	if(e3.begi("run")) e3.gett(e3 1); run e3; err
	else if(e3.begi("act"))
		e3.gett(e3 1)
		if(e3.len) act win(e3 "" "" 2); err
		else act; err

 BEGIN DIALOG
 0 "" 0x9000024A 0x88 0 0 96 14 "inp9"
 3 Edit 0x54030080 0x204 0 0 96 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010703 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	mov ScreenWidth/2-80 0 hDlg 
	ret 1
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
