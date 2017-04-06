\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_SysAnimate32" &dlg_SysAnimate32)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 250 250 "Dialog"
 3 SysAnimate32 0x54000000 0x0 0 0 96 48 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020002 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	int han=id(3 hDlg)
	SetWinStyle han ACS_TRANSPARENT 1
	str f="$windows$\clock.avi"
	if(!dir(f)) ;;Vista
		f="$system$\migwiz\progress.avi"
		if(!dir(f)) f=""; if(!OpenSaveDialog(0 f "avi[]*.avi")) ret
	SendMessage han ACM_OPEN 0 f.expandpath
	 SendMessage han ACM_OPEN GetExeResHandle 1
	int from=0 ;;beginning
	int to=-1 ;;end
	int repeat=-1 ;;indefinitely
	SendMessage han ACM_PLAY repeat to<<16|from
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
