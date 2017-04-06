\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 46 18 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 msctls_progress32 0x54000000 0x4 0 0 46 18 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030605 "" "" "" ""


if(!ShowDialog("" &Dialog4567)) ret

ret
 messages

int hpb=id(3 hDlg)
int step= 1 
int range=6
sel message
	case WM_INITDIALOG
		__Font-- f
		f.Create("Courier New" 22 1)
		SetWindowSubclass hpb &subclass_progress_bar_text 1 &f
		SendMessage hpb PBM_SETRANGE 1 range<<16
		SetTimer hDlg 1 1 0
		int-- rangecounter
	case WM_DESTROY
	case WM_TIMER
		out rangecounter
		SendMessage hpb PBM_SETPOS rangecounter 0
		rangecounter = rangecounter+1
		if rangecounter > range
			KillTimer hDlg 1
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1