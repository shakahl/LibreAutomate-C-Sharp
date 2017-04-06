\Dialog_Editor

 Explore windows. Shows all processes, threads, windows and controls.

type ___EW_ITEM htvi v !t !filled ;;t: 0 nothing, 1 pid, 2 tid, 3 hwnd
type ___EW_ENUM htv htviPar

ARRAY(___EW_ITEM) ai

str controls = "6"
str si6Dra
si6Dra="&$qm$\target.ico"

if(!ShowDialog("" &sub.DlgProc &controls _hwndqm 0 0 0 0 -1 40)) ret

 BEGIN DIALOG
 1 "" 0x90CF0A48 0x108 0 0 350 250 "Explore windows"
 4 Button 0x54032001 0x4 2 2 48 14 "Refresh"
 3 SysTreeView32 0x54010027 0x204 0 20 260 142 ""
 5 QM_DlgInfo 0x54000002 0x20000 0 164 260 64 ""
 6 Static 0x54020003 0x4 62 0 16 16 "Dra" ".1 Drag and drop to capture.[]Right click for more options."
 END DIALOG
 DIALOG EDITOR: "" 0x2040104 "*" "" "" ""

#sub DlgProc
function# hDlg message wParam lParam
if(sub_to.ToolDlgCommon(&hDlg "0[]$qm$\window_find.ico")) ret wParam
sel message
	case WM_INITDIALOG
	GetWorkArea 0 0 0 _i 0 hDlg
	_i*0.9
	siz 0 _i hDlg 1
	goto g1
	
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
	
	case WM_SIZE
	int htv=id(3 hDlg)
	RECT rc rt; GetClientRect hDlg &rc; GetWindowRect htv &rt; ScreenToClient hDlg +&rt
	int y1(rc.bottom-rt.top) y2(y1*0.8)
	MoveWindow htv 0 rt.top rc.right y2-4 1
	MoveWindow id(5 hDlg) 0 rt.top+y2 rc.right y1-y2 1
	
	case WM_LBUTTONDOWN
	if(GetDlgCtrlID(child(mouse))=6) sub.Drag hDlg
	
	case WM_RBUTTONUP
	if(GetDlgCtrlID(child(mouse))=6)
		sub_to.DragTool_Menu(hDlg "{+}" 2)
ret
 messages2
sel wParam
	case 4
	 g1
	sub.Refresh hDlg
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 3
	sel nh.code
		case TVN_ITEMEXPANDINGW
		sub.OnExpanding +nh
		case TVN_GETDISPINFOW
		sub.OnGetdispinfo +nh
		case TVN_SELCHANGEDW
		sub.OnSelect hDlg +nh
		case NM_CUSTOMDRAW
		ret DT_Ret(hDlg sub.OnCustomDraw(+nh))


#sub Refresh v
function hDlg

int htv=id(3 hDlg)
hid htv
SendMessage(htv TVM_DELETEITEM 0 0)
ai.redim
_s.setwintext(id(5 hDlg))

 Add processes and threads.

type ___EW_TIDPID tid pid hasWin
ARRAY(___EW_TIDPID) atp

int hSnap k nt i hasWin hip hiwl hihi
str s1
PROCESSENTRY32W pe.dwSize=sizeof(PROCESSENTRY32W)
THREADENTRY32 te.dwSize=sizeof(THREADENTRY32)

hSnap=CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD 0)
k=Thread32First(hSnap &te)
for nt 0 1000000
	if(!k) break
	if(nt=atp.len) atp.redim(nt+100)
	___EW_TIDPID& r=atp[nt]
	r.tid=te.th32ThreadID
	r.pid=te.th32OwnerProcessID
	EnumThreadWindows(r.tid &sub.EnumThreadWin &r.hasWin)
	k=Thread32Next(hSnap &te)
CloseHandle(hSnap)

hiwl=sub.TvAdd(htv 0 "<processes without windows>" 0 0 0 1)
hihi=sub.TvAdd(htv 0 "<processes without visible windows>" 0 0 0 1)

