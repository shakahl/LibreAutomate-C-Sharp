function'WTI* ^waitMax $txt [flags] [matchIndex] ;;flags: 0 partial, 1 full, 2 with *?, 3 regexp, 4 case insens., 8 +invisible, 0x100 continuous capturing

 Waits until specified text item appears in window.
 Returns address of internal variable that contains item properties.

 waitmax - max time to wait, in seconds. 0 is infinite. Error on timeout.
 txt, flags, matchIndex - text to find. Same as with <help>WindowText.Find</help>.
   other flags:
     0x100 - use continuous text capturing mode. Read more in remarks.

 REMARKS
 Repeatedly calls Capture() and Find(), until finds the specified text item.
 In some windows may use up to 30% CPU. Look in Task Manager. The process using CPU is the target process. To minimize overhead, hwnd of Init() should be the control that contains the text, not whole top-level window; also you can use a rectangle. Also try flag 0x100.
 The return value becomes invalid when the variable is destroyed or when a text capturing function called again.
 Init() must be called to set target window.

 About continuous text capturing mode:
   In this mode, continuously captures new text as it appears in the window.
   When not in this mode, repeatedly captures all existing text.
   Advantages:
     Uses much less CPU.
     Faster response.
     Prevents text cursor flickering in some windows.
     In some windows may capture text that normally wouldn't, eg while web browser is [re]loading page.
   Disadvantages:
     In some windows does not work.
     In some windows in some conditions results may be incorrect, eg may get already erased text.

 EXAMPLES
  wait for text "findme" in Notepad, and click it
 int w=id(15 win("Notepad" "Notepad"))
 WindowText x.Init(w)
 x.Mouse(1 x.Wait(0 "findme" 0x100))
 
  wait max 30 s for text "findme" in rectangle
 RECT r; SetRect &r 10 10 150 100
 x.Init(w r)
 x.Wait(30 "findme")
 err out "timeout"; ret
 out "text ''findme'' is in the rectangle"


if(waitMax<0 or waitMax>2000000 or empty(txt)) end ERR_BADARG
opt waitmsg -1

int wt(waitMax*1000) t0(timeGetTime) con flagsAdd hMain(GetAncestor(m_hwnd 2))

 g1
flagsAdd=0
con=flags&0x100!0
if(con) flagsAdd=WT_WAIT_BEGIN

rep
	if IsIconic(hMain) or (!IsWindowVisible(m_hwnd) and IsWindow(m_hwnd)) ;;cannot capture. Also would be problems with continuous.
		if(con) con=1; flagsAdd=WT_WAIT_BEGIN
		0.1; continue
	
	int _t=timeGetTime
	
	Capture(flagsAdd)
	WTI* t=Find(txt flags&0xff matchIndex)
	if(t) break
	
	int te=timeGetTime-t0
	if(wt and te>=wt) break
	
	double _wt _wt2=timeGetTime-_t/500.0
	if(con) _wt=0.1; if(con=1) con=2; flagsAdd=WT_WAIT; _wt2=0
	else _wt=sqrt(te+200)/200; if(_wt>1) _wt=1 ;;0 s 0.07, 1 s 0.17, 10 s 0.5
	 out "%.3g  %.3g" _wt _wt2
	wait _wt+_wt2

err+ int e=1

m_tc.End ;;easier to end continuous capturing

if(e) end _error
if(!t) end ERR_TIMEOUT

 wait for mouse up, because may call Mouse() which would drag etc
int mbdown=0
rep() ifk((1)) 0.1; mbdown=1; else break
if(mbdown) goto g1 ;;caller may need text position, which may change while waiting for mouse button
ret t
