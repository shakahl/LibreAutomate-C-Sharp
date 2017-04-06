 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 5000 1101 1201 1301 1303 1304 1306 1307 1310 1308 1401 1501 1601 1602 1702 1801 1901 2001 2101 2201 2301"
__strt lb3Act qmt5000 c1101Clo lb1201 lb1301 e1303x e1304y e1306cx e1307cy c1310Pop c1308Wor lb1401 lb1501 lb1601 c1602Not e1702hwn lb1801 lb1901 lb2001 lb2101 lb2201 lb2301

TO_FavSel wParam lb3Act "Activate or set focus[]Close[]Maximize, minimize, restore[]Move, resize[]Hide, show, disable, enable[]Z-order[]If...[]If name, class...[]Get name, class, program[]Get position, size, monitor[]Get style, id[]Get related window[]Get windows (array)[]Arrange windows"
lb1201="&Maximize[]Minimize[]Restore[]Restore minimized, activate"
lb1301="&Move[]Resize[]Move and resize"
c1310Pop=1
lb1401="&Hide[]Unhide[]Unhide and activate[]Disable[]Enable"
lb1501="&Make always-on-top[]Make not always-on-top[]Z-order top[]Z-order bottom"

lb1601="&Exists[]Is the active window[]Is the focused control[]Visible[]Enabled[]Maximized[]Minimized[]Normal (not min/max)[]Always-on-top[]Hung[]64-bit[]Cloaked (Win8/10)[]Control[]Is valid handle"

lb1801="Name/text[]Class[]Program file name[]Program file full path[]Output handle, class, name"
lb1901="X[]Y[]Width[]Height[]RECT[]Monitor"
lb2001="Style[]Extended style[]Control id"
lb2101="&Owner of owned window[]TL parent of control[]Direct parent of control[]First Alt+Tab window[]Next Alt+Tab window[]First TL window[]First child[]Next sibling[]Previous sibling[]First sibling[]Last sibling"
lb2201="&All top-level windows[]Visible top-level windows[]Main windows, like Alt+Tab[]All child windows[]Direct child windows"
lb2301="&Show/hide desktop[]Minimize all[]Restore all[]Cascade[]Tile horizontally[]Tile vertically"

if(!ShowDialog("" &TO_Window &controls _hwndqm)) ret

str s sout winVar winFind
__strt vd v
ARRAY(str) z
int f j i

i=val(lb3Act)
if(i<=12) qmt5000.Win(winVar 0 winFind)

