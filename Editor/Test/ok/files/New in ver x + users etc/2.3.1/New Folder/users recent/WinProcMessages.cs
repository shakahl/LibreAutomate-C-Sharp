function [nCode] [wParam] [CWPSTRUCT*m]

if(getopt(nargs)=0)
	int+ g_whook=SetWindowsHookEx(WH_CALLWNDPROC &WinProcMessages 0 GetCurrentThreadId)
	mes 1
	
	 opt waitmsg 1
	 wait -1 -V g_whook
	ret

sel m.message
	case WM_LBUTTONDBLCLK
	out "double"

	case WM_MOVE
	out "move"
   
	case WM_SIZE
	out "size"

ret CallNextHookEx(g_whook nCode wParam m)