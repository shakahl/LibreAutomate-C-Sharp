 /
function# [$url]

 Waits while Opera is busy (loading page).
 Returns window handle.
 Error if fails, eg if Opera closed while waiting.
 Tested with Opera 10.63 - 12.02. May not work with other versions.

 url - web page address to open.
   If empty, just waits while Opera is busy.
   Can include command line, eg "-newpage ''url''".

 May not wait if this function opens new window when a window already exists.
 Does not work if toolbar or its Reload button does not exist, for example in full screen mode, or if the button removed. Also probably will not work if the new tab is in background.

 To get "busy" state, uses accessible object properties of Reload button.
 If your Opera uses language other than English, the default values may not match. Then the function fails (error). Open this function and change variables. You can discover the values using the 'Find Accessible Object' dialog.
 Also may need to change Opera path.


 variables to change
str windowName="Opera" ;;can be partial
str buttonName="Reload" ;;can be partial
str operaPath="$pf$\Opera\opera.exe"

 ____________________________________________

int w1=WB_Open(operaPath url windowName "OperaWindowClass" 0x4)

 find client of all tabs. Makes faster later.
Acc aca=acc("" "APPLICATION" w1 "" "" 0x1080|64)
aca=acc("" "CLIENT" aca "" "" 0x1000|64)

 wait until Reload/Stop button name changes from "Stop" to "Reload"
int n
rep
	0.2
	if(!IsWindow(w1)) end ES_FAILED
	 find Reload button. Not so easy, because each tab has its toolbar. And must be fast.
	Acc ac at ab
	if(!ac.a or ac.State&STATE_SYSTEM_INVISIBLE) ac=acc("?*" "CLIENT" aca "" "" 0x81); at.a=0; ab.a=0; n=0
	if(ac.a and !at.a) at=acc("" "TOOLBAR" ac "" "" 16|64); ab.a=0; n=0 ;;note: may not exist in fullscreen
	if(at.a) ab=acc(buttonName "PUSHBUTTON" at "" "" 16|64) ;;~25ms
	
	if(ab.a) n+1; if(n=5) break
	else n=0

ret w1
err+ end ES_FAILED

 info: Opera toolbars/buttons are very customizable. The Reload button can be anywhere.
