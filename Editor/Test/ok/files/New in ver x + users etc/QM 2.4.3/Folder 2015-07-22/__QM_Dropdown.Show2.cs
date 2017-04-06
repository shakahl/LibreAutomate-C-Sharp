 /TO_QM_ComboBox
function# ICsv'csv [hwndOwner] [RECT&rList] [int&checkStates]

 Shows a drop-down list (DDL), similar to the combo box control DDL.
 By default, returns 1-based selected item index. Returns 0 if the user closed the DDL without selecting an item, for example with the mouse (clicked the dialog or moved the mouse far away) or Esc key.
 If with check boxes, returns -1 if the user closed the DDL with the mouse or Enter key. Else returns 0 (if pressed Esc etc).

 csv - CSV string.
   First row sets control properties. Can have 1-3 columns:
     1 - selected item index (if a number). Can be empty or any text, then not used. Also not used if with check boxes.
     2 - imagelist. Can be:
       Imagelist file (if single line). To create imagelists you can use QM Imagelist Editor, look in floatng toolbar.
       List of icon files (if multiple lines or ends with a newline character).
       Imagelist handle (if a number). To load/create/delete an imagelist, you can use an __ImageList variable. This function does not delete the imagelist.
     3 - DDL flags:
       1 with check boxes. Use item flags to get/set check states.
       2 with check boxes. To get/set check states, use the checkStates parameter instead of item flags.
   Other rows set list items. Can have 1-3 columns:
     1 - item text.
     2 - item image index in imagelist.
     3 - item flags: 1 checked. If the first row has flag 1 (check boxes) and not flag 2, this function updates flag 1 in this column with the new check states, even if returns 0.
   Example CSV strings:
     Simplest: "0[]Item1[]Item2[]Item3"
     With icons: "0,$my qm$\imagelist.bmp[]Item1,0[]Item2,1[]Item3,2"
     With checkboxes: ",,1[]CheckedItem,,1[]UncheckedItem1[]UncheckedItem2,,0"
 hwndOwner - owner control handle. The DDL will be below or above the control. It will have control's width, unless it is < widthOfListItems*2. The control can be visible or hidden. The DDL will have control's font.
 rList - can be used instead of hwndOwner. The DDL will be in this rectangle on screen. It will have rList width, unless it is < widthOfListItems*2.
 checkStates - sets and receives checked item states, one item for each bit, max 32 items. If used, the list will be with checkboxes, and flag 1 not used.

 If rList and hwndOwner not used, the DDL will be under the mouse cursor. If hwndOwner not used, the DDL will be unowned, topmost window.

 TODO: item tooltips (4-th column)

class __QM_ComboList
	-ICsv'_csv -_nr -_nc
	-_hwnd -_hlv -_howner
	-!_csvFlags -!_checkBoxes -int*_checkStates
	-!_exitLoop -!_wasMouseIn -!_lbuttonDown
	-_result

 debug:
 hwndOwner=0
 RECT t; GetWindowRect win("Dialog") &t; &rList=t
 t.right=t.left
 t.bottom=t.top
 hwndOwner=_hwndqm

int iSelect1based i j; lpstr s
__ImageList il
_csv=csv; _howner=hwndOwner; _checkStates=&checkStates
_hlv=0; _csvFlags=0; _checkBoxes=0; _exitLoop=0; _wasMouseIn=0; _lbuttonDown=0; _result=0; _nr=0; _nc=0

 get options etc from CSV; create imagelist if need
if(!_csv) ret
_nr=_csv.RowCount-1; if(_nr<1) ret
_nc=_csv.ColumnCount
if _nc>1
	s=_csv.Cell(0 1)
	if !empty(s)
		i=val(s 0 j)
		if(j and !s[j]) il=i; else if(findc(s 10)>=0) il.Create(s); else il.Load(s)
	if(_nc>2) _csvFlags=val(_csv.Cell(0 2))
if(!_checkStates) _csvFlags~2; else if(_csvFlags&2=0) _checkStates=0
_checkBoxes=_csvFlags&3
if(!_checkBoxes) s=_csv.Cell(0 0); i=val(s 0 j); if(j) iSelect1based=i+1

 register class and create listview parent window (_hwnd)
__RegisterWindowClass+ __rwcQMCBP; int atom=__rwcQMCBP.atom; if(!atom) atom=__rwcQMCBP.Register("QM_ComboList" &sub.WndProc_ 4 0 CS_SAVEBITS|CS_DROPSHADOW|CS_DBLCLKS)
i=WS_EX_TOOLWINDOW|WS_EX_NOACTIVATE; if(!_howner) i|WS_EX_TOPMOST
if(!CreateWindowExW(i +atom 0 WS_POPUP|WS_BORDER 0 0 900 900 _howner 0 _hinst &this)) ret

 create/init listview
