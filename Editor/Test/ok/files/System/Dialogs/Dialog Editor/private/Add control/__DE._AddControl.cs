 /Dialog_Editor
function# t [flags] [___DE_CLIPBOARD&p] ;;flags: 1 start drag-drop.  With ___DE_PASTE and capturing use t=-1 and p.
if(!t) ret ;;folder

int x y fl h newid maxid; str spec tip
___DE_ADDCONTROL a
if t<0 ;;paste or capture
	if(t!-1) ret
	a.cls=p.cls; a.style=p.style; a.exstyle=p.exstyle; a.cx=p.wid; a.cy=p.hei; a.txt=p.c.txt
	x=p.x; y=p.y; fl=2
else
	a=_aadd[t-1]
	spec=a.cls

 unique id
sel spec
	case "<OK>" a.cls="Button"; if(id(IDOK _hform 1)) sub_sys.TooltipOsd "Button OK already exists in the dialog" 32; ret; else newid=IDOK
	case "<Cancel>" a.cls="Button"; if(id(IDCANCEL _hform 1)) sub_sys.TooltipOsd "Button Cancel already exists in the dialog" 32; ret; else newid=IDCANCEL
	case else
	sub.PageMap(newid maxid)
	if(newid<2) newid=2
	rep() newid+1; if(!id(newid _hform 1)) break
	if(newid>=maxid) mes "Too many controls for this page" "Error" "x"; ret

 more input, tip
sel spec
	case "<other>" if(!sub.OtherControls(a)) ret
	case "<ax>" a.cls="ActiveX"; if(!__TypeLibDialog(_hwnd &a.txt)) ret
	case "QM_Grid" tip="Click the Properties button to edit grid columns etc"
	case "SysLink" tip="Click the Events button to create on-link-click code"

_Undo

h=_CreateControl(a.exstyle a.cls a.txt a.style a.cx a.cy newid fl x y)
if h
	___DE_CONTROL& c=_ac[]
	sel(t) case -1 c=p.c; case else c.txt=a.txt
	c.cid=newid
	c.style=GetWinStyle(h)
	c.exstyle=GetWinStyle(h 1)
	
	_Select(h)
	 note: don't show a Properties dialog here, it cancels moving.
	
	if(tip.len) sub_sys.TooltipOsd tip 1 "DE" 0 0 -12 _htb
	
	if(flags&1) PostMessage _hwnd WM_APP+1 0 h
else
	_Undo(1)

ret h


#sub OtherControls c
function# ___DE_ADDCONTROL&a

str dd=
 BEGIN DIALOG
 4 "" 0x90C80A48 0x100 0 0 272 208 "Add Control"
 4 Static 0x54000200 0x4 4 8 20 13 "Class"
 5 ComboBox 0x54230241 0x4 24 8 108 176 ""
 3 QM_DlgInfo 0x54030000 0x20000 140 8 124 166 "Select from the list or enter a class name that exists in QM process.[][]QM only creates these controls and does not provide more services. To work with them (set/get control data etc), need programming at Windows API level - in dialog procedure send and receive messages etc. Windows controls are documented in the MSDN Library on the Internet. Some examples are in QM forum. Read more in QM Help (press ? in Dialog Editor)."
 1 Button 0x54030001 0x4 8 188 48 14 "OK"
 2 Button 0x54030000 0x4 60 188 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040200 "" "" "" ""

str controls = "5"
str cb5
cb5=
 ComboBoxEx32
 msctls_hotkey32
 msctls_progress32
 msctls_statusbar32
 msctls_trackbar32
 msctls_updown32
 ReBarWindow32
 RichEdit50W
 ScrollBar
 SysAnimate32
 SysDateTimePick32
 SysHeader32
 SysIPAddress32
 SysListView32
 SysMonthCal32
 SysPager
 SysTabControl32
 SysTreeView32
 ToolbarWindow32

if(!ShowDialog(dd 0 &controls _hwnd) or !cb5.len) ret
a.cls=cb5

WNDCLASSEXW w.cbSize=sizeof(w)
int retry
 gRetry
BSTR b=a.cls
if !GetClassInfoExW(0 b &w) and !GetClassInfoExW(_hinst b &w)
	if(!retry) retry=1; __CDD_LoadControlDllOnDemand(a.cls); goto gRetry
	mes "QM cannot create controls of this class." "Error" "x"; ret

a.cx=96; a.cy=12; a.style=WS_TABSTOP|WS_GROUP
sel a.cls 1
	case "ComboBoxEx32" a.style|CBS_DROPDOWN|WS_VSCROLL|CBS_AUTOHSCROLL
	case "msctls_trackbar32" a.cy=16
	case "msctls_updown32" a.cx=12
	case "RichEdit50W" a.style|WS_VSCROLL|ES_AUTOVSCROLL|ES_DISABLENOSCROLL|ES_MULTILINE|ES_WANTRETURN; a.exstyle|WS_EX_CLIENTEDGE; a.cy=48
	case "SysAnimate32" a.cy=48
	case "SysListView32" a.cy=48
	case "SysMonthCal32" a.cx=102; a.cy=98
	case "SysPager" a.cy=48
	case "SysTabControl32" a.cy=48
	case "SysTreeView32" a.cy=48
	case "#32770" a.cy=48

ret 1


#sub PageMap c
function# [int&idmin] [int&idmax]

int i=_page

ARRAY(lpstr) a
if(i>=0 and _pageMap.len and tok(_pageMap a -1 " ()" 8)>i) i=val(a[i])

if i<0
	if(&idmin) idmin=1
	if(&idmax) idmax=1000
else
	if(&idmin) idmin=i*100+1000
	if(&idmax) idmax=i*100+1100

ret i
