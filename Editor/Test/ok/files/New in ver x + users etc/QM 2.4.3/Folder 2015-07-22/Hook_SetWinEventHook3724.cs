 Run this function, it should show "window activated" events in QM output.
 It uses the same type of hooks that QM uses for window triggers.
 If there is no output:
   Maybe hooks don't work only in QM process. To test it:
     Make exe from this function. Double click it in Windows Explorer to run. If events work, it should show in QM output. If no output:
       Test events with accessibility testing/debugging tools. One is attached to this post. It should show events, no setup required. Test its options "in context" and "out of context".
 Note that events may not work with windows of programs that run as administrator. Test with non-admin program windows, eg with Windows Explorer folder windows.

 If events work in this function, I'll create a function that replaces QM window triggers. Until QM triggers will be fixed. Maybe already fixed, try to download QM 2.4.2 if you have an older version.
 If events will not work in this function and in the attached program, it means that something wrong is with your Windows. I tried to find something about on the internet, unsuccessfully. Try to disable antivirus, close as many running programs as possible, maybe then events will work, then maybe you can find the reason.


function hHook event hwnd idObject idChild dwEventThread dwmsEventTime
if(hHook) goto gHook

out
int hh=SetWinEventHook(EVENT_SYSTEM_FOREGROUND EVENT_SYSTEM_FOREGROUND 0 &Hook_SetWinEventHook3724 0 0 WINEVENT_OUTOFCONTEXT)
if(!hh) end F"{ERR_FAILED}. {_s.dllerror}"
mes "Now activate several windows and see QM output. Don't close this message box until you finish testing."
UnhookWinEvent hh
ret


 gHook
str sc.getwinclass(hwnd); err
str sn.getwintext(hwnd); err
out "event=%i hwnd=%i idObject=%i idChild=%i dwEventThread=%i    window class=''%s'', name=''%s''" event hwnd idObject idChild dwEventThread sc sn
