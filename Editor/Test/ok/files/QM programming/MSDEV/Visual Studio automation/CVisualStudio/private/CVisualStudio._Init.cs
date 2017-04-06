opt noerrorshere
int w pid uac
uac=GetProcessUacInfo(0 1)
ARRAY(int) a; int i; win("Microsoft Visual Studio" "wndclass_desked_gsk" "devenv" 0x404 "" a)
for(i 0 a.len) if(GetProcessUacInfo(a[i])=uac) w=a[i]; break
if !w
	if mes("Visual Studio not running or has different UAC IL. Run it with correct IL?" "QM - getting Visual Studio DTE" "OC")='O'
		w=sub.RunVS
GetWindowThreadProcessId(w &pid)
 dte._getactive(0 0 _s.from("!VisualStudio.DTE.7.1:" pid))
dte._getactive(0 0 _s.from("!VisualStudio.DTE.9.0:" pid))

 note: still problems with uiAccess.


#sub RunVS
function#

int w
 run "q:\app\app.sln" "" "" "" 0x32000 "app - Microsoft Visual Studio" w ;;runs without uiAccess

_s.expandpath("$program files$\Microsoft Visual Studio 9.0\Common7\IDE\devenv.exe")
int pid=CreateProcessSimple(F"''{_s}'' q:\app\app.sln" 4)
if(!pid) ret
w=wait(0 WV win("app - Microsoft Visual Studio" "wndclass_desked_gsk" pid))
if(!IsWindowEnabled(w)) clo GetLastActivePopup(w) ;;intellisense data locked by another process
ret w
