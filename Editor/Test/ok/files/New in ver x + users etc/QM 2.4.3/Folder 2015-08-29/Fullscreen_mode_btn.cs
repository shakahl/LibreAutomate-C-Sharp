\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD="http://www.google.com"
if(!ShowDialog("Fullscreen_mode_btn" &Fullscreen_mode_btn &controls)) ret

 BEGIN DIALOG
 0 "" 0x90CC0AC8 0x0 0 0 444 446 "보기"
 3 ActiveX 0x54030000 0x0 0 0 444 428 "SHDocVw.WebBrowser"
 6 Button 0x54032000 0x0 280 432 78 14 "Full_Screen (F11)"
 2 Button 0x54030000 0x0 396 432 48 14 "Close"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 DT_SetAutoSizeControls hDlg "3s 6m 2m"
	DT_SetAccelerators hDlg "6 F11"

	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_SETCURSOR

ret
 messages2
sel wParam
	case 6
	int-- t_fullScreen
	t_fullScreen^1
	int cAX=id(3 hDlg)
	if t_fullScreen
		Zorder hDlg HWND_TOPMOST
		SetWinStyle hDlg WS_BORDER|WS_THICKFRAME 2
		SetWindowState hDlg SW_SHOWMAXIMIZED 1
		SetWindowState cAX SW_SHOWMAXIMIZED 1
	else
		Zorder hDlg HWND_NOTOPMOST
		SetWinStyle hDlg WS_BORDER|WS_THICKFRAME 1
		SetWindowState hDlg SW_SHOWNORMAL 1
		SetWindowState cAX SW_SHOWNORMAL 1
	0; InvalidateRect child("" "Internet Explorer_Server" cAX) 0 0 ;;sometimes does not repaint without this
	
	case IDOK
	case IDCANCEL
ret 1