sel i
	case 0 ;;activate
	s="act"
	
	case 1 ;;close
	if(c1101Clo=1 and winFind.len and 0=winFind.replacerx("^\w+ \w+=win\(" "CloseWindowsOf(" 4)) s.getl(winFind 0); goto gIns
	s="clo"
	
	case 2 ;;min max res
	lb1201.SelS(s "max min res act")
	
	case 3 ;;mov siz
	int f1 f2 f3 f4
	str sxy=F" {e1303x.N(`0` f1)} {e1304y.N(`0` f2)}"
	str swh=F" {e1306cx.N(`0` f3)} {e1307cy.N(`0` f4)}"
	sel val(lb1301)
		case 0 s.from("mov" sxy); f=f1|(f2<<1)
		case 1 s.from("siz" swh); f=f3|(f4<<1)
		case 2 s.from("mov+" sxy swh); f=f1|(f2<<1)|(f3<<8)|(f4<<9)
	if(c1308Wor=1) f|4
	
	case 4 ;;hide enable
	lb1401.SelS(s "hid hid- act <EnableWindow [1] 0> <EnableWindow [1] 1>")
	
	case 5 ;;zorder
	lb1501.SelS(s "TOPMOST NOTOPMOST TOP BOTTOM")
	s-"Zorder [1] HWND_"
	
	case 6 ;;if
	j=lb1601.SelS(s "[1] [1]=win [1]=child IsWindowVisible([1]) IsWindowEnabled([1]) IsZoomed([1]) IsIconic([1]) <!(IsZoomed([1]) or IsIconic([1]))> <GetWinStyle([1] 1)&WS_EX_TOPMOST> IsHungAppWindow([1]) IsWindow64Bit([1]) IsWindowCloaked([1]) IsChildWindow([1]) IsWindow([1])")
	
	if c1602Not=1
		sel j
			case [1,2] s.insert("!" findcr(s '='))
			case 7 s.remove(0 1)
			case 8 s-"!("; s+")"
			case else s-"!"
	
	s=F"if {s}[][9]"
	
	case 7 ;;if name class
	e1702hwn.N("windowHandle")
	sel winVar 2
		case "child(?*)"
			if(winVar.replacerx("'' \w+\)$" "'')")) out "Note: cannot use 'match index' with childtest."
			sub_to.Trim(winVar " 0") ;;on IDOK Window field text replaced with "(0)" because don't need with childtest
			s=F"if childtest({e1702hwn} {winVar+6}[][9]"
		case "id(?*)"
			if(!tok(winVar+3 &s 1 " ()" 8)) ret
			s=F"if childtest({e1702hwn} {s})[][9]"
		case else
			j=findc(winFind '(')+1; if(!j) ret
			s=F"if wintest({e1702hwn} {winFind+j}[9]"
	goto gIns
	
	case 8 ;;get name class program
	if(findc(lb1801 '1')<0) lb1801[0]='1'
	if(lb1801[0]='1') s=F"{vd.VD(`str txt`)}.getwintext([1])"
	if(lb1801[1]='1') s.addline(F"{vd.VD(`str cls`)}.getwinclass([1])" 1)
	if(lb1801[2]='1') s.addline(F"{vd.VD(`str exe`)}.getwinexe([1])" 1)
	if(lb1801[3]='1') s.addline(F"{vd.VD(`str exe`)}.getwinexe([1] 1)" 1)
	if(lb1801[4]='1') s.addline("outw [1]" 1)
	
	case 9 ;;get pos size monitor
	lb1901.LbSelectedItemsToNames(z "x y cx cy" "111100")
	if !lb1901.beg("0000")
		vd.VD("-D int 0 0 0 0[]" z[0] z[1] z[2] z[3])
		s=F"{vd}GetWinXY [1] {z[0]} {z[1]} {z[2]} {z[3]}"; sub_to.Trim s " 0 0 0"
	if(lb1901[4]='1') vd.VD("-i RECT r" v); s.addline(F"{vd}; DpiGetWindowRect [1] &{v}" 1)
	if(lb1901[5]='1') s.addline(F"{vd.VD(`int monitor`)}=MonitorIndex(MonitorFromWindow([1] 0))" 1)
	
	case 10 ;;get style id
	if(findc(lb2001 '1')<0) lb2001[0]='1'
	if(lb2001[0]='1') s=F"{vd.VD(`int style`)}=GetWinStyle([1])"
	if(lb2001[1]='1') s.addline(F"{vd.VD(`int exstyle`)}=GetWinStyle([1] 1)" 1)
	if(lb2001[2]='1') s.addline(F"{vd.VD(`int cid`)}=GetWinId([1])" 1)
	
	case 11 ;;get related window
	lb2101.SelS(s "<GetWindow([1] GW_OWNER)> <GetAncestor([1] 2)> <GetParent([1])> <RealGetNextWindow(0)> <RealGetNextWindow([1])> <GetTopWindow(0)> <GetWindow([1] GW_CHILD)> <GetWindow([1] GW_HWNDNEXT)> <GetWindow([1] GW_HWNDPREV)> <GetWindow([1] GW_HWNDFIRST)> <GetWindow([1] GW_HWNDLAST)>")
	s-F"{vd.VD(`int w`)}="
	
	case 12 ;;get array
	vd.VD("-i ARRAY(int) a[]" v)
	lb2201.SelS(s F"<opt hidden 1[]win('''' '''' '''' 0 0 0 {v})[]opt hidden 0> <win('''' '''' '''' 0x400 0 0 {v})> <GetMainWindows({v})> <child('''' '''' [1] 0 0 0 {v})> <child('''' '''' [1] 16 0 0 {v})>")
	s-vd
	sout=F" sample code, shows how to use the array[]out[]int i hwnd[]for i 0 {v}.len[][9]hwnd={v}[i][][9]outw hwnd"
	
	case 13 ;;arrange
	s=F"ArrangeWindows({val(lb2301)})"

if(i>12) goto gIns
j=findc(s 1)
if(!winVar.len) if(j>=0 or f) winVar="win()"; else goto gIns
if(j>=0) s.findreplace("[1]" winVar); else s+F" {winVar}"
if(f) s+F" 0x{f}"
qmt5000.WinEnd(s winFind)

 gIns
 sub_to.TestDialog s i
