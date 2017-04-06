 /
function'SHDocVw.InternetExplorer [hwnd]

 Gets SHDocVw.InternetExplorer of folder that is opened in Windows Explorer (WE).

 hwnd - WE window handle. If 0 or omitted, finds WE window that is above all other WE windows in Z order.


SHDocVw.ShellWindows sw._create
SHDocVw.InternetExplorer ie
type __GFWIE hwnd SHDocVw.InternetExplorer'ie
ARRAY(__GFWIE) a
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
		if(hwnd2=hwnd) ret ie
	else
		__GFWIE& r=a[]
		r.hwnd=hwnd2; r.ie=ie

if(hwnd) ret

hwnd=GetTopWindow(0)
rep
	if(!hwnd) break
	int i
	for(i 0 a.len) if(a[i].hwnd=hwnd) ie=a[i].ie; ret ie
	hwnd=GetWindow(hwnd GW_HWNDNEXT)
