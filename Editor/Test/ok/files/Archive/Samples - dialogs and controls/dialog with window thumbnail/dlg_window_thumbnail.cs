\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_window_thumbnail" &dlg_window_thumbnail 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 186 138 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	int h=win("Calc")
	int-- thumbnail
	thumbnail=DwmShowThumbnail(hDlg h)
	
	case WM_DESTROY
	if(thumbnail) DwmUnregisterThumbnail thumbnail
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