int font; if(_howner) font=SendMessage(_howner WM_GETFONT 0 0)
_hlv=CreateControl(0 "SysListView32" 0 LVS_REPORT|LVS_OWNERDATA|LVS_SINGLESEL|LVS_NOCOLUMNHEADER|LVS_SHOWSELALWAYS|LVS_SHAREIMAGELISTS 0 0 900 900 _hwnd 1010 font)
i=LVS_EX_FULLROWSELECT|LVS_EX_INFOTIP|LVS_EX_DOUBLEBUFFER|LVS_EX_TRACKSELECT; if(_checkBoxes) i|LVS_EX_CHECKBOXES
sub.LvSend(LVM_SETEXTENDEDLISTVIEWSTYLE i -1)
sub.LvSend(WM_CHANGEUISTATE MakeInt(UIS_SET UISF_HIDEFOCUS) 0)
if(il) sub.LvSend(LVM_SETIMAGELIST LVSIL_SMALL il)
TO_LvAddCol _hlv 0 0 100
sub.LvSend(LVM_SETITEMCOUNT _nr)

 get suggested x y and calculate max window height
int x y cx cy cyLvitem ho=_howner
RECT r ri; POINT p
ri.left=LVIR_BOUNDS; sub.LvSend(LVM_GETITEMRECT 0 &ri); cyLvitem=ri.bottom; cy=cyLvitem*_nr+2
 g1
if(&rList) r=rList; x=r.left; y=r.top; cx=r.right-r.left; i=r.bottom-r.top; if(i>0 and cy>i) cy=i
else if(ho) GetWindowRect ho &r; x=r.left; y=r.bottom; cx=r.right-r.left
else xm p; x=p.x; y=p.y; cx=0

 get monitor rect
int Y CX CY mon monFlags half1 half2
if(&rList) p.x=r.left+r.right/2; p.y=r.top; mon=MonitorFromPoint(p 2); monFlags=32; else if(ho) mon=ho; else mon=-1
GetWorkArea 0 Y CX CY monFlags mon
Y+10; CY-20

 calculate window height
if(&rList) half1=y-Y-20; half2=Y+CY-y-20
else if(ho) half1=r.top-Y; half2=Y+CY-y; if(half1<CY/4 and half2<CY/4) ho=0; goto g1
else half1=y-Y; half2=Y+CY-y
int above
if(cy>half2) if(half1>half2) cy=iif(cy<half1 cy half1); above=1
if(!above) cy=iif(cy<half2 cy half2)
if(above) if(&rList) y-cy+20; else if(ho) y=r.top-cy; else y-cy

 calculate window width if need
if(cx<cyLvitem*2) cx=0 ;;allows eg to use a small QM combo box as a button
if cx<=0
	cx=0
	int dc oldFont n=iif(_nr<500 _nr 500)
	SIZE z; str su.flags=1
	dc=GetDC(_hlv); oldFont=SelectObject(dc sub.LvSend(WM_GETFONT))
	for(i 0 n) su.unicode(_csv.Cell(i+1 0)); if(GetTextExtentPoint32W(dc +su su.len/2 &z) and z.cx>cx) cx=z.cx
	SelectObject(dc oldFont); ReleaseDC _hlv dc
	cx+cyLvitem; if(cyLvitem*_nr>cy-2) cx+GetSystemMetrics(SM_CXVSCROLL)
	if(cx<100) cx=100; else i=CX/2; if(cx>i) cx=i
	ri.left=LVIR_LABEL; sub.LvSend(LVM_GETITEMRECT 0 &ri); cx+ri.left
	
 move/size windows
MoveWindow _hwnd x y cx cy 0
MoveWindow _hlv 0 0 cx-2 cy-2 0

 adjust listview
TO_LvAdjustColumnWidth _hlv 1
if(iSelect1based) TO_LvSelect _hlv iSelect1based-1 2
sub.LvSend(WM_SETFOCUS)

 show window
EnsureWindowInScreen _hwnd
ShowWindow _hwnd SW_SHOWNOACTIVATE

 message loop
MSG m
rep
	if(_exitLoop or GetMessage(&m 0 0 0)<1) break
	 sel(m.message) case [WM_TIMER] case else OutWinMsg m.message m.wParam m.lParam
	sel m.message
		case WM_SYSKEYDOWN sub.EndPopup
		
		case [WM_KEYDOWN,WM_KEYUP]
		sel m.wParam
			case VK_ESCAPE sub.EndPopup; break
			case VK_RETURN sub.EndPopup(1); break
			case [VK_DOWN,VK_UP,VK_PRIOR,VK_NEXT] m.hwnd=_hlv
		
		case [WM_LBUTTONDOWN,WM_RBUTTONDOWN,WM_NCLBUTTONDOWN,WM_NCRBUTTONDOWN]
		if(GetAncestor(m.hwnd 2)!_hwnd) sub.EndPopup(2); break ;;clicked in another window of current thread, eg in the parent dialog
		
		case WM_LBUTTONUP
		if(_lbuttonDown and m.hwnd=_hlv) sub.EndPopup(1); break
		_lbuttonDown=0
	TranslateMessage &m; DispatchMessage &m

 destroy window
