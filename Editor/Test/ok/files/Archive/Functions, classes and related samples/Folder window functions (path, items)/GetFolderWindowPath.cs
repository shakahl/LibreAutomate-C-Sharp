 /
function# str&s [hwnd]

 Gets full path of folder that is opened in Windows Explorer (WE).
 Returns WE window handle, or 0 on failure.

 s - str variable that will receive the path.
 hwnd - WE window handle. If 0 or omitted, finds WE window that is above all other WE windows in Z order.


SHDocVw.ShellWindows sw._create
SHDocVw.InternetExplorer ie
ARRAY(STRINT) a
int hwnd2
str ss
foreach ie sw
	ss=ie.LocationURL; err continue
	 out ss
	if(!ss.begi("file:///")) continue ;;web browser or non-file-system folder
	ss.get(ss 8)
	ss.findreplace("/" "\")
	ss.escape(8)
	 out ss
	hwnd2=ie.HWND; err continue
	if(!hwnd2) continue
	if(hwnd)
		if(hwnd2=hwnd) s=ss; ret hwnd
	else
		STRINT& r=a[a.redim(-1)]
		r.i=hwnd2; r.s=ss

if(hwnd) ret

hwnd=GetTopWindow(0)
rep
	if(!hwnd) break
	int i
	for(i 0 a.len) if(a[i].i=hwnd) s=a[i].s; ret hwnd
	hwnd=GetWindow(hwnd GW_HWNDNEXT)
