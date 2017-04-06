 /
function# $items [$text] [$caption] [flags] [hwndOwner] [x] [y] [timeoutS] [default] [`icons] ;;items: "item1[]item2[]...".    flags: 1 don't activate, 2 item IDs, 4 image indices, 8 wider, 16 fixed height, 32 32x32 icons, 64 raw x y, 128 mouse x y.

 Shows list box dialog.
 Returns 1-based index of selected item, or 0 on Cancel.

 items - list of items, like "item1[]item2[]item3". Each line adds one item to the list.
   Item that begins with & is initially selected.
   If empty, list is not displayed, and there is more space for text.
 text - text above list. Supports <help #IDP_SYSLINK>links</help>.
 caption - dialog title bar text.
 flags:
   1 - don't activate dialog window initially.
   2 - each line begins with a number (item ID) that will be returned instead of line index. See the second example.
   4 - each line begins with a 0-based image index (see icons). If flag 2 also used, there are 2 numbers, like "5 3 Text".
   8 - make dialog wider.
   16 - don't change dialog height depending on the number of items.
   32 - when using list of icons, display 32x32 icons instead of 16x16.
   64 - raw x y. It disables special processing of 0 and negative x y.
   128 - show at the mouse position.
 hwndOwner - owner window handle. The dialog will be on top of the owner window.
   The owner window will be disabled, if belongs to the same thread.
 x, y - coordinates. If <0, relative to the right or bottom edge. If 0 (default), at screen center.
 timeoutS - max time (seconds) to show dialog. Default: 0 (infinite). To stop countdown, click the list box.
 default - return this value on timeout. Default: 0. Also selects that item, if & not used.
 icons - imagelist or list of icons. Can be:
   .bmp file created with the Imagelist Editor. Supports <help #IDP_RESOURCES>macro resources</help> (QM 2.4.1) and exe resources.
   List of icons. Example: "file.ico[]file.dll,5[]file.txt[]resource:file.ico[]:5 file.ico". If single line, add newline, like "file.ico[]".
   Imagelist handle. See <help>__ImageList.Load</help>. ListDialog does not delete the imagelist.

 Added in: QM 2.4.1. Replaces <help>list</help>.
 See also: <ShowMenu>, <DynamicMenu>, <ShowDropdownList>.

 EXAMPLES
 sel ListDialog("Line1[]Line2[]Line3" "Lines")
	 case 1 out "Line1"
	 case 2 out "Line2"
	 case 3 out "Line3"
	 case else out "Cancel"
 
  numbered
 str s=
  10 Line1
  15 Line2
 sel ListDialog(s "Lines" "" 2)
	 case 10 out "Line1"
	 case 15 out "Line2"
	 case else out "Cancel"
 
  get string
 str s="one[]two"
 int i=ListDialog(s); if(i=0) ret
 s.getl(s i-1)
 out s
 
  with array
 ARRAY(str) a="one[]two"
 str s=a
 int i=ListDialog(s); if(i=0) ret
 out a[i-1]


type ___LD41 $s r ii
type ___LD42 flags timeout default ARRAY(___LD41)a iSel BSTR'dispB dispI il
___LD42 z.flags=flags; z.timeout=timeoutS; z.default=default

int style(0x80C008CD) exStyle(0x10189) lvStyle(0x50A1504D) caWidth(150) stHeight(20)
lpstr cancel("Cancel"); str st sc
if(flags&1=0) style|WS_VISIBLE|DS_SETFOREGROUND
if(hwndOwner) exStyle~WS_EX_TOPMOST
if(empty(items)) cancel="Close"; lvStyle~WS_VISIBLE; stHeight=88
if(timeoutS>0) caWidth=100
if(empty(caption)) caption=iif(empty(items) "QM - Information" "QM - Select"); else sc=caption; caption=sc.escape(1)
st=text; st.escape(1)

int i n(numlines(items)) j isSel ii; lpstr s(items) s2
z.a.create(n)
for i 0 n
	ii=-1; isSel=0
	if(s[0]=='&') s+1; isSel=1
	 retval
	if(z.flags&2) j=__Val(s &s2 4); if(s2>s) s=s2; if(s[0]==32) s+1
	else j+1
	 image
	if(z.flags&4) _i=__Val(s &s2 4); if(s2>s) ii=_i; s=s2; if(s[0]==32) s+1
	else ii=i
	
	z.a[i].s=s; z.a[i].r=j; z.a[i].ii=ii
	if(isSel or (j=z.default and !z.iSel)) z.iSel=i+1
	s=strchr(s 10)+1

sel icons.vt
	case VT_I4 z.il=icons.lVal
	case VT_BSTR
	lvStyle~LVS_SHAREIMAGELISTS
	_s=icons.bstrVal
	z.il=__ImageListLoad(icons iif(findc(_s 10)<0 0 MakeInt(2 flags&32)))

if(flags&16=0) int H=sub.LvHeight(z)-64; if(H<0) H=0
if(flags&8) int W=150

