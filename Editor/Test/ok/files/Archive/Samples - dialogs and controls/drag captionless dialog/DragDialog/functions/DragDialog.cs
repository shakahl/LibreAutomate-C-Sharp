 /
function# hDlg message [modifiers] ;;modifiers: 1 Shift, 2 Ctrl, 4 Alt, 8 Win

 Moves the dialog when the user presses left mouse button on the client area and drags.
 Returns 1 if suscessful, 0 if not (then the message should be passed to the default window procedure).
 The dialog must be a smart dialog, ie with dialog procedure.
 Call this function from the dialog procedure as shown below:

	 case [WM_LBUTTONDOWN,WM_LBUTTONUP,WM_MOUSEMOVE,WM_CANCELMODE] DragDialog hDlg message

 To enable moving the dialog by dragging a control, call DialogDragSubclassControl on WM_INITDIALOG.
 If the control has scroll bars, use Ctrl or other modifier key to scroll.


POINT-- pp_drag
POINT p
RECT r
int-- t_indrag

sel message
	case WM_LBUTTONDOWN
	t_indrag=0
	if(GetMod!=modifiers) ret
	if(IsChildWindow(hDlg)) ;;need to detect drag, or the control will not respond to clicks
		xm p
		if(!DragDetect(hDlg p))
			ScreenToClient hDlg &p
			PostMessage hDlg WM_LBUTTONUP 0 p.y<<16|p.x
			ret
	xm pp_drag
	SetCapture hDlg
	t_indrag=1
	
	case [WM_LBUTTONUP,WM_CANCELMODE]
	if(!t_indrag or GetCapture!=hDlg) ret
	ReleaseCapture
	t_indrag=0
	
	case WM_MOUSEMOVE
	if(!t_indrag or GetCapture!=hDlg) ret
	hDlg=GetAncestor(hDlg 2) ;;in case hDlg is a subclassed control
	GetWindowRect hDlg &r
	xm p
	mov r.left+p.x-pp_drag.x r.top+p.y-pp_drag.y hDlg
	pp_drag=p

ret 1
