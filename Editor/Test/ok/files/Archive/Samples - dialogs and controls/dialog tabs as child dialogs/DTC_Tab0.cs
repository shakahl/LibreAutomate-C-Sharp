 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4"
str e4
sel message
	case 0 ;;on WM_INITDIALOG of the parent dialog
	ret ShowDialog("" &DTC_Tab0 &controls wParam 1 WS_CHILD 0 0 2 lParam)
	case 1 ;;on OK of the parent dialog
	hDlg=lParam
	DT_GetControls hDlg &controls
	out e4

 BEGIN DIALOG
 0 "" 0x10000648 0x0 0 0 187 107 ""
 3 Static 0x54000000 0x0 26 22 48 12 "A"
 4 Edit 0x54030080 0x200 26 38 96 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
 sel wParam
	 case ...
	 ...
ret 1