hSnap=CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS 0)
k=Process32FirstW(hSnap &pe)
rep
	if(!k) break
	if pe.th32ProcessID
		s1.getfilename(s1.ansi(&pe.szExeFile) 1)
		hasWin=0; for(i 0 nt) if(atp[i].pid=pe.th32ProcessID) hasWin|atp[i].hasWin
		hip=sub.TvAdd(htv iif(hasWin&2 0 iif(hasWin hihi hiwl)) s1 0 pe.th32ProcessID 1 1)
		for(i 0 nt)
			if(atp[i].pid=pe.th32ProcessID)
				&r=atp[i]
				s1.format("Thread 0x%08X" r.tid)
				sub.TvAdd(htv hip s1 r.hasWin r.tid 2)
	 
	k=Process32NextW(hSnap &pe)
CloseHandle(hSnap)

hid- htv


#sub EnumThreadWin
function# hWnd int&yes
yes|1
if(IsWindowVisible(hWnd)) yes|2; ret
ret 1


#sub FillWindows
function htv ___EW_ITEM&r ;;r must be of thread

 Adds windows of thread if not already added.

if(r.filled) ret
r.filled=1

___EW_ENUM ew
ew.htv=htv
ew.htviPar=r.htvi
EnumThreadWindows r.v &sub.EnumWin &ew


#sub EnumWin
function# hWnd ___EW_ENUM&ew

 Adds thread windows, including children.

sub.EnumChild hWnd &ew sub.TvAdd(ew.htv ew.htviPar +LPSTR_TEXTCALLBACK 0 hWnd 3 1)
ret 1


#sub EnumChild
function# hWnd ___EW_ENUM&ew htviPar

 Adds children.

hWnd=GetWindow(hWnd GW_CHILD)
rep
	if(!hWnd) break
	sub.EnumChild hWnd &ew sub.TvAdd(ew.htv htviPar +LPSTR_TEXTCALLBACK 0 hWnd 3 1)
	hWnd=GetWindow(hWnd GW_HWNDNEXT)


#sub TvAdd v
function# htv hparent $s [hasChildren] [v] [t] [filled]

 Adds tv item and ai item.

TVINSERTSTRUCTW in
in.hParent = hparent
in.hInsertAfter = TVI_LAST
TVITEMW& ti=in.item
ti.mask = TVIF_TEXT|TVIF_PARAM
if(hasChildren) ti.mask|TVIF_CHILDREN; ti.cChildren=1
ti.lParam=ai.len

if s!LPSTR_TEXTCALLBACK
	if(!s) s=""
	ti.pszText=@s

___EW_ITEM& r=ai[]
r.htvi=SendMessage(htv TVM_INSERTITEMW 0 &in)
r.v=v
r.t=t
r.filled=filled
ret r.htvi


#sub OnExpanding v
function NMTREEVIEWW&nt

 Adds windows of thread when expanding thread.

if(nt.action!TVE_EXPAND) ret
TVITEMW& ti=nt.itemNew
___EW_ITEM& r=ai[ti.lParam]
 out "%i %i" r.t r.v

sel r.t
	case 2 ;;thread
	sub.FillWindows(nt.hdr.hwndFrom r)


#sub OnGetdispinfo v
function NMTVDISPINFOW&nt

 Sets tv item text. Only for windows.

TVITEMW& ti=nt.item
if(ti.mask&TVIF_TEXT=0) ret
___EW_ITEM& r=ai[ti.lParam]
 out "%i %i" r.t r.v

str s sc st
sel r.t
	case 3
	sc.getwinclass(r.v); err
	st.getwintext(r.v); err
	s.format("%i   %s   ''%s''" r.v sc st)
	__DispinfoText-- dit
	ti.pszText=dit.Get(s)


#sub OnSelect v
function hDlg NMTREEVIEWW&nt

 Shows window info when the tv item selected.

TVITEMW& ti=nt.itemNew
___EW_ITEM& r=ai[ti.lParam]
 out "%i %i" r.t r.v

