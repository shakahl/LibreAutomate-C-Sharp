 /
function# $classname wndproc [flags] [hicon] [cbWndExtra] [style] [hiconSmall] ;;flags: 1 unregister, 4 Unicode

 Registers or unregisters a window class. Obsolete, use __RegisterWindowClass class.
 Returns class atom if succeeded, 0 if not. On unregister returns 1 if succeded.
 QM 2.3.0: Returns -1 if a window of the class exists, because then class properties cannot be changed.

 classname - some unique string.
 wndproc - address of window procedure.
   A template is available in menu -> File -> New -> Templates.
 hicon - icon handle. Default: uses default QM dialog icon.
 cbWndExtra, style, hiconSmall - the same as with <google>WNDCLASSEX</google>.
   hiconSmall added in QM 2.3.0.

 REMARKS
 If you register Unicode class, in window procedure use DefWindowProcW, not DefWindowProc.


if(flags&1) ret UnregisterClassW(@classname _hinst)

WNDCLASSEX w.cbSize=sizeof(w)
int at

if flags&2 ;;fbc
	if(flags&4) GetClassInfoExW(0 +32770 +&w); else GetClassInfoEx(0 +32770 &w)
	if(wndproc) w.lpfnWndProc=wndproc
else
	w.lpfnWndProc=wndproc
	w.hCursor=LoadCursor(0 +IDC_ARROW)
	w.hbrBackground=COLOR_BTNFACE+1
	w.cbWndExtra=cbWndExtra
	w.style=style

w.hInstance=_hinst
if(hicon or hiconSmall) w.hIcon=hicon; w.hIconSm=hiconSmall; else w.hIcon=_dialogicon

 g1
if(flags&4)
	w.lpszClassName=+@classname
	at=RegisterClassExW(+&w)
else
	w.lpszClassName=classname
	at=RegisterClassEx(&w)

if(at) ret at
if(GetLastError=ERROR_CLASS_ALREADY_EXISTS)
	if(!UnregisterClassW(@classname _hinst)) ret -1 ;;need to unregister to avoid 'invalid callback' error after reloading file
	goto g1
