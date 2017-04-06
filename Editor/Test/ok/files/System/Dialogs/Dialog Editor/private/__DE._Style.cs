 \Dialog_Editor

str controls = "4 6 7 10"
str lb4WS lb6CS cb7Typ e10Add

str s cls
lpstr pr amp comm
___DE_CONTROL& c=subs.GetControl(_hsel)
int i tmask st1(c.style) exst1(c.exstyle) stmask(-1) stUnknown(st1&0xffff) ccs
int flags ;;1 no Type control, 2 no Add control
int ccsType(CCS_TOP|CCS_NOMOVEY|CCS_BOTTOM|CCS_VERT|CCS_LEFT|CCS_NOMOVEX|CCS_RIGHT) ccsCommon(ccsType|CCS_NORESIZE|CCS_NOPARENTALIGN|CCS_NODIVIDER)

long j
ARRAY(str) ac as
ARRAY(long) ic
ARRAY(int) is it

sel cls.getwinclass(_hsel) 1
	case "#32770" pr="DS"; flags|2
	case "Button" pr="BS"; tmask=BS_TYPEMASK
	case ["Edit","QM_Edit"] pr="ES"; stmask=0x3ff7
	case ["RichEdit20A","RichEdit20W","RichEdit50W"] pr="ES"
	case "Static" pr="SS"; tmask=SS_TYPEMASK
	case "ListBox" pr="LBS"
	case "ComboBox" pr="CBS"; tmask=3
	case "QM_ComboBox" pr="CBS"; tmask=2; stmask=2|CBS_AUTOHSCROLL|CBS_SORT
	case "QM_Grid" pr="LVS"; stmask=LVS_NOCOLUMNHEADER|LVS_NOSORTHEADER|LVS_SHOWSELALWAYS|LVS_SINGLESEL; flags|2
	case "SysListView32" pr="LVS"; tmask=LVS_TYPEMASK
	case "ComboBoxEx32" pr="CBS"; tmask=3; stmask=3|CBS_AUTOHSCROLL
	case "SysAnimate32" pr="ACS"
	case "SysDateTimePick32" pr="DTS"; tmask=0xC
	case "SysHeader32" pr="HDS"
	case "SysMonthCal32" pr="MCS"
	case "SysPager" pr="PGS"; tmask=1
	case "msctls_progress32" pr="PBS"
	case "ReBarWindow32" pr="RBS"; ccs=ccsCommon; tmask=ccsType
	case "ScrollBar" pr="SBS"; tmask=1
	case "msctls_statusbar32" pr="SBARS"; ccs=ccsCommon; tmask=ccsType
	case "SysTabControl32" pr="TCS"
	case "ToolbarWindow32" pr="TBSTYLE"; ccs=ccsCommon|CCS_ADJUSTABLE; tmask=ccsType
	case "msctls_trackbar32" pr="TBS"; tmask=2
	case "SysTreeView32" pr="TVS"
	case "msctls_updown32" pr="UDS"
if(!tmask) flags|1

s.getmacro("WinStyles")
s.addline("def CCS_DEFAULT 0x0")

findrx(s "^def (WS_\w+)[ \t]+(0x\w+)( ;;[^[]]+)?" 0 4|8 ac) ;;info: in WinStyles, non-style constants such as masks and 0 are 0X... (uppercase x), and this regex skips them
for i 0 ac.len
	comm=0; if(ac[3 i].len) comm=F"  ({ac[3 i]+3})"
	str& r=ac[1 i]
	if(_hsel=_hform) sel(r) case ["WS_GROUP","WS_TABSTOP"] continue
	else sel(r) case ["WS_CHILD","WS_MAXIMIZEBOX","WS_MINIMIZEBOX","WS_POPUP"] continue
	amp=""; j=val(ac[2 i] 1); if(!j) continue
	if r.beg("WS_EX_")
		if(j&exst1=j) amp="&"
		j|0x100000000
	else
		if(j&st1=j) amp="&"
		_i=j; sel(_i) case [WS_BORDER,WS_DLGFRAME] if(st1&WS_CAPTION=WS_CAPTION) amp="" ;;select only 1 of WS_CAPTION,WS_BORDER,WS_DLGFRAME
	lb4WS.formata("%s%s%s[]" amp r comm)
	ic[]=j

 g1
