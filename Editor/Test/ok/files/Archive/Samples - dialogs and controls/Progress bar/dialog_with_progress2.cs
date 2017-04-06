 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 198 27 "Form"
 3 msctls_progress32 0x54000000 0x4 8 10 136 12 ""
 5 Static 0x54000000 0x0 150 10 44 12 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010900 "" ""

ret
 messages
 you can change these values
int range=100
int step=1
int timerperiod=100 ;;should be >=50

int- ti
int hpb=id(3 hDlg)
str s
sel message
	case WM_INITDIALOG
	SendMessage hpb PBM_SETRANGE ti range<<16
	SetTimer hDlg 1 timerperiod 0
	
	case WM_TIMER
	ti+step
	SendMessage hpb PBM_SETPOS ti 0
	if(ti<range) s.format("%i %%" 100*ti/range)
	else
		s="Completed"
		KillTimer hDlg 1
		DT_Ok hDlg ;;close dialog
	s.setwintext(id(5 hDlg))
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
