\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

out
int+ g_ecounter; g_ecounter+1
out ShowDialog(dd &sub.DlgProc 0)


#sub DlgProc
function# hDlg message wParam lParam

OutWinMsg message wParam lParam
int- t_ecounter; t_ecounter+1; if(t_ecounter=g_ecounter) min 0
sel message
	case WM_INITDIALOG
	 SetTimer hDlg 1 2000 0
	
	 out "<ERROR>"
	 opt end 1
	 min 0
	 end "aaa"
	 RaiseException 0x80004445 0 0 0
	
	case WM_TIMER
	 _i/0
	PostMessage hDlg WM_USER+100 0 0
	
	case WM_USER+100
	_i/0
	
	case WM_DESTROY
	 min 0
	 end "aaa"
	 RaiseException 0x80004445 0 0 0
	
	 case WM_LBUTTONUP
	 int dlgproc=&sub.DlgProc
	 PF
	  rep(100) CallWindowProc(dlgproc hDlg WM_APP wParam lParam)
	 rep(100) call(dlgproc hDlg WM_APP wParam lParam)
	 PN;PO
	
	case WM_APP
	 out 777
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