str dd=
F
 BEGIN DIALOG
 1 "" {style} {exStyle} 0 0 {150+W} {107+H} "{caption}"
 4 SysListView32 {lvStyle} 0x0 0 24 {150+W} {68+H} ""
 2 Button 0x50030000 0x0 0 {92+H} {caWidth+W} 14 "{cancel}"
 3 _SysLink 0x54030080 0x0 4 2 {142+W} {stHeight} "{st}"
 9 Static 0x54000002 0x0 {100+W} {94+H} 44 10 ""
 END DIALOG

#if !EXE
__PlayQmSound 4
#endif
ret ShowDialog(dd &sub.DlgProc 0 hwndOwner (flags&1*128)|(flags&64) iif(flags&128 DS_CENTERMOUSE 0) 0 &z x y)


#sub DlgProc
function# hDlg message wParam lParam

___LD42& z=+DT_GetParam(hDlg)
sel message
	case WM_INITDIALOG goto gInit
	
	case WM_TIMER
	sel wParam
		case 5000 ;;timeout countdown
		if(z.timeout<1) KillTimer(hDlg 5000); ret
		z.timeout-1
		if z.timeout>0
			_s=z.timeout
			_s.setwintext(id(9 hDlg))
		else
			KillTimer(hDlg wParam)
			DT_EndDialog(hDlg z.default)
	
	case WM_SETCURSOR
	sel(lParam>>16) case [WM_LBUTTONDOWN,WM_RBUTTONDOWN] z.timeout=0
	
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK goto gSelect
ret 1
 __________________________

 gInit
int i hlv=GetDlgItem(hDlg 4)
if(z.il) SendMessage hlv LVM_SETIMAGELIST LVSIL_SMALL z.il
SendMessage hlv LVM_SETEXTENDEDLISTVIEWSTYLE 0 LVS_EX_FULLROWSELECT|LVS_EX_INFOTIP
TO_LvAddCol hlv 0 0 100
z.dispI=-1
SendMessage hlv LVM_SETITEMCOUNT z.a.len 0
TO_LvAdjustColumnWidth hlv 1
if z.iSel
	LVITEMW li.stateMask=LVIS_SELECTED|LVIS_FOCUSED; li.state=LVIS_SELECTED|LVIS_FOCUSED
	SendMessage hlv LVM_SETITEMSTATE z.iSel-1 &li
	SendMessage hlv LVM_ENSUREVISIBLE z.iSel-1 0
if(GetWinStyle(hDlg 1)&WS_EX_TOPMOST) Zorder SendMessage(hlv LVM_GETTOOLTIPS 0 0) HWND_TOPMOST SWP_NOACTIVATE
if(z.timeout>0) SetTimer(hDlg 5000 1000 0)
if(z.flags&1) hid- hDlg
ret
 __________________________

 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 4
	sel nh.code
		case LVN_GETDISPINFOW
		NMLVDISPINFOW* ndi=+nh; LVITEMW& u=ndi.item
		i=u.iItem
		if(u.mask&LVIF_TEXT)
			if(i!=z.dispI) _s.getl(z.a[i].s 0); z.dispB=_s; z.dispI=i
			u.pszText=z.dispB
		if(u.mask&LVIF_IMAGE) u.iImage=z.a[i].ii
		
		case NM_CLICK
		 gSelect
		i=SendDlgItemMessage(hDlg 4 LVM_GETNEXTITEM -1 LVNI_SELECTED)
		if(z.flags&2 and i>=0) i=z.a[i].r; else i+1
		DT_EndDialog(hDlg i)
		
		case LVN_KEYDOWN
		z.timeout=0
		NMLVKEYDOWN* nk=+nh
		sel(nk.wVKey) case VK_SPACE goto gSelect ;;let Space behave like Enter
		
	case 3
	i=__SysLinkOnClick(hDlg nh)
	if(i) DT_EndDialog(hDlg i)
ret

err+


#sub LvHeight
function# ___LD42&z

 Calculates required listview height depending on num items etc.
 Returns height in dialog units. Returns 0 if failed or !z.a.len.

if(!z.a.len) ret

 to calc listview item height, create temp control. Dirty but reliable.
int _hlv=CreateWindowExW(0 L"SysListView32" 0 LVS_REPORT|LVS_SHAREIMAGELISTS|LVS_OWNERDATA|LVS_NOCOLUMNHEADER 0 0 44 44 HWND_MESSAGE 0 _hinst 0)
SendMessageW _hlv WM_SETFONT _hfont 0
if(z.il) SendMessageW _hlv LVM_SETIMAGELIST LVSIL_SMALL z.il
SendMessageW _hlv LVM_SETITEMCOUNT 1 0
RECT r; SendMessageW _hlv LVM_GETITEMRECT 0 &r
DestroyWindow _hlv
int lvih(r.bottom-r.top) lvh(lvih*z.a.len)

 get font height
int dc(CreateCompatibleDC(0)) _f(SelectObject(dc _hfont))
TEXTMETRICW t; GetTextMetricsW(dc &t)
SelectObject(dc _f); DeleteDC dc
if(!t.tmHeight) ret

 limit listview height
int wah; GetWorkArea 0 0 0 wah 0 _monitor
wah-t.tmHeight*9
if(lvh>wah) lvh=wah

 convert listview height to dialog units
ret MulDiv(lvh+2 8 t.tmHeight)
ret lvh