err+ _result=0
_exitLoop=1
DestroyWindow _hwnd; _hwnd=0
if(m.message=WM_QUIT) PostQuitMessage(m.wParam); ret

ret _result


#sub WndProc_
function# hwnd message wParam lParam

__QM_ComboList* d
if message=WM_NCCREATE
	CREATESTRUCTW* cs=+lParam; d=cs.lpCreateParams
	d._hwnd=hwnd
	SetWindowLong hwnd __rwcQMCBP.baseClassCbWndExtra d
else d=+GetWindowLong(hwnd __rwcQMCBP.baseClassCbWndExtra); if(!d) ret DefWindowProcW(hwnd message wParam lParam)

ret d.sub.WndProc(hwnd message wParam lParam)


#sub WndProc c
function# hwnd message wParam lParam

 sel(message) case [WM_TIMER] case else OutWinMsg message wParam lParam

sel message
	case WM_CREATE SetTimer hwnd 1 100 0
	case WM_DESTROY sub.EndPopup ;;if not ended
	case WM_TIMER sel(wParam) case 1 ret sub.OnTimer
	case WM_MOUSEACTIVATE ret sub.OnMouseactivate(wParam lParam)
	case WM_NOTIFY ret sub.OnNotify(+lParam)

ret DefWindowProcW(hwnd message wParam lParam)


#sub EndPopup c
function [ok] ;;ok: 0 cancel, 1 ok, 2 on mouse out/click
if(_exitLoop) ret
_exitLoop=1
KillTimer _hwnd 1
ShowWindow _hwnd 0
if(ok=2 and !_checkBoxes) ok=0
if(!ok) _result=0
else if(_checkBoxes) _result=-1
else _result=sub.LvSelected+1
PostMessage 0 0 0 0


#sub OnTimer c
if(_exitLoop) ret
if(_howner) if(!IsWindow(_howner) or IsIconic(_howner)) ret sub.EndPopup
POINT p; xm p
RECT r r2; GetWindowRect _hwnd &r; if(_howner) GetWindowRect _howner &r2; UnionRect &r &r &r2
InflateRect &r 100 100
if(PtInRect(&r p.x p.y)) _wasMouseIn=1
else if _wasMouseIn
	int hc=GetCapture; if(hc and _hwnd=GetAncestor(hc 2)) ret ;;scrolling etc
	sub.EndPopup(2)


#sub OnMouseactivate c
function# wParam lParam
if(lParam>>16!=WM_LBUTTONDOWN) ret MA_NOACTIVATEANDEAT
if(lParam&0xffff!=HTCLIENT) ret MA_NOACTIVATE ;;eg scrollbar, then allow to scroll
int i=TO_LvItemFromMouse(_hlv); if(i<0 or i>=_nr) ret MA_NOACTIVATEANDEAT
if _checkBoxes
	if(_checkStates) if(i<32) *_checkStates^1<<i
	else _csv.Cell(i+1 2)=F"{val(_csv.Cell(i+1 2))^1}"
	sub.LvSend(LVM_REDRAWITEMS i i)
else _lbuttonDown=1
ret MA_NOACTIVATEANDEAT


#sub OnNotify c
function# NMHDR*nh
if(nh.hwndFrom!_hlv) ret
sel nh.code
	case LVN_GETDISPINFOW
	NMLVDISPINFOW& di=+nh
	int image check i=di.item.iItem; if(i<0 or i>=_nr) ret
	lpstr text=_csv.Cell(i+1 0); if(_nc>1) image=val(_csv.Cell(i+1 1))
	if _checkBoxes
		if(_checkStates) check=*_checkStates>>i&1; if(i>31) ret
		else check=val(_csv.Cell(i+1 2))&1
	TO_LvSetDispInfo di text image 0 check+1
	
	case LVN_HOTTRACK
	NMLISTVIEW& nl=+nh
	 out nl.iItem
	i=nl.iItem; if(i<0) i=TO_LvItemFromMouse(_hlv) ;;on XP -1 if not on item text
	if(i>=0 and i!sub.LvSelected) TO_LvSelect(_hlv i)
	ret 1 ;;don't know why, listview does not select itself
	
	 case NM_CUSTOMDRAW ;;could be useful if we don't send wm_setfocus to _hlv; then also remove LVS_SHOWSELALWAYS because customdraw does not work with the selected item
	 NMLVCUSTOMDRAW& cd=+nh
	 sel cd.nmcd.dwDrawStage
		 case CDDS_PREPAINT ret CDRF_NOTIFYITEMDRAW
		 case CDDS_ITEMPREPAINT
		 if() cd.clrTextBk=GetSysColor(COLOR_HIGHLIGHT); cd.clrText=GetSysColor(COLOR_HIGHLIGHTTEXT) ;;note: cd.nmcd.uItemState is incorrect


#sub LvSend c
function message [wParam] [lParam]
ret SendMessageW(_hlv message wParam lParam)


#sub LvSelected c
function#
ret sub.LvSend(LVM_GETNEXTITEM -1 LVNI_SELECTED)
