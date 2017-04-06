 /
function hhook event hwnd idObject idChild dwEventThread dwmsEventTime
if(!hwnd or idObject or idChild or IsChildWindow(hwnd)) ret

 This function is called whenever a window (hwnd) is created. Edit it.
 Must return as soon as possible. Must not execute any code that would
 interact with applications (instead use mac to start other macros).

 This just filters some not useful windows to make debugging easier.
sel _s.getwinclass(hwnd) 1
	case ["SysShadow"] ret

 This code shows what window is created
str sc.getwinclass(hwnd) st.getwintext(hwnd) se.getwinexe(hwnd)
out "class=%s, text=%s, exe=%s" sc st se

  To filter windows and launch macros, here you can use code like this
 if(wintest(hwnd "window text" "window class" ...))
	 mac "some macro"
 else if(wintest(hwnd "window text" "window class" ...))
	 mac "another macro"
 ...
