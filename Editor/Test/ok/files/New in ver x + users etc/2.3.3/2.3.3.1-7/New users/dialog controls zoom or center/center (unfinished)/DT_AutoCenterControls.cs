 /dlg_controls_centered
function hDlg message $controls

 Moves or resizes dialog controls when you resize the dialog.
 Call this function from dialog procedure, before sel message.

 hDlg, message - hDlg, message.
 controls - space-separated list of controls.
   Syntax for a control: IdActionDirection
     Id - control id.
     Action - m (move) or s (resize).
     Direction - h (horizontally) or v (vertically) or none (horizontally and vertically).

 EXAMPLE
  ...
  messages
 DT_AutoSizeControls hDlg message "1m 2m 3sh 4mv 5s"
 sel message
	 case WM_INITDIALOG
	  ...


type ACC_CONTROL hwnd direction ^x ^y cx cy
ACC_CONTROL* p
int i j k x y
RECT r rc

sel message
	case WM_INITDIALOG
	GetClientRect hDlg &rc
	
	ARRAY(lpstr) a
	tok controls a
	p._new(a.len)
	SetProp hDlg "acc_controls" p
	
	for i 0 a.len
		ACC_CONTROL& c=p[i]
		lpstr s=a[i] ;;out s
		j=val(s 0 k); s+k
		c.hwnd=id(j hDlg); if(!c.hwnd) out "Warning: control %i not found." j; continue
		c.direction=s[0]
		
		GetWindowRect c.hwnd &r; MapWindowPoints 0 hDlg +&r 2
		c.cx=r.right-r.left
		c.cy=r.bottom-r.top
		c.x=1.0*r.left/(rc.right-c.cx-r.left)
		c.y=1.0*r.top/(rc.bottom-c.cy-r.top)
	
	case WM_SIZE
	GetClientRect hDlg &rc
	
	p=+GetProp(hDlg "acc_controls")
	
	for i 0 p._len
		&c=p[i]
		j=0; sel(c.direction) case 'h' j|2; case 'v' j|1
		x=c.x*(rc.right-c.cx)/(1+c.x)
		y=c.y*(rc.bottom-c.cy)/(1+c.y)
		mov x y c.hwnd j; err
	
	case WM_DESTROY
	p=+RemoveProp(hDlg "acc_controls")
	p._delete
