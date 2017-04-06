\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "7 8"
str e7 c8Pau
if(!ShowDialog("dialog_stop_watch" &dialog_stop_watch &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Button 0x54032000 0x0 4 4 48 14 "Start"
 4 Button 0x5C032000 0x0 4 22 48 14 "Stop"
 5 Button 0x54032000 0x0 4 40 48 14 "Reset"
 6 Static 0x54000000 0x0 58 6 68 12 ""
 7 Edit 0x54230844 0x20000 56 22 70 60 ""
 8 Button 0x54012003 0x0 4 68 48 13 "Pause"
 END DIALOG
 DIALOG EDITOR: "" 0x2030301 "*" "" ""

ret
 messages
int-- t_running t_pause
long-- t_startTime t_activeTime
str s
str-- t_results
sel message
	case WM_INITDIALOG
	
	case WM_TIMER
	sel wParam
		case 1
		s=TimeSpanToStr(perf-t_startTime+t_activeTime*10 2)
		s.setwintext(id(6 hDlg))
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3 ;;Start/Lap
	if !t_running ;;start
		t_running=1
		t_startTime=perf
		SetTimer hDlg 1 100 0
		EnableWindow id(4 hDlg) 1
		s="Lap"; s.setwintext(lParam)
	else ;;lap
		s=TimeSpanToStr(perf-t_startTime+t_activeTime*10 2)
		t_results.addline(s 1); t_results.setwintext(id(7 hDlg)); SendMessage id(7 hDlg) WM_VSCROLL SB_BOTTOM 0
		if(t_pause) t_activeTime+perf-t_startTime; else t_activeTime=0
		t_startTime=perf
	
	case 4 ;;Stop
	if t_running
		t_running=0
		if(t_pause) t_activeTime+perf-t_startTime; else t_activeTime=0
		KillTimer hDlg 1
		s="Start"; s.setwintext(id(3 hDlg))
		EnableWindow id(4 hDlg) 0
	
	case 5 ;;Reset
	if(t_running) but 4 hDlg
	t_activeTime=0
	t_results=""
	s.setwintext(id(6 hDlg))
	s.setwintext(id(7 hDlg))
	
	case 8 ;;Pause
	but 5 hDlg
	t_pause=but(lParam)
ret 1