str s sc st sst
sel r.t
	case 3
	int h(r.v) ho style(GetWinStyle(h)) exstyle(GetWinStyle(h 1))
	sub.WinStyleString sst style exstyle
	RECT k; DpiGetWindowRect(h &k)
	sc.getwinclass(h); err
	st.getwintext(h); err
	st.LimitLen(300 1)
	
	s=
	F
	 <>Class:   <_>''{sc}''
	 Text:    ''{st}''</_>
	 <google "site:microsoft.com Window Styles">Styles</google>:   0x{style}  0x{exstyle}  ( {sst} )
	 Rect.:    L={k.left}  T={k.top}  R={k.right}  B={k.bottom}    W={k.right-k.left}  H={k.bottom-k.top}
	 Other:    {iif(IsWindow64Bit(h) "64" "32")}-bit,  {iif(IsWindowUnicode(h) "unicode" "non-unicode")},  <google "site:microsoft.com Window Class Styles">class styles</google>: 0x{GetClassLong(h GCL_STYLE)}
	if(IsWindowCloaked(h)) s+",  <help ''IsWindowCloaked''>cloaked</help>"
	if(DpiIsWindowScaled(h)) s+",  <help ''DpiIsWindowScaled''>DPI-scaled</help>"
	 shoulddo: detect 16-bit. Don't know how.
	
	if(style&WS_CHILD) s+F"[]Id:  {GetDlgCtrlID(h)}"
	else ho=GetWindow(h GW_OWNER); if(ho) s+F"[]Owner:  {ho}  <_>{sc.getwinclass(ho)}  ''{st.getwintext(ho)}''</_>"; err
	
	int tColor tAlpha tFlags
	if exstyle&WS_EX_LAYERED and GetLayeredWindowAttributes(h &tColor &tAlpha &tFlags) and tFlags and !(tFlags=2 and tAlpha>=255)
		s+"[]Transparent:   "
		if(tFlags&2) s+F"opaque {MulDiv(tAlpha 100 255)} %, "
		if(tFlags&1) s+F"transparent color 0x{tColor}"
		s.rtrim(", ")
	
	case 1
	GetProcessExename(r.v &st 1)
	s=
	F
	 Process Id:   {r.v}
	 File:   {st}
	;
	if _winnt>=6
		sc=
		F
		 Integrity Level:   {GetProcessIntegrityLevel(r.v 1)}    (1 Low, 2 Medium, 3 High, 4 System)
		 UAC:   {GetProcessUacInfo(r.v 1)}    (1 UAC off or user account, 2 admin, 3 uiAccess, 4 user)
		 64-bit:   {IsWindow64Bit(r.v 1)}
		;
		s.addline(sc)

s.setwintext(id(5 hDlg))


#sub OnCustomDraw v
function# NMTVCUSTOMDRAW&nt

 Sets gray text for invisible windows.

NMCUSTOMDRAW& cd=nt.nmcd
sel cd.dwDrawStage
	case CDDS_PREPAINT ret CDRF_NOTIFYITEMDRAW
	case CDDS_ITEMPREPAINT
	 if(cd.uItemState&CDIS_SELECTED) ret
	___EW_ITEM& r=ai[cd.lItemlParam]
	 out "%i %i" r.t r.v
	sel r.t
		case 3
		if(!IsWindowVisible(r.v)) nt.clrText=0x808080


#sub Drag
function# hDlg

 Window finder.

__MinimizeDialog m.Minimize(hDlg)
__OnScreenRect osr
__Drag d.Init(hDlg 1)
rep() sub.Rect osr 0; if(!sub_to.DragTool_Loop(d)) break
if(d.dropped) sub.SelectHwnd(hDlg sub.Rect(osr 2))


#sub Rect
function# __OnScreenRect&osr flags ;;flags: 1 begin, 2 end, 0 move.

 Draws rectangle of window/child from mouse. Returns its hwnd.

int h=child(mouse 1)
RECT r; DpiGetWindowRect(h &r)
osr.Show(flags &r)
ret h


#sub SelectHwnd v
function! hDlg h

 Selects tv item of window h.

int i htv(id(3 hDlg)) tid re

SetFocus htv

 find thread and fill its windows
tid=GetWindowThreadProcessId(GetAncestor(h 2) 0) ;;getancestor because h may belong to different thread or process, like tabs in IE
 g1
