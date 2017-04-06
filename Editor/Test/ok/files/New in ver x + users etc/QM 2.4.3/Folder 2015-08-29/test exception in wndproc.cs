#compile "__Dtor"

 CRITICAL_SECTION+ g_cs; int+ g_csInited; if(!g_csInited) g_csInited=1; InitializeCriticalSection &g_cs

 Dtor m
 DtorThread- t_m
sub.Inner


#sub Inner
DtorInner m

out
 atend sub.Atend
int+ g_ecounter; g_ecounter+1
__RegisterWindowClass+ g_wcte.Register("QM_test_exc" &sub.WndProc)
int hwnd=CreateWindowExW(0 +g_wcte.atom @"Test exc" WS_VISIBLE|WS_SYSMENU 200 200 400 200 0 0 _hinst 0)
MessageLoop
out "after loop"
 1.5

 TODO test 'call &UDF'


#sub WndProc
function# hWnd message wParam lParam
 lock
OutWinMsg message wParam lParam
int- t_ecounter; t_ecounter+1; if(t_ecounter=g_ecounter) min 0
 opt end 1
sel message
	case WM_CREATE
	 out TryEnterCriticalSection(&g_cs)
	 out
	 DtorWP m
	 out "<ERROR>"
	 deb
	 min 0
	 out DestroyWindow(hWnd)
	 end "aaa"
	 RaiseException 0x80004445 0 0 0
	 sub.Exc
	 LeaveCriticalSection &g_cs
	 call &sub.Exc
	
	case WM_DESTROY
	 out "%i 0x%X" IsWindowVisible(hWnd) GetWinStyle(hWnd)
	 sub.DestroyThreadWindows
	 out DestroyWindow(hWnd)
	 out "WM_DESTROY"
	PostQuitMessage 0
	
	case WM_NCDESTROY
	 out DestroyWindow(hWnd)
	
	case WM_LBUTTONUP
	min 0
	 RaiseException 0x80004445 0 0 0
	
	case WM_MOUSEACTIVATE
	end "ff";

ret DefWindowProcW(hWnd message wParam lParam)


#sub Exc
 out "eeee"
 end "aaa"
min 0

#sub Atend
out "atend"


 #sub DestroyThreadWindows
 int-- once; if(once) ret; else once=1
 EnumThreadWindows(GetCurrentThreadId &sub.EnumProc 0)
 
 
 #sub EnumProc
 function# hwnd param
 
 outw hwnd
 DestroyWindow hwnd
 ret 1

 
 #sub DestroyThreadWindows
 int-- once; if(once) ret; else once=1
  EnumThreadWindows
 ARRAY(int) a
 GetThreadWindows a
 int i
 for i 0 a.len
	 outw a[i]
	 DestroyWindow a[i]