InsertStatement s
if(sout.len) out "<><code>%s</code>" sout

 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 368 165 "Window"
 3 ListBox 0x54230101 0x204 4 4 104 128 "Act"
 5000 QM_Tools 0x54030000 0x10000 124 4 240 54 "1 0x1020"
 1001 QM_DlgInfo 0x54000000 0x20000 124 98 240 34 "If window, activates. Also restores minimized and unhides.[]If control, activates its window and sets focus.[]If next window, activates another window like Alt+Tab."
 1101 Button 0x54012003 0x0 124 70 240 12 "Close all windows that have this name, class and program"
 1201 ListBox 0x54230101 0x200 124 68 100 64 ""
 1202 QM_DlgInfo 0x54000000 0x20000 236 68 128 64 "<>See also: <help>SetWindowState</help>."
 1301 ListBox 0x54230101 0x200 124 68 100 30 ""
 1302 Static 0x54000000 0x0 236 70 52 12 "X, Y"
 1303 Edit 0x54030080 0x204 290 68 36 14 "x" "If empty, will not change X position.[]A double number means % of screen or parent, eg 0.5 is 50%."
 1304 Edit 0x54030080 0x204 328 68 36 14 "y" "If empty, will not change Y position.[]A double number means % of screen or parent, eg 0.5 is 50%."
 1305 Static 0x44000000 0x0 236 86 52 12 "Width, Height"
 1306 Edit 0x44030080 0x204 290 84 36 14 "cx" "If empty, will not change width.[]A double number means % of screen or parent, eg 0.5 is 50%."
 1307 Edit 0x44030080 0x204 328 84 36 14 "cy" "If empty, will not change height.[]A double number means % of screen or parent, eg 0.5 is 50%."
 1310 Button 0x54012003 0x0 290 100 77 21 "Populate now with the Drag tool"
 1308 Button 0x54012003 0x0 290 122 48 10 "Work area" "Window coordinates are in the work area"
 1311 QM_DlgInfo 0x54000000 0x20000 124 102 152 30 "<>See also: <help>RegWinPos</help>, <help>CenterWindow</help>, <help>EnsureWindowInScreen</help>, <help>MoveWindowToMonitor</help>, <help>_monitor</help>."
 1401 ListBox 0x54230101 0x200 124 68 100 64 ""
 1501 ListBox 0x54230101 0x200 124 68 100 64 ""
 1601 ListBox 0x54230101 0x200 124 68 100 64 ""
 1602 Button 0x54012003 0x4 230 68 48 13 "Not"
 1701 Static 0x54000000 0x0 124 70 32 12 "Handle"
 1702 Edit 0x54030080 0x200 158 68 39 14 "hwn"
 1703 QM_DlgInfo 0x54000000 0x20000 124 86 240 46 "<>Use this when you have a window or control handle and want to know if the window/control has the specified name, class, program, etc.[]This will create code for <help>wintest</help> or <help>childtest</help> (if Control selected).[][]See also: <help>WinTest</help>."
 1801 ListBox 0x54230109 0x200 124 68 100 64 "" "Select one or several"
 1810 QM_DlgInfo 0x54000000 0x20000 236 68 128 64 "Gets the window name/text property. It is title bar text, button text, edit control text etc. To capture all text in the window, use dialog 'Window text'.[][]To change window or other UI object text, use one of dialogs: Text, Accessible object actions, HTML element actions."
 1901 ListBox 0x54230109 0x200 124 68 100 64 "" "Select one or several"
 1902 QM_DlgInfo 0x54000000 0x20000 236 68 128 64 "<>See also: <help>RegWinPos</help>, <help>_monitor</help>, <help>MoveWindowToMonitor</help>."
 2001 ListBox 0x54230109 0x200 124 68 100 64 "" "Select one or several"
 2002 QM_DlgInfo 0x54000000 0x20000 236 68 128 64 "<>Only controls can have id.[][]See also: <help>SetWinStyle</help>."
 2101 ListBox 0x54230101 0x200 124 68 100 64 ""
 2102 QM_DlgInfo 0x54000000 0x20000 236 68 128 64 "<>Gets handle of other window that is related to this window.[][]TL - top-level (not control).[][]See also: <help>TriggerWindow</help>, <help>GetToolbarOwner</help>, <help>_hwndqm</help>, <help>FirstWindowInMonitor</help>.[][]In dialog procedure, dialog handle is hDlg; don't use win(''Dialog'')."
 2201 ListBox 0x54230101 0x200 124 68 100 64 ""
 2202 QM_DlgInfo 0x54000000 0x20000 236 68 128 64 "Gets handles of top-level windows, or of child windows of the window or control.[][]If need only windows that have certain name, class, etc, will need to edit the macro - add name, class etc in code that this dialog will create."
 2301 ListBox 0x54230101 0x200 124 68 100 64 ""
 2302 QM_DlgInfo 0x54000000 0x20000 236 68 128 64 "<>See also: <help>SaveMultiWinPos</help>, <help>RestoreMultiWinPos</help>."
 2401 QM_DlgInfo 0x54000000 0x20000 124 4 240 128 ""
 1 Button 0x54030001 0x4 4 146 48 14 "OK"
 2 Button 0x54010000 0x4 54 146 51 14 "Cancel"
 4 Button 0x54032000 0x4 106 146 16 14 "?"
 5 Button 0x54032000 0x4 124 146 34 14 "? More"
 6 Static 0x54000010 0x20004 0 138 374 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x203050D "*" "" "(0 40) (1 40) (2 40) (3 40) (4 40) (5 40) (6 40) (7 40) (8 40) (9 40) (10 40) (11 40) (12 40)" ""

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "3[]$qm$\window.ico" "" 1)) ret wParam
sel message
	case WM_INITDIALOG
	int-- t_hWinCtrl; t_hWinCtrl=id(5000 hDlg)
	SetDlgItemText t_hWinCtrl 510 "Next window"
	goto gAction
	
	case WM_COMMAND goto messages2
	case __TWN_DRAGEND goto gFinderDrop
