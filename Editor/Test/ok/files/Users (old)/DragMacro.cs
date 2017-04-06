 \
function# [iid] [FILTER&f]

 this part runs as filter function, on right button down event

 if clicked QM tree view, it launches itself without arguments
if(getopt(nargs)=2)
	if(f.hwnd=_hwndqm and GetDlgCtrlID(f.hwnd2)=2202) mac iid; ret
	ret -2
 _________________________________

 this part runs as normal function

 get QM item id from tree view
int htv=id(2202 _hwndqm)
TVHITTESTINFO ht
GetCursorPos(&ht.pt); ScreenToClient(htv &ht.pt)
SendMessage(htv TVM_HITTEST 0 &ht)
if(ht.flags&0x46=0) ret
iid=TvGetParam(htv ht.hItem)
str name.getmacro(iid 1)

 wait until right button is released
rep() 0.1; if(GetKeyState(2)&0x8000=0) break

 is it desktop?
int h=win(mouse)
if(!wintest(h "" "Progman")) ret

 get number of icons on desktop
int hlv=id(1 h)
int i=SendMessage(hlv LVM_GETITEMCOUNT 0 0)

 create shortcut
str sp.format("$desktop$\%s.lnk" name)
str sa.format("M ''%s''" name)
CreateShortcut sp "qmcl.exe" sa

 wait until new icon is added
rep(100) 0.1; if(SendMessage(hlv LVM_GETITEMCOUNT 0 0)>i) break

 move the icon to the mouse position
POINT p; xm p hlv 1
SendMessage(hlv LVM_SETITEMPOSITION i p.y<<16|p.x)
