function# $className wndProc [cbWndExtra] [$icon] [style] [hbrBackground] [hCursor]

 Registers window class.
 Returns class atom.

 className - class name.
 wndProc - address of window procedure (user-defined function that will be called to process messages).
   Template: menu File -> New -> Templates -> WndProc -> WndProc_Normal.
 cbWndExtra - number of extra window bytes. Can be accessed with SetWindowLong/GetWindowLong, with nIndex>=0.
 style - <google "site:microsoft.com Window Class Styles">class styles</google>.
 icon - icon to show in title bar, taskbar etc.
   Can be icon file. Loads using <help>GetFileIcon</help>.
   Or can be string containing handles of icon and small icon, eg F"{hIcon} {hIconSm}", or one of them.
   If not set, shows default icon.
 hbrBackground - background brush handle. Or a system color constant + 1.
   If 0, uses dialog background color (COLOR_BTNFACE+1).
   If -1, does not set; then the window procedure must erase.
 hCursor - cursor handle.
   If 0, uses arrow cursor (LoadCursor(0 +IDC_ARROW)).
   If -1, does not set; then the window procedure should call SetCursor on WM_SETCURSOR.

 REMARKS
 Calls <google "site:microsoft.com RegisterClassEx">RegisterClassEx</google> with WNDCLASSEX filled with the specified values.
 The class is Unicode. The window procedure must call DefWindowProcW and handle Unicode messages and Unicode UTF-16 strings in their parameters.
 Should be used single global variable for a class. Window classes are global for this process.
 Fails if the class already exists. However does nothing if Register already successfully called with this variable.
 Error if fails.
 If used brush/cursor/icon handles: Does not copy. When unregistering class, deletes brush, but does not destroy icons and cursor.

 EXAMPLE
 __RegisterWindowClass+ g_wndClassTest
 if(!g_wndClassTest.atom) g_wndClassTest.Register("QM_Test" &WndProc_Test 0 ":5 $qm$\macro.ico")
 int w=CreateWindowExW(0 @"QM_Test" @"Test" WS_VISIBLE|WS_POPUP|WS_CAPTION|WS_SYSMENU 0 0 200 100 0 0 _hinst 0)
 MessageLoop ;;runs until WndProc_Test calls PostQuitMessage on WM_DESTROY


lock
if(atom) ret atom

WNDCLASSEXW w.cbSize=sizeof(w)
w.hInstance=_hinst
w.lpszClassName=@className
w.lpfnWndProc=wndProc
w.cbWndExtra=cbWndExtra
w.style=style
if(hbrBackground!-1) w.hbrBackground=iif(hbrBackground hbrBackground COLOR_BTNFACE+1)
if(hCursor!-1) w.hCursor=iif(hCursor hCursor LoadCursor(0 +IDC_ARROW))
if !empty(icon)
	int i j
	i=val(icon 0 j)
	if(j) w.hIcon=i; w.hIconSm=val(icon+j)
	else
		m_hicon=GetFileIcon(icon 0 1); w.hIcon=m_hicon
		m_hiconSm=GetFileIcon(icon); w.hIconSm=m_hiconSm

atom=RegisterClassExW(&w)
if !atom
	_s.dllerror
	Unregister ;;destroy icon
	end F"failed to register window class. {_s}"
ret atom
