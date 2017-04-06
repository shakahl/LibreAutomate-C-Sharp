 /MMT_Main
function

 This function manages most things.
 Gets window list, icons, adds/removes buttons, etc.
 Called every 500 ms.


MMTVAR- v
int i j ra h
TBBUTTONINFOW bi.cbSize=32

 first time init
if !v.il
	v.il=ImageList_Create(16 16 ILC_MASK|ILC_COLOR32 0 10)
	SendMessage v.htb TB_SETIMAGELIST 0 v.il
	SendMessage v.htb TB_BUTTONSTRUCTSIZE sizeof(TBBUTTON) 0
	v.bwidth=160

 get windows
ARRAY(MMTWINMON) a
MMT_GetWindows a

 remove buttons?
for i v.a.len-1 -1 -1
	h=v.a[i].hwnd
	for(j 0 a.len) if(a[j].hwnd=h) break
	if j=a.len or a[j].monitor!=v.monitor
		ra|1
		SendMessage v.htb TB_DELETEBUTTON i 0
		v.a.remove(i)
		ImageList_Remove v.il i
		bi.dwMask=TBIF_BYINDEX|TBIF_COMMAND|TBIF_IMAGE
		for j i v.a.len
			bi.idCommand=j; bi.iImage=j
			SendMessage v.htb TB_SETBUTTONINFOW j &bi

 add buttons?
for i 0 a.len
	if(a[i].monitor!=v.monitor) continue
	h=a[i].hwnd
	for(j 0 v.a.len) if(v.a[j].hwnd=h) break
	if j=v.a.len
		ra|2
		MMTWINDOW& r=v.a[]
		r.hwnd=h
		 icon
		int hicon=GetWindowIcon(h)
		if(!hicon) hicon=GetFileIcon("$qm$\empty.ico")
		ImageList_ReplaceIcon(v.il -1 hicon)
		DestroyIcon hicon
		 add button
		TBBUTTON t
		t.idCommand=j
		t.iBitmap=j
		t.fsStyle=BTNS_NOPREFIX|BTNS_CHECKGROUP
		t.fsState=TBSTATE_ENABLED
		t.iString=-1 ;;why cannot add wide string?
		SendMessage v.htb TB_ADDBUTTONSW 1 &t
		 set button text and width
		bi.dwMask=TBIF_TEXT|TBIF_SIZE
		bi.pszText=@r.text.getwintext(h); err
		bi.cx=v.bwidth
		SendMessage v.htb TB_SETBUTTONINFOW j &bi

 button width
if(ra&3) MMT_SetButtonWidth

 check active window's button
h=win
int ho=GetWindow(h GW_OWNER); if(ho and ho!=GetDesktopWindow and GetWinStyle(h 1)&WS_EX_APPWINDOW=0) h=ho
for i 0 v.a.len
	if(v.a[i].hwnd=h)
		ra|8
		SendMessage v.htb TB_CHECKBUTTON i 1
		break
if ra&8=0
	for(i 0 v.a.len) SendMessage v.htb TB_CHECKBUTTON i 0

 text changed?
for i 0 v.a.len
	&r=v.a[i]
	_s.getwintext(r.hwnd); err continue
	if(_s!=r.text)
		r.text.Swap(_s)
		bi.dwMask=TBIF_TEXT
		bi.pszText=@r.text
		SendMessage v.htb TB_SETBUTTONINFOW i &bi

 remove buttons in Windows taskbar. MMTWINDOW dtor will restore.
 Always remove because Windows (<Win7) restores it when the window is activated.
 This is the slowest part. About 0.8 ms (80%).
for i 0 v.a.len
	MMT_WinTaskbarAddRemoveButton 2 v.a[i].hwnd
