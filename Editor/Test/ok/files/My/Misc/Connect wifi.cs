 Now don't need this macro. It seems Windows fixed the bug when we need to enter passord etc each time.

int hwnd=TriggerWindow
 g1
Acc a.Find(hwnd "LISTITEM" "7K*" "" 0x1001 15); err ret
rep 100
	0.1
	a.DoDefaultAction
	Acc ac.Find(hwnd "PUSHBUTTON" "Connect|Disconnect" "" 0x2)
	err ret ;;once was error: window not found, the handle invalid
	if(ac.a) break
if(!ac.a) ret
sel(ac.Name) case "Disconnect" ret ;; 0.1; ac.DoDefaultAction; 0.5; goto g1 ;;may remove 7K from the list etc
rep 100
	0.1
	ac.DoDefaultAction
	err ;;often error, although does the job
	Acc at.Find(hwnd "TEXT" "" "state=0x20100004 0x20000040" 0x4)
	err ret ;;sometimes window handle invalid
	if(at.a) break
if(!at.a) ret
at.SetValue("jslapta1")
Acc an.Find(hwnd "PUSHBUTTON" "Next" "" 0x1001 5)
an.DoDefaultAction
Acc ay.Find(hwnd "PUSHBUTTON" "Yes" "" 0x1001 5)
ay.DoDefaultAction

 note: everything is async...
