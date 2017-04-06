\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_flicks" &dlg_flicks)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 467 441 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages

def WM_TABLET_FLICK 0x000002CB
def FLICK_WM_HANDLED_MASK 0x1

 OutWinMsg message wParam lParam
sel message
	case WM_INITDIALOG
	case WM_TABLET_FLICK
	out "flick"
	ret DT_Ret(hDlg FLICK_WM_HANDLED_MASK)
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