for(i 0 ai.len) if(tid=ai[i].v and ai[i].t=2) break
if(i=ai.len) goto notfound
sub.FillWindows(htv ai[i])

 find window and select
for(i 0 ai.len)
	if(h=ai[i].v and ai[i].t=3)
		SendMessage htv TVM_SELECTITEM TVGN_CARET ai[i].htvi
		SendMessage htv TVM_ENSUREVISIBLE 0 ai[i].htvi
		ret 1

 notfound
if(!re) sub.Refresh hDlg; re=1; goto g1


#sub WinStyleString
function str&styleStr style [exStyle]

int s(style) e(exStyle)
str& k=styleStr
k.fix(0)

if(s&WS_POPUP) k+"popup,"
if(s&WS_CHILD) k+"child,"
if(s&WS_MINIMIZE) k+"minimize,"
if(s&WS_VISIBLE) k+"visible,"
if(s&WS_DISABLED) k+"disabled,"
if(s&WS_CLIPSIBLINGS) k+"clipsiblings,"
if(s&WS_CLIPCHILDREN) k+"clipchildren,"
if(s&WS_MAXIMIZE) k+"maximize,"
if(s&WS_CAPTION=WS_CAPTION) k+"caption,"
else if(s&WS_BORDER) k+"border,"
else if(s&WS_DLGFRAME) k+"dlgframe,"
if(s&WS_VSCROLL) k+"vscroll,"
if(s&WS_HSCROLL) k+"hscroll,"
if(s&WS_SYSMENU) k+"sysmenu,"
if(s&WS_THICKFRAME) k+"thickframe,"
if s&WS_CHILD
	if(s&WS_GROUP) k+"group,"
	if(s&WS_TABSTOP) k+"tabstop,"
else
	if(s&WS_MINIMIZEBOX) k+"minimizebox,"
	if(s&WS_MAXIMIZEBOX) k+"maximizebox,"

if(e&WS_EX_DLGMODALFRAME) k+"ex_dlgmodalframe,"
if(e&WS_EX_NOPARENTNOTIFY) k+"ex_noparentnotify,"
if(e&WS_EX_TOPMOST) k+"ex_topmost,"
if(e&WS_EX_ACCEPTFILES) k+"ex_acceptfiles,"
if(e&WS_EX_TRANSPARENT) k+"ex_transparent,"
if(e&WS_EX_MDICHILD) k+"ex_mdichild,"
if(e&WS_EX_TOOLWINDOW) k+"ex_toolwindow,"
if(e&WS_EX_WINDOWEDGE) k+"ex_windowedge,"
if(e&WS_EX_CLIENTEDGE) k+"ex_clientedge,"
if(e&WS_EX_CONTEXTHELP) k+"ex_contexthelp,"
if(e&WS_EX_RIGHT) k+"ex_right,"
if(e&WS_EX_LEFT) k+"ex_left,"
if(e&WS_EX_RTLREADING) k+"ex_rtlreading,"
if(e&WS_EX_LTRREADING) k+"ex_ltrreading,"
if(e&WS_EX_LEFTSCROLLBAR) k+"ex_leftscrollbar,"
if(e&WS_EX_RIGHTSCROLLBAR) k+"ex_rightscrollbar,"
if(e&WS_EX_CONTROLPARENT) k+"ex_controlparent,"
if(e&WS_EX_STATICEDGE) k+"ex_staticedge,"
if(e&WS_EX_APPWINDOW) k+"ex_appwindow,"
if(e&WS_EX_LAYERED) k+"ex_layered,"
if(e&WS_EX_NOINHERITLAYOUT) k+"ex_noinheritlayout,"
if(e&WS_EX_LAYOUTRTL) k+"ex_layoutrtl,"
if(e&WS_EX_COMPOSITED) k+"ex_composited,"
if(e&WS_EX_NOACTIVATE) k+"ex_noactivate,"
if(e&WS_EX_NOREDIRECTIONBITMAP) k+"ex_noredirectionbitmap,"

styleStr.rtrim(","); styleStr.findreplace("," ", ")
