 /
function hDlg lParam

 Initializes dialog.
 Called implicitly on WM_INITDIALOG, before calling dialog procedure.


if(!lParam) ret
__DIALOG* d=+lParam
if(d.flags2&1) d.flags2|16; ret ;;already called
d.flags2|1

SendMessage hDlg WM_SETICON 0 d.hicon16
SendMessage hDlg WM_SETICON 1 d.hicon32
if(d.hmenu) DT_SetMenu hDlg d.hmenu d.haccel
if(d.tt) d.tt.Create(hDlg d.ttFlags d.ttTime)
if(d.controls) DT_SetControls hDlg d
if(d.flags2&0x200) SetWindowLong hDlg GWL_HWNDPARENT d.hwndowner ;;set owner of modal dialog when owner belongs to other thread

 set dialog position
RECT r _r; int monitor=_monitor
if(d.flags&0x100)
else if(d.style&WS_CHILD)
	mov d.x d.y hDlg
else if(d.flags&64) ;;raw x y, relative to primary monitor
	r.left=d.x; r.top=d.y
	if(d.style&DS_CENTER) ;;ensure in work area
		GetWindowRect hDlg &_r
		r.right=d.x+(_r.right-_r.left); r.bottom=d.y+(_r.bottom-_r.top)
		AdjustWindowPos 0 &r 9
	goto g2
else if(d.style&DS_CENTERMOUSE)
	if(d.x or d.y)
		GetWindowRect hDlg &r
		OffsetRect &r -r.left-(r.right-r.left/2)+xm+d.x -r.top-(r.bottom-r.top/2)+ym+d.y
		AdjustWindowPos 0 &r 9 -1
		goto g2
else if(d.hwndowner)
	if(d.style&(DS_CENTER|DS_ABSALIGN)) goto g1
	r.left=d.x; r.top=d.y
	ClientToScreen d.hwndowner +&r
	goto g2
else
	 g1
	r.left=d.x; r.top=d.y
	AdjustWindowPos hDlg &r iif(d.style&DS_CENTER 1 1|4) iif(monitor monitor d.hwndowner)
	 g2
	 if(d.hwndowner) SetWindowLong hDlg GWL_HWNDPARENT 0
	SetWindowPos hDlg 0 r.left r.top 0 0 SWP_NOSIZE|SWP_NOZORDER|SWP_NOOWNERZORDER|SWP_NOACTIVATE|SWP_NOSENDCHANGING
	 if(d.hwndowner) SetWindowLong hDlg GWL_HWNDPARENT d.hwndowner
	 Windows bug: on nonprimary monitor, if hwndowner used, SetWindowPos moves hDlg to the owner's center. Cannot reproduce now, maybe fixed. SWP_NOSENDCHANGING should prevent it, not tested.
	 Other workarounds: 1. Temporarily set owner 0. 2. Unhide hDlg. 3. Ensure that neither x nor y match monitor edge. 4. Temporarily set WS_CHILD style.

 note: we don't check if it has WS_MINIMIZE or WS_MAXIMIZE because these styles are removed by Windows. Dialogs cannot start min/max.
