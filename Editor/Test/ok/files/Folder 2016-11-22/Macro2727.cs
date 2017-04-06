 /exe 1
 POINT p.x=500; p.y=400; ShowMenu "one[]>two[]three[]<[]four" 0 p
 MessageBox 0 "t" "c" 0
 ret

out

 PostMessage 0 0 0 0
 MSG m
  PeekMessage &m 0 0 0 1
 GetMessage(&m 0 0 0)

 SetThreadPriority(GetCurrentThread THREAD_PRIORITY_HIGHEST)

 SetWindowsHookEx(WH_GETMESSAGE &sub.Hook_WH_GETMESSAGE 0 GetCurrentThreadId)

 TODO: why so slowly exe-compiles this macro? Fast if without __RegisterWindowClass or MessageLoop or CreateWindowEx.

__RegisterWindowClass+ __MyClass1.Register("MyClass1" &sub.WndProc)
 __RegisterWindowClass+ __MyClass1.Register("MyClass1" 0) ;;now fast. But with empty WndProc slow.
rep 2
	0.5
	int w=CreateWindowEx(WS_EX_NOACTIVATE|WS_EX_TOPMOST "MyClass1" 0 WS_POPUP|WS_CAPTION|WS_SYSMENU 500 400 300 200 0 0 0 0)
	MessageLoop
#ret

 out AllowSetForegroundWindow(GetCurrentProcessId)
 AllowActivateWindows

 POINT p.x=500; p.y=400; ShowMenu "one[]>two[]three[]<[]four" 0 p

 MessageBox 0 "t" "c" 0
 MessageBox 0 "t" "c" MB_SETFOREGROUND
 mes "ggg"

 BEGIN PROJECT
 main_function  Macro2727
 exe_file  $my qm$\Macro2727.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {FC1BA45F-6884-4DB6-BEB8-38C1897F40A8}
 END PROJECT


#sub WndProc
function# hwnd message wParam lParam

0.005

 if(IsMouseCursor(IDC_WAIT)) OutWinMsg message wParam lParam
 if(!IsMouseCursor(IDC_ARROW))
if(IsMouseCursor(IDC_APPSTARTING))
	out F" {message}"
	 out SetCursor(LoadCursor(0 +IDC_ARROW))
	 SetCursor does not change the cursor, probably because it is set by another process or Windows.
	 TODO: try AttachThreadInput

sel message
	case WM_CREATE
	out "WM_CREATE"
	SetCursor LoadCursor(0 +IDC_ARROW)
	
	 out GetThreadPriority(GetCurrentThread)
	 SetForegroundWindow hwnd
	 SetActiveWindow hwnd ;;activates if no WS_EX_NOACTIVATE
	 out GetThreadPriority(GetCurrentThread)
	 SendMessage hwnd WM_ACTIVATE 1 0 ;;activates
	 SetFocus hwnd ;;activates
	
	 PostMessage hwnd WM_PAINT 0 0
	 MSG m; if(PeekMessage(&m 0 0 0 1)) DispatchMessage(&m)
	 MSG m; if(PeekMessage(&m 0 0 0 1)) out 1
	
	 SetCapture(hwnd)
	out "--->"
	if(GetWinStyle(hwnd 1)&WS_EX_NOACTIVATE) ShowWindow hwnd SW_SHOWNA
	 ShowWindow hwnd SW_SHOWNA
	out "<---"
	 ReleaseCapture
	
	SetTimer hwnd 1 1000 0
	
	case WM_TIMER
	sel wParam
		case 1
		KillTimer hwnd wParam
		DestroyWindow hwnd
	
	case WM_SHOWWINDOW
	if(wParam)
		out "WM_SHOWWINDOW"
		 SetCursor LoadCursor(0 +IDC_ARROW)
	
	case WM_DESTROY
	PostMessage 0 2000 0 0

int R=DefWindowProcW(hwnd message wParam lParam)
ret R


#sub Hook_WH_GETMESSAGE
function# nCode remove MSG&m
if(nCode<0) goto gNext

out F"   {m.message}"

 gNext
ret CallNextHookEx(0 nCode remove &m)
