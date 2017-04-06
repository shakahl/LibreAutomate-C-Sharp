 /EnumIcons
function# hModule lpszType lpszName lParam

 out lpszName
int hicon
 #if _winnt
 hicon=LoadImage(hModule +lpszName 1 16 16 0)
 #else
int h1 h2 h3 iid
h1=FindResource(hModule +lpszName +lpszType)
h2=LoadResource(hModule h1)
h3=LockResource(h2)
iid=LookupIconIdFromDirectoryEx(+h3 1 16 16 0)
h1=FindResource(hModule +iid +RT_ICON)
h2=LoadResource(hModule h1)
h3=LockResource(h2)
 hicon=CreateIconFromResource(+h3 SizeofResource(hModule h1) 1 0x00030000)
hicon=CreateIconFromResourceEx(+h3 SizeofResource(hModule h1) 1 0x00030000 16 16 0)
if(!hicon)
	out _s.dllerror
	out GetLastError
	ret
 #endif

int dc=GetDC(id(2201 _hwndqm))
 out hicon
 out dc
int+ iconx; iconx+16; if(iconx>200) iconx=0
DrawIconEx(dc iconx 0 hicon 16 16 0 0 3)
ret 1
