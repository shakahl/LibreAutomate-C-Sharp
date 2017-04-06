out
 atend sub.Atend
#compile "__Bu"
Bu x
out DialogBoxParam(_hinst +103 0 &sub.DlgProc 0)


#sub DlgProc
function# hDlg message wParam lParam

 int- ods
 if(ods) OutWinMsg message wParam lParam _s; OutputDebugString _s
 OutWinMsg message wParam lParam
sel message
	case WM_INITDIALOG
	 SetTimer hDlg 1 1 &sub.Timer
	
	case WM_DESTROY
	case WM_LBUTTONUP
	 out
	 opt end 1
	 PostQuitMessage 0; ret
	 ods=1
	ret CallWindowProc(&sub.DlgProc2 hDlg message wParam lParam)
	 act "jjkjkj"
	
	 shutdown -7 1
	 shutdown -7
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK EndDialog hDlg 1
	case IDCANCEL EndDialog hDlg 0
ret 1


#sub DlgProc2
function# hDlg message wParam lParam

Bu x
 ret CallWindowProc(&sub.DlgProc3 hDlg message wParam lParam)
ret call(&sub.DlgProc3 hDlg message wParam lParam)
 opt end 1
act "jjkjkj"

 shutdown -7 1
 shutdown -7


#sub DlgProc3
function# hDlg message wParam lParam

Bu x
 opt end 1
act "jjkjkj"

 shutdown -7 1
 shutdown -7


#sub Atend
out "Atend"


#sub Timer
function a b c d
 opt end 1
child("" "h" _i)
 out 1
 end

