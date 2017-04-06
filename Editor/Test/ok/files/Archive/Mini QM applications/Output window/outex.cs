 /
function [~s] [$tab] [flags] ;;flags: 1 display time

 Displays s in a multitab output window.
 If used without arguments, clears text of the tab.

 To create the window, call outex_create_window.

 Like in QM, the output is asynchronous, and therefore fast.
 Does not support tags.

 s - a string or number to display.
 tab - destination tab name. Default: name of current thread.


int+ __outex_hwnd ;;faster
if(!(IsWindow(__outex_hwnd) and wintest(__outex_hwnd "" "QM_outex"))) __outex_hwnd=win("" "QM_outex"); if(!__outex_hwnd) ret

str stab stim

if(empty(tab))
	int iid=getopt(itemid 3)
	if(iid) stab.getmacro(iid 1); else stab="QM"
else stab=tab

if(flags&1)
	SYSTEMTIME t; GetLocalTime &t
	stim.format("%i:%02i:%02i.%04i.  " t.wHour t.wMinute t.wSecond t.wMilliseconds)

if(getopt(nargs)) s.from(stab ":" stim s "[]")
else s=stab ;;clear

SendMessageW __outex_hwnd WM_SETTEXT 1 @s
