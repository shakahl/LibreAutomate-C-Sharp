\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
ax3SHD="http://www.quickmacros.com/forum"
if(!ShowDialog("Dialog6" &Dialog6 &controls)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 551 377 "Form"
 3 ActiveX 0x54000000 0x4 0 16 550 360 "SHDocVw.WebBrowser"
 4 Button 0x54032001 0x4 0 0 48 14 "acc"
 5 Button 0x54032000 0x0 50 0 48 14 "htm"
 END DIALOG
 DIALOG EDITOR: "" 0x2010703 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG DT_Init(hDlg lParam)
	 SHDocVw.WebBrowser we3._getcontrol(id(3 hDlg))
	
	
	ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	int t1=perf
	Acc a=acc("Programming" "LINK" "Form" "Internet Explorer_Server" "" 0x1001)
	int t2=perf
	out t2-t1
	out a.Name
	
	case 5
	int t11=perf
	Htm el=htm("" "Programming" "" "Form" 0 0 0x21)
	int t22=perf
	out t22-t11
	out el.Text
	
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