ret
 messages2
sel wParam
	case LBN_SELCHANGE<<16|3
	 gAction
	i=TO_Selected(hDlg 3)
	DT_Page hDlg i "(0 40) (1 40) (2 40) (3 40) (4 40) (5 40) (6 40) (7 40) (8 40) (9 40) (10 40) (11 40) (12 40)" 1
	TO_Show t_hWinCtrl "510" i=0
	SendMessage t_hWinCtrl __TWM_SETLOCK 0 0 ;;unlock if locked
	
	sel(i) case 5 goto g5; case 6 goto g6; case 11 goto g11; case 12 goto g12
	
	case LBN_SELCHANGE<<16|1301
	TO_ShowSelected hDlg "1302-1307" TO_Selected(hDlg 1301) "1302-1304" "1305-1307" "1302-1307"
	
	case LBN_SELCHANGE<<16|1501
	 g5
	sel(TO_Selected(hDlg 1501)) case [0,1] j=1; case else j=0
	SendMessage t_hWinCtrl __TWM_SETLOCK j 0 ;;select and lock Window etc
	
	case LBN_SELCHANGE<<16|1601
	 g6
	sel(TO_Selected(hDlg 1601)) case 1 j=1; case else j=0
	SendMessage t_hWinCtrl __TWM_SETLOCK j 0 ;;select and lock Window etc
	
	case LBN_SELCHANGE<<16|2101
	 g11
	sel(TO_Selected(hDlg 2101)) case [0,4] j=1; case [1,2] j=2; case [3,5] j=3; case else j=0
	SendMessage t_hWinCtrl __TWM_SETLOCK j 0 ;;select and lock Window etc
	
	case LBN_SELCHANGE<<16|2201
	 g12
	sel(TO_Selected(hDlg 2201)) case [0,1,2] j=3; case else j=0
	SendMessage t_hWinCtrl __TWM_SETLOCK j 0 ;;select and lock Window etc
	
	case 4 ;;?
	i=TO_Selected(hDlg 3)
	QmHelp "IDP_ACT[]IDP_CLO[]IDP_MIN[]IDP_MOV[]IDP_HID[]Zorder[]IDP_IF[]IDP_WIN[]IDP_S_WINDOW[]GetWinXY[]GetWinStyle[][]IDP_WIN[]ArrangeWindows" i
	
	case 5 ;;? More
	s=
 Not all window functions are included in this dialog. You can find more in the window <help #IDP_CATEGORIES>category</help>. Example:
;
 <code>
 int w=win("Options" "#32770")
 window.Transparent w 128
 </code>
;
 See also: <help #IDP_E_WINDOW>Windows API</help>
	QmHelp s 0 6
	
	case IDOK
	sel TO_Selected(hDlg 3)
		case 7 _i=id(5000 hDlg); if(SendMessage(_i __TWM_GETSELECTED 0 0)=2) _s="(0)"; _s.setwintext(id(500 _i)) ;;set childtest hwnd argument to 0
ret 1

 gFinderDrop
if TO_Selected(hDlg 3)=3 and but(1310 hDlg)
	int w(wParam) isChild=IsChildWindow(w)
	RECT _r _w; int* pr(&_r) pw(&_w)
	SetRect &_w id(1303 hDlg) id(1304 hDlg) id(1306 hDlg) id(1307 hDlg)
	GetWindowRect w &_r; if(isChild) MapWindowPoints 0 GetParent(w) +&_r 2
	if(_winver<0x603 && DpiIsWindowScaled(w)) DpiScale +&_r 2
	if(but(1308 hDlg) and !isChild) GetWorkArea i j 0 0 0 w; OffsetRect &_r -i -j ;;work area
	for(i 0 4) s=pr[i]-iif(i<2 0 pr[i-2]); s.setwintext(pw[i])
ret 1
