 /
function hDlg message $controls

 Proportionally moves and resizes dialog controls when you resize the dialog.
 Call this function from dialog procedure, before sel message.

 hDlg, message - hDlg, message.
 controls - space-separated list of controls.
   Syntax for a control: IdActionDirection
     Id - control id.
     Action - m (move) or s (resize) or none (move and resize).
     Direction - h (horizontally) or v (vertically) or none (horizontally and vertically).

 EXAMPLE
  ...
  messages
 DT_AutoZoomControls hDlg message "3mv 4 5h 6m"
 sel message
	 case WM_INITDIALOG
	  ...


type AZC_CONTROL hwnd !action !direction ^x ^y ^cx ^cy
AZC_CONTROL* p
int i j k x y cx cy
RECT r rc

sel message
	case WM_INITDIALOG
	GetClientRect hDlg &rc
	
	ARRAY(lpstr) a
	tok controls a
	p._new(a.len)
	SetProp hDlg "azc_controls" p
	
	for i 0 a.len
		AZC_CONTROL& c=p[i]
		lpstr s=a[i]
		j=val(s 0 k); s+k
		c.hwnd=id(j hDlg); if(!c.hwnd) out "Warning: control %i not found." j; continue
		sel(s[0]) case ['m','s'] c.action=s[0]; s+1
		c.direction=s[0]
		
		GetWindowRect c.hwnd &r; MapWindowPoints 0 hDlg +&r 2
		c.x=1.0*r.left/rc.right
		c.y=1.0*r.top/rc.bottom
		c.cx=1.0*r.right-r.left/rc.right
		c.cy=1.0*r.bottom-r.top/rc.bottom
	
	case WM_SIZE
	GetClientRect hDlg &rc
	
	p=+GetProp(hDlg "azc_controls")
	
	for i 0 p._len
		&c=p[i]
		j=0; sel(c.direction) case 'h' j|2; case 'v' j|1
		x=c.x*rc.right
		y=c.y*rc.bottom
		cx=c.cx*rc.right
		cy=c.cy*rc.bottom
		if(c.action!'s') mov x y c.hwnd j; err
		if(c.action!'m') siz cx cy c.hwnd j; err
	
	case WM_DESTROY
	p=+RemoveProp(hDlg "azc_controls")
	p._delete
