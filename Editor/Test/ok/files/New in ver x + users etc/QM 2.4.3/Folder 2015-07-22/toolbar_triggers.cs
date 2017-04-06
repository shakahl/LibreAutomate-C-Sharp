 Edit the sel/case code. Add cases for your windows.
 Then run this function. For example assign trigger "QM file loaded" (don't check Synchronous).
 Its thread runs all the time. Watches for "window activated" events and runs your code.
 To end this thread, run this function again.
 To apply your new sel/case code without ending thread, click Compile button in QM toolbar.
 Your code can use mac to run toolbars and other macros. Your code must be as fast as possible.


function hHook event hwnd idObject idChild dwEventThread dwmsEventTime
if(hHook) goto gHook

if(getopt(nthreads)>1) EndThread "toolbar_triggers"; ret

int hh=SetWinEventHook(EVENT_SYSTEM_FOREGROUND EVENT_SYSTEM_FOREGROUND 0 &toolbar_triggers 0 0 WINEVENT_OUTOFCONTEXT)
if(!hh) end F"{ERR_FAILED}. {_s.dllerror}"
opt waitmsg 1
wait -1
UnhookWinEvent hh
ret


 gHook
str sClass.getwinclass(hwnd) sText.getwintext(hwnd) sProgram
 sProgram.getwinexe(hwnd) ;;this is slow, use only when really need

 examples
sel _s.from(sClass "[]" sText) 3
	case "Notepad[]*" mac "Notepad toolbar" hwnd ;;note: the second argument must be hwnd, it attaches the toolbar to the window. Subsequent window activations will not create more instances of this toolbar.
	
	case "#32770[]Options" mac "Options toolbar" hwnd
	
	 more cases ...
