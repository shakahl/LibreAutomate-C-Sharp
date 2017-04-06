function $taskName isXP

if(empty(taskName)) taskName=0
int w htv hlv pid

 set a hook to wait for Task Scheduler window and make transparent ASAP
if taskName
	sub_sys.TooltipOsd "Please wait..." 8|32 "ts wait" -1 0 -30
	int hh=SetWinEventHook(EVENT_OBJECT_CREATE EVENT_OBJECT_CREATE 0 &sub.HookProc 0 0 WINEVENT_OUTOFCONTEXT)
	opt waitmsg 1

 run Task Scheduler. Always starts new process.
run "$system$\mmc.exe" "$system$\taskschd.msc" "" "" 0x20300

if(!taskName) goto gWaitAndUpdateQmList

 wait for Task Scheduler window
wait 30 V w
wait 30 WV w

 select "Task Scheduler Library\Quick Macros" in treeview
spe 10
htv=id(12785 w)
act htv ;;error here if QM as User
rep 50
	int htvi=SendMessage(htv TVM_GETNEXTITEM TVGN_CHILD SendMessage(htv TVM_GETNEXTITEM 0 0)) ;;"Task Scheduler Library"
	if(htvi) break
	0.1
if isXP
	SendMessage(htv TVM_SELECTITEM TVGN_CARET htvi)
else
	rep(50) if(SendMessage(htv TVM_EXPAND TVE_EXPAND htvi)) break; else 0.1 ;;note: don't wait for TVM_GETITEMSTATE TVIS_EXPANDED
	Acc a.Find(htv "OUTLINEITEM" "Quick Macros" "value=2" 0x1005 10)
	a.Select(2) ;;works better than TVM_SELECTITEM

 select the task in listview, and open its Properties dialog
0.2; hlv=wait(30 WV child("" "*.SysListView32.*" w 0x400 "wfName=listViewMain")) ;;the slowest part; the listview appears after ~2 s
a.Find(hlv "LISTITEM" taskName "" 0x1015 10)
a.Select(2)
PostMessage hlv WM_KEYDOWN VK_RETURN 0 ;;open Properties. DoDefaultAction just selects, men() etc don't work or unreliable. This works without focusing hlv.

 wait for task Properties dialog
GetWindowThreadProcessId(w &pid)
int w1=wait(10 WV win(taskName "" pid))

 on error let user open Properties dialog manually
err+
	if taskName
		SetWinStyle(w 0 4)
		Transparent w 256
		act w; err
		_s=F"To edit the task, in 'Task Scheduler' window double click task[][]{taskName}"
		if(!isXP) _s+"[][]It is in subfolder 'Quick Macros'."
		if(GetProcessUacInfo(0 1)=4) _s+"[][]Info: QM cannot open the task Properties dialog when running as User. Look in Options -> General."
		OsdHide "ts wait"
		sub_sys.TooltipOsd _s 8 "ts failed"
	goto gWaitAndUpdateQmList

 select Triggers tab
int c1=child("" "*SysTabControl32*" w1 0x0 "wfName=tabControlDetail") ;;page tab list
SelectTab c1 1
err+
OsdHide "ts wait"

 close Task Scheduler when user closes the Properties dialog. Reusing would be difficult, eg need to refresh.
wait 0 WD w1
clo w
__ScheduleUpdated
ret

 gWaitAndUpdateQmList
if(!w) w=wait(30 WV "+MMCMainFrame")
2
rep
	2
	__ScheduleUpdated
	if(!IsWindow(w)) break

err+

 notes:
 Fails if QM not admin/uiAccess, because mmc process is always admin. Could use /exe 2, never mind.
 Don't use strings, may be not English.
 Works even if TS window inactive, although a.Select(2) activates.


#sub HookProc v
function hHook event hwnd idObject idChild dwEventThread dwmsEventTime

 EVENT_OBJECT_CREATE hook.
 When Task Scheduler window created, unhooks, makes it transparent and removes taskbar button.
 Works even if QM as User, just cannot make transparent etc.

if(idObject) ret
if(!WinTest(hwnd "MMCMainFrame")) ret
UnhookWinEvent hHook
w=hwnd
SetWinStyle(hwnd WS_EX_TOOLWINDOW 4)
Transparent hwnd 0
err+