if pr
	findrx(s _s.format("^def (%s_\w+)[ \t]+(0x\w+)( ;;[^[]]+)?" pr) 0 4|8 as)
	for i 0 as.len
		comm=0; if(as[3 i].len) comm=F"  ({as[3 i]+3})"
		amp=""; j=val(as[2 i])
		if tmask and (j&tmask or !j)
			if(j=st1&tmask) amp="&"
			else sel(cls 1) case "SysDateTimePick32" if(j=st1&(tmask|1)) amp="&" ;;DTS_TIMEFORMAT==9, DTS_UPDOWN==1
			cb7Typ.formata("%s%s%s[]" amp as[1 i] comm)
			it[]=j
		else
			if(j&stmask=0) continue
			
			sel cls 1
				case "#32770" sel(as[1 i]) case "DS_FIXEDSYS" as[1 i]="DS_SHELLFONT"
				case ["Edit","QM_Edit"] sel(as[1 i]) case "ES_DISABLENOSCROLL" continue ;;ES_DISABLENOSCROLL==ES_NUMBER
				case ["RichEdit20A","RichEdit20W","RichEdit50W"] sel(as[1 i]) case "ES_NUMBER" continue
			
			if(j&st1=j) amp="&"
			lb6CS.formata("%s%s%s[]" amp as[1 i] comm)
			is[]=j
		stUnknown~j

if(ccs) stmask=ccs; ccs=0; pr="CCS"; goto g1

if(stUnknown) e10Add=F"0x{stUnknown}"

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 384 256 "Styles"
 3 Static 0x54020000 0x4 8 4 74 10 "Common styles"
 4 ListBox 0x54330309 0x204 8 16 216 158 "WS"
 5 Static 0x54020000 0x4 232 4 136 10 "Styles specific to this window class"
 6 ListBox 0x54230109 0x204 232 16 144 124 "CS"
 9 Static 0x54000200 0x0 232 144 22 13 "Type"
 7 ComboBox 0x54230243 0x0 256 144 120 213 "Typ"
 12 Static 0x54000200 0x0 232 160 22 13 "Other"
 10 Edit 0x54030080 0x200 256 160 120 13 "Add" "A raw style value to add to the styles selected above.[]Must be numeric, like 0xC001. Don't use style constants.[]Must be less than 0x10000."
 11 QM_DlgInfo 0x54000000 0x20000 8 184 368 44 ""
 1 Button 0x54030001 0x4 8 236 48 14 "OK"
 2 Button 0x54030000 0x4 60 236 48 14 "Cancel"
 8 Button 0x54032000 0x0 112 236 18 14 "?"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""
_styleFlags=flags
if(!ShowDialog(dd &sub.DlgProcStyle &controls _hwnd 0 0 0 &this)) ret

int st2 exst2
st2=val(e10Add)&0xffff
if pr
	if(st2&tmask=0) i=val(cb7Typ); if(i>=0) st2|it[i]
	for(i 0 lb6CS.len) if(lb6CS[i]='1') st2|is[i]
for(i 0 lb4WS.len) if(lb4WS[i]='1') j=ic[i]; if(j&0x100000000) exst2|j; else st2|j

sub.SetStyle(st2 exst2)


#sub DlgProcStyle
function# hDlg message wParam lParam

__DE* d=+DT_GetParam(hDlg)
ret d.sub._DlgProcStyle(hDlg message wParam lParam)


#sub _DlgProcStyle c
function# hDlg message wParam lParam

int i h; str s; RECT r
type ___DE_STYLEINFO ARRAY(str)a hlb1 hlb2 hcb hPrev iPrev
___DE_STYLEINFO- x

