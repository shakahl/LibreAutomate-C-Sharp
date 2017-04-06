 /
function# hDlg message wParam lParam

 Adds simple auto-shrink (actually roll-up) feature for a dialog.
 The dialog must be a smart dialog, ie with dialog procedure.
 Insert the following statement in the dialog procedure, before 'sel message' line:

 DT_AutoShrink hDlg message wParam lParam


type DASDATA !shr RECT'r
DASDATA* d=+GetProp(hDlg "qm_das")

sel message
	case WM_INITDIALOG
	SetProp hDlg "qm_das" d._new
	goto shrink
	case WM_DESTROY
	d._delete; RemoveProp hDlg "qm_das"
	case [WM_SETCURSOR,WM_PARENTNOTIFY]
	if(d.shr) goto shrink
	case WM_TIMER
	sel wParam
		case 1578
		if(win(mouse)!=hDlg and !GetCapture) goto shrink
ret

 shrink
d.shr^1
if(d.shr)
	KillTimer hDlg 1578
	GetWindowRect hDlg &d.r
	TITLEBARINFO ti.cbSize=sizeof(TITLEBARINFO)
	GetTitleBarInfo hDlg &ti
	int hei
	if(ti.rgstate[0]&STATE_SYSTEM_INVISIBLE) hei=20
	else hei=ti.rcTitleBar.bottom-ti.rcTitleBar.top+4
	siz 0 hei hDlg 1
else
	siz 0 d.r.bottom-d.r.top hDlg 1
	SetTimer hDlg 1578 100 0
