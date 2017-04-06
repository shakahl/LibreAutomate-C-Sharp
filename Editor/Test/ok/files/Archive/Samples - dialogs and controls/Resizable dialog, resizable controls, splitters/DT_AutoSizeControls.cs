 /
function hDlg message $controls

 Moves or resizes dialog controls when you resize the dialog.
 Call this function from dialog procedure, before sel message.

 hDlg, message - hDlg, message.
 controls - space-separated list of controls.
   Syntax for a control: IdActionDirection
     Id - control id.
     Action - m (move) or s (resize).
     Direction - h (horizontally) or v (vertically) or none (horizontally and vertically).

 See also: <DT_SetAutoSizeControls>.

 EXAMPLE
  ...
  messages
 DT_AutoSizeControls hDlg message "1m 2m 3sh 4mv 5s"
 sel message
	 case WM_INITDIALOG
	  ...


type ASC_CONTROL hwnd !action !direction horz vert
ASC_CONTROL* p
int i j k x y
RECT r rc

sel message
	case WM_INITDIALOG
	GetClientRect hDlg &rc
	
	ARRAY(lpstr) a
	tok controls a
	p._new(a.len)
	SetProp hDlg "asc_controls" p
	
	for i 0 a.len
		ASC_CONTROL& c=p[i]
		lpstr s=a[i] ;;out s
		j=val(s 0 k); s+k
		c.hwnd=id(j hDlg); if(!c.hwnd) out "Warning: control %i not found." j; continue
		c.action=s[0]; c.direction=s[1]
		
		GetWindowRect c.hwnd &r; MapWindowPoints 0 hDlg +&r 2
		sel c.action
			case 'm' c.horz=rc.right-r.left; c.vert=rc.bottom-r.top
			case 's' c.horz=rc.right-r.right; c.vert=rc.bottom-r.bottom
	
	case WM_SIZE
	GetClientRect hDlg &rc
	
	p=+GetProp(hDlg "asc_controls")
	
	for i 0 p._len
		&c=p[i]
		j=0; sel(c.direction) case 'h' j|2; case 'v' j|1
		sel c.action
			case 'm'
			x=rc.right-c.horz; y=rc.bottom-c.vert
			mov x y c.hwnd j; err
			
			case 's'
			GetWindowRect c.hwnd &r; MapWindowPoints 0 hDlg +&r 2
			x=rc.right-r.left-c.horz; y=rc.bottom-r.top-c.vert
			siz x y c.hwnd j; err
	
	case WM_DESTROY
	p=+RemoveProp(hDlg "asc_controls")
	p._delete