sel message
	case WM_INITDIALOG
	h=id(4 hDlg); GetClientRect h &r; SendMessage h LB_SETCOLUMNWIDTH r.right/2 0
	i=DT_GetParam(hDlg)
	if(i&1) TO_Show hDlg "7 9" 0
	if(i&2) TO_Show hDlg "10 12" 0
	
	s.getmacro("DE_StyleHelp")
	findrx(s "(?s)^([A-Z]+_\w+)[](.+?)[][]" 0 4|8 x.a)
	x.hlb1=id(4 hDlg); x.hlb2=id(6 hDlg); x.hcb=id(7 hDlg)
	SendMessage id(11 hDlg) SCI.SCI_SETTABWIDTH 60 0
	SetTimer hDlg 1 100 &sub.InfoTimer
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case LBN_SELCHANGE<<16|4
	_i=LB_SelectedItem(lParam)
	if(_hsel=_hform) sel(_i) case [0,1] TO_LbUnselect lParam _i "0 1"; case [6,7,8] TO_LbUnselect lParam _i "6 7 8"
	else sel(_i) case [4,5,6] TO_LbUnselect lParam _i "4 5 6"
	
	case CBN_SELENDOK<<16|7
	sub.ShowInfo(hDlg lParam CB_SelectedItem(lParam))
	
	case 8
	i=ShowMenu("WS_ styles[]WS_EX_ styles[]Control styles[]Dialog box styles" hDlg 0 2)
	sel i
		case 1 s="window styles"
		case 2 s="CreateWindowEx"
		case 3
		s="control library list view tree"
		if(mes("This will search for controls reference.[]In the list of results:[]click 'Control Library (Windows)',[]click a control link,[]scroll to Styles." "" "OCi")!='O') ret
		case 4 s="dialog box styles"
	if(i) run F"http://www.google.com/search?q=site:microsoft.com {s.escape(9)}"; err
ret 1


#sub InfoTimer
function hDlg a2 a3 a4

___DE_STYLEINFO- x
int i h=child(mouse 1)
POINT p

if(h=x.hlb1 or h=x.hlb2) xm p h 1; i=SendMessage(h LB_ITEMFROMPOINT 0 MakeInt(p.x p.y))
else if(h=x.hcb) i=CB_SelectedItem(h)
else ret
sub.ShowInfo(hDlg h i)


#sub ShowInfo
function hDlg h i

___DE_STYLEINFO- x
str s

if(h=x.hPrev and i=x.iPrev) ret
x.hPrev=h; x.iPrev=i
if(i&0x10000) ret

if(!iif(h=x.hcb CB_GetItemText(h i s) LB_GetItemText(h i s))) ret
s.gett(s 0) ;;remove comments

int found
for(i 0 x.a.len) if(s=x.a[1 i]) found=1; break
if(!found) ret

s.format("<><b>%s</b>[]%s" x.a[1 i] x.a[2 i])
s.setwintext(id(11 hDlg))


#sub SetStyle c
function newStyle newExStyle

_Undo

___DE_CONTROL& c=subs.GetControl(_hsel)
str txt cls; int h hprev pstyle(c.style) pexstyle(c.exstyle)
RECT r r2
c.style=newStyle
c.exstyle=newExStyle

if(_hsel=0) _hsel=_hform
h=_hsel
if(h=_hform)
	GetClientRect h &r
	if(c.style&WS_POPUP) c.style~WS_CHILD
	SetWinStyle h c.style|WS_CHILD|WS_VISIBLE|WS_DISABLED|WS_CLIPSIBLINGS~WS_POPUP 8
	SetWinStyle h c.exstyle 12
	 don't resize client area
	GetClientRect h &r2
	if memcmp(&r &r2 sizeof(r))
		TO_AdjustWindowRect &r 0 0 h
		SetWindowPos h 0 0 0 r.right-r.left r.bottom-r.top SWP_NOMOVE|SWP_NOZORDER
		subs.AutoSizeEditor ;;ensure all form is in editor when made bigger
else
	c.style|WS_CHILD; c.style~WS_POPUP
	hprev=GetWindow(h GW_HWNDPREV)
	cls.getwinclass(h)
	if(__CDD_CanHaveText(cls newStyle)) txt=c.txt
	GetWindowRect(h &r); MapWindowPoints(0 _hform +&r 2)
	
	h=_CreateControl(c.exstyle cls txt c.style r.right-r.left r.bottom-r.top c.cid 1 r.left r.top)
	if(h)
		DestroyWindow(_hsel)
		_hsel=h
		SetWindowPos(h hprev 0 0 0 0 SWP_NOMOVE|SWP_NOSIZE)
	else
		c.style=pstyle; c.exstyle=pexstyle
		mes "Failed to set styles." "Dialog Editor" "!"

_Select(_hsel)
