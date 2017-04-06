\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str sb3
sb3="$windows$\soap bubbles.bmp"
hDlg=ShowDialog("dialog_logo" &dialog_logo &controls 0 1 0 WS_VISIBLE) ;;create modeless and invisible to avoid activation
SetWindowPos hDlg 0 0 0 0 0 SWP_NOMOVE|SWP_NOSIZE|SWP_NOZORDER|SWP_NOACTIVATE|SWP_SHOWWINDOW ;;show with SWP_NOACTIVATE flag
opt waitmsg 1
wait 5

 BEGIN DIALOG
 0 "" 0x90000844 0x80 0 0 119 111 ""
 3 Static 0x5400000E 0x0 0 0 16 16 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010901 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	DT_Init(hDlg lParam) ;;not necessary in QM >= 2.1.9
	
	int cx cy; GetWinXY id(3 hDlg) 0 0 cx cy
	siz cx cy hDlg
	
	case WM_DESTROY DT_DeleteData(hDlg) ;;not necessary in QM >= 2.1.9
ret
