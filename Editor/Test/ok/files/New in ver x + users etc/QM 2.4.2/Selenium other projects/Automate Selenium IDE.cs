int wf pid
wf=win("- Mozilla Firefox" "Mozilla*WindowClass" "" 0x4)
act wf
GetWindowThreadProcessId(wf &pid)
int ws=win("Selenium IDE" "Mozilla*WindowClass" pid 0x4)
if !ws
	key CAs ;;or A{tn}
	ws=wait(10 WV win("Selenium IDE" "Mozilla*WindowClass" pid 0x4))
act ws
 key Co
Acc a.FindFF(ws "toolbarbutton" "" "id=record-button" 0x2004)
if(a.a and a.WebAttribute("checked")="true") a.DoDefaultAction
