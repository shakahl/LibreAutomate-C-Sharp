\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4 5 6"
str e3 e4 e5 e6
if(!ShowDialog("dlg_Denise" &dlg_Denise &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 220 132 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Edit 0x54030800 0x20000 54 2 94 12 ""
 4 Edit 0x54030800 0x20000 54 18 94 12 ""
 5 Edit 0x54030800 0x20000 54 36 94 13 ""
 6 Edit 0x54030800 0x20000 54 52 94 13 ""
 7 Static 0x54000000 0x0 4 2 48 12 "Time started"
 8 Static 0x54000000 0x0 4 18 48 12 "Time running"
 9 Static 0x54000000 0x0 4 36 48 13 "Initial file size"
 10 Static 0x54000000 0x0 4 52 48 13 "File size now"
 11 Static 0x54000000 0x0 4 76 48 13 "Status"
 12 Static 0x54000000 0x0 56 76 48 13 "Running"
 END DIALOG
 DIALOG EDITOR: "" 0x2010901 "" ""

ret
 messages
int-- t_hThread
DATE-- t_date
Dir d
sel message
	case WM_INITDIALOG
	DT_Init(hDlg lParam) ;;not necessary in QM >= 2.1.9
	
	t_hThread=mac("Macro353")
	t_date.getclock
	SetDlgItemText hDlg 3 _s.time(t_date "%H:%M:%S")
	if(d.dir("$desktop$\test.txt" 0)) SetDlgItemInt hDlg 5 d.FileSize 0
	SetTimer hDlg 1 1000 0
	goto info
	
	case WM_TIMER
	sel wParam
		case 1
		if(WaitForSingleObject(t_hThread 0)!=258) ;;def WAIT_TIMEOUT 258
			SetDlgItemText hDlg 12 "Finished"
			KillTimer hDlg 1
		goto info
	
	case WM_DESTROY DT_DeleteData(hDlg) ;;not necessary in QM >= 2.1.9
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	DT_Ok hDlg ;;not necessary in QM >= 2.1.9
	case IDCANCEL DT_Cancel hDlg ;;not necessary in QM >= 2.1.9
ret 1
 info
DATE d2.getclock
d2=d2-t_date
SetDlgItemText hDlg 4 iif(d2.date _s.time(d2 "%H:%M:%S") "00:00:00")
if(d.dir("$desktop$\test.txt" 0)) SetDlgItemInt hDlg 6 d.FileSize 0
