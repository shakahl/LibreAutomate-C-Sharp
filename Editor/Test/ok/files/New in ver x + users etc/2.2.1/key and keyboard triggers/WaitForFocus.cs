 /
function# [^waitmax] ;;waitmax: 0 is infinite.

 Waits until a child window in currently active window has focus.
 Returns its handle.
 On timeout throws error.


 int t0=GetTickCount
 opt waitmsg -1
 GUITHREADINFO g.cbSize=sizeof(g)
 rep
	 if(GetGUIThreadInfo(0 &g) and g.hwndFocus and IsChildWindow(g.hwndFocus)) ret g.hwndFocus
	 if(waitmax>0 and GetTickCount-t0>=waitmax*1000) end "wait timeout"
	 0.01

ret wait(waitmax WA child())
