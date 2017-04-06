\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
if(!ShowDialog("" &Dialog111 0)) ret
 if(!ShowDialog("" &Dialog111 0 0 128)) ret


 int h=ShowDialog("Dialog111" 0 0 0 1)
 int h=ShowDialog("Dialog111" &Dialog111 0 0 1)
  atend DestroyWindow h
 opt waitmsg 1
 10

 DestroyWindow h
 opt nowarnings 1

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 END DIALOG

ret
 messages
sel message
	case WM_INITDIALOG
	hid- hDlg
	 clo hDlg
	
	 case WM_LBUTTONDOWN
	 shutdown -7
	case WM_COMMAND goto messages2
	case WM_QM_ENDTHREAD ret 1
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
