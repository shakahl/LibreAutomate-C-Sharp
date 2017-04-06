 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 8 6 10 1001 1102 1106 1103 1202 1303 1305 1402 1404 1405 1406 1410 1408 1502 1602 1701 1702 1704 1802 1905 1902 1903 1907"
__strt lb3Act c8Not e6Tim c10Res qmt1001 e1102Key cb1106Dow qmdi1103 e1202CPU e1303Var c1305Dec e1402Col e1404x e1405y c1406Cli c1410Act c1408Win e1502URL e1602Han lb1701cur si1702 e1704crc lb1802mou e1905Han e1902Nam cb1903Nam c1907Cas

TO_FavSel wParam lb3Act "Wait[]Wait for window active[]Wait for window created[]Wait for program exit[]Wait for window visible[]Wait for window enabled[]Wait for window name[]Wait for key[]Wait for mouse button[]Wait for CPU[]Wait for variable[]Wait for kernel object[]Wait for color[]Wait for web page[]Wait for cursor (mouse pointer)[]Set speed"
lb1802mou="&Any[]Left[]Right[]Middle"
sub.KeyCodes qmdi1103
c10Res=1; c1305Dec=1; c1406Cli=1
cb1903Nam="&Partial[]Full or with *?[]Regular expression"
cb1106Dow="&Wait for key up[]Wait for key down; eat"
lb1701cur="Arrow[]I Beam[]Wait[]Cross[]Size NWSE[]Size NESW[]Size WE[]Size NS[]Size All[]No[]Hand[]App Starting[]Help[]<other>"

if(!ShowDialog("" &TO_Wait &controls _hwndqm)) ret

str s winVar sWaitVarDecl
__strt vd

int A=val(lb3Act)

sel A
	case 0
		s=e6Tim.N("0.1"); if(!isdigit(s[0])) s-"wait "
		goto gIns
	case 15
		s=F"spe {e6Tim.NE}"; s.trim
		goto gIns
	case [1,2,3,4,5,12]
		qmt1001.Win(winVar "0")

s=F"wait {e6Tim.N} "
sel(A) case [1,2,4,5,6,9,10,12,14] int not=val(c8Not); if(not) s+"-"

sel A
	case [1,2,3,4,5] s+F"W{` ACPVE`+A%%.1s} {winVar}"
	case 6 s+F"WT {e1905Han.N} {e1902Nam.S} 0x{val(c1907Cas)*4|val(cb1903Nam)}"; sub_to.Trim s " 0x0"
	case 7 s+F"K{`F`+!val(cb1106Dow)} {e1102Key}"
	case 8 s+F"M{lb1802mou.SelC(` LRM`)}"
	case 9 s+F"P {e1202CPU.NE}"
	case 10 s+F"V {e1303Var.VN(`g_wait`)}"; if(c1305Dec=1) sWaitVarDecl=F"int+ {e1303Var}={not}[]"
	case 11 s+F"H {e1602Han.N}"
	case 12
	if(winVar!"0") TO_FlagsFromCheckboxes(c1406Cli c1410Act 2 c1408Win 0x1000)
	s+F"C {e1402Col.N} {e1404x.N} {e1405y.N} {winVar} {c1406Cli}"
	sub_to.Trim s " 0 0"
	case 13 s+F"I {e1502URL.SE}"
	case 14
	sel lb1701cur.CbItem
		case 13 s+F"CU {e1704crc.N(`???`)}"
		case else lb1701cur.findreplace(" "); s+F"CU IDC_{lb1701cur.ucase}"

s.rtrim
if(c10Res=1) sel(A) case [1,2,3,4,5,7,8] s[4]='('; _s.gett("w w w w w w vk mc" A-1); s=F"{vd.VD(`-D int R` _s)}={s})"
s-sWaitVarDecl
qmt1001.WinEnd(s)

 gIns
 sub_to.TestDialog s A
InsertStatement s
ret

 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 396 198 "Wait"
 3 ListBox 0x54230101 0x204 4 4 124 141 "Action"
 8 Button 0x54012003 0x4 4 150 60 13 "Not" "For example, 'Wait for window active' becomes 'Wait for window inactive'."
 5 Static 0x54020000 0x4 148 153 44 13 ""
 6 Edit 0x54030080 0x204 194 150 42 14 "Tim" "With 'Wait' - number of seconds to wait, eg 0.5.[]With 'Wait for...' - max time (seconds) to wait. Then error. Default - infinite.[]With 'Set speed' - number of milliseconds to wait after keyboard, mouse and some other commands."
 10 Button 0x54012003 0x0 274 150 116 14 "Res"
 1001 QM_Tools 0x54030000 0x10000 148 4 240 54 "1 32"
 1102 Edit 0x44030080 0x204 166 4 74 14 "Key" "Empty - any key."
 1104 Button 0x44032000 0x4 242 4 16 14 "VK"
 1105 Button 0x44032000 0x4 258 4 16 14 "VK"
 1106 ComboBox 0x54230243 0x0 298 4 90 213 "Dow"
 1101 Static 0x44020000 0x4 148 6 16 12 "Key"
 1103 QM_DlgInfo 0x54000000 0x20000 146 22 242 98 ""
 1202 Edit 0x44032000 0x204 202 4 42 14 "CPU"
 1201 Static 0x44020000 0x4 148 6 52 12 "CPU level, %"
 1203 QM_DlgInfo 0x54000000 0x20000 146 36 242 66 "Waits until CPU usage drops below this level. Can be 1 to 100.[]If 'Not' is checked, waits until will be above this level. Can be 0 to 99."
 1302 Static 0x44020000 0x4 148 6 30 12 "Variable"
 1303 Edit 0x44030080 0x204 182 4 48 14 "Variable"
 1304 QM_DlgInfo 0x54000000 0x20000 146 36 242 66 "Waits while this variable is 0.[]If ''Not'' is checked, waits while it is not 0."
 1305 Button 0x44012003 0x4 234 4 78 14 "Declare and reset"
 1407 Button 0x54032000 0x0 206 66 98 15 "Capture" "Select the pixel now from screen"
 1401 Static 0x44020000 0x4 150 89 22 13 "Color"
 1402 Edit 0x44030080 0x204 174 86 64 14 "Col" "Color value in 0xBBGGRR format"
 1403 Static 0x44020000 0x4 256 89 16 13 "X, Y"
 1404 Edit 0x44030080 0x204 274 86 28 14 "x"
 1405 Edit 0x44030080 0x204 304 86 28 14 "y"
 1406 Button 0x44012003 0x4 334 86 54 12 "Client area" "Coordinates are relative to the client area of the window or control, or work area of screen."
 1410 Button 0x54012003 0x0 148 112 184 10 "Activate window; error if X Y does not belong to it"
 1408 Button 0x54012003 0x0 148 124 184 10 "Window can be in background, if Aero enabled"
 1409 Button 0x5C032000 0x0 336 123 48 12 "View pixels" "View pixels that QM gets from the window/control when 'background' is checked.[]QM cannot get pixels from some windows. Then all pixels usually are black.[]If Aero not enabled or the window is DPI-scaled, cannot get pixels from window[]parts covered by other windows."
 1503 Static 0x54000000 0x0 146 4 100 10 "Wait for this URL (optional)"
 1502 Edit 0x44030080 0x204 146 14 242 14 "URL"
 1501 QM_DlgInfo 0x54000000 0x20000 146 36 242 66 "Waits while web browser is busy. Works only with Internet Explorer or other compatible window (see Help). To have more control, use ''Open web page'' dialog. Alternatively, use ''Wait for active window'' action (to wait until browser window's title changes), or ''Find html element'' dialog (to wait for some element in new web page).[][]You can optionally specify an URL to wait for. Can be partial."
 1601 Static 0x44020000 0x4 148 6 52 12 "Object handle"
 1602 Edit 0x44030080 0x204 202 4 44 14 "Han"
 1603 QM_DlgInfo 0x54000000 0x20000 146 36 242 66 "Waits until this Windows kernel object becomes signaled. It can be thread (wait for exit), process (wait for exit), event, mutex, waitable timer, etc. In QM the most useful is waiting until ends thread started by mac. Waiting for multiple objects also is possible. Click the ? button to read more."
 1701 ListBox 0x54230101 0x200 148 4 90 117 "cur"
 1702 Static 0x54000803 0x0 254 6 26 22 ""
 1703 QM_DlgInfo 0x54000000 0x20000 254 82 134 22 "Now press Shift+F11 when the cursor is visible."
 1704 Edit 0x5C030080 0x200 254 106 96 14 "crc"
 1801 Static 0x44020000 0x4 148 6 32 10 "Button"
 1802 ListBox 0x54230101 0x200 148 18 90 46 "mou"
 1803 QM_DlgInfo 0x54000000 0x20000 148 98 240 22 "Waits for mouse button up event."
 1904 Static 0x54000000 0x0 148 8 30 12 "Handle"
 1905 Edit 0x54030080 0x200 180 6 96 14 "Han"
 1901 Static 0x54000000 0x0 148 26 30 12 "Name"
 1902 Edit 0x54030080 0x200 180 24 208 14 "Nam"
 1906 Static 0x54000000 0x0 148 42 30 13 "Name is"
 1903 ComboBox 0x54230243 0x0 180 42 96 213 "Nam"
 1907 Button 0x54012003 0x0 316 42 72 12 "Case insensitive"
 1 Button 0x54030001 0x4 4 178 48 15 "OK"
 2 Button 0x54030000 0x4 54 178 48 15 "Cancel"
 18 Button 0x54032000 0x4 104 178 16 15 "?"
 9 Button 0x54032000 0x0 122 178 32 15 "? More"
 7 Static 0x54000010 0x20004 0 170 416 1 ""
 4 Static 0x54000010 0x20000 148 145 244 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "4" "" "1103"

 messages
if(sub_to.ToolDlgCommon(&hDlg "3[]$qm$\wait.ico" "" 1)) ret wParam
sel message
	case WM_INITDIALOG
	SendDlgItemMessage hDlg 1103 SCI.SCI_SETTABWIDTH 24 0
	goto gAction
	
	case WM_COMMAND goto messages2
	
	case WM_HOTKEY
	sel wParam
		case 2468 sub.Cursor hDlg 2
	
	case __TWN_DRAGEND ;;finder drop
	if TO_Selected(hDlg 3)=12
		POINT& p=+lParam
		sub.FillColorAndXY hDlg pixel(p.x p.y 0) wParam p
ret
 messages2
sel wParam
	case 8 goto gAction ;;Not
	case [1104,1105] sub_to.VirtualKeysMenu id(1102 hDlg) wParam=1105 1
	case 1407 sub.PickColor hDlg
	case [1408,1409] ;;background, View pixels
	_i=SendDlgItemMessage(hDlg 1001 __TWM_GETCAPTUREDHWND 0 0)
	sel wParam
		case 1408 TO_Enable hDlg "1409" but(lParam); sub_scan.CanGetPixels _i hDlg 1408
		case 1409 if(IsWindow(_i)) sub_scan.ViewPixels _i but(1406 hDlg)<<4|0x1000; else mes "Need a window." "" "i"
	
	case LBN_SELCHANGE<<16|3
	 gAction
	A=TO_Selected(hDlg 3)
	DT_Page hDlg A "-1 0 0 0 0 0 9 1 8 2 3 6 (0 4) 5 7 -1"
	
	sel(A) case [1,2,4,5,6,9,10,12,14] _i=1
	TO_Show hDlg "8" _i ;;Not
	
	TO_Show hDlg "+510" A=12; SendDlgItemMessage hDlg 1001 __TWM_SETFLAGS A!12 0
	if(A=14) sub.Cursor hDlg 3
	
	lpstr st
	sel(A) case 0 st="Time, s"; case 15 st="Wait, ms"; case else st="Timeout, s"
	SetDlgItemText hDlg 5 st
	
	sel A
		case [1,2,3,4,5] st="handle of the window"; if(A=3 or (A<3 and but(8 hDlg))) st="handle of active window"
		case 7 st="virtual-key code"
		case 8 st="mouse button code"
		case else st=0
	SetDlgItemText hDlg 10 F"Get {st}" 
	TO_Show hDlg "10" st!0
	
	case 18 QmHelp "IDP_WAIT[]IDP_WAIT_FOR[]*[]*[]*[]*[]*[]*[]*[]*[]*[]*[]*[]*[]*[]IDP_SPE" TO_Selected(hDlg 3)
	case 9 QmHelp "::/commands/IDP_WAIT_FOR.html#aother"
	
	case LBN_SELCHANGE<<16|1701 sub.Cursor hDlg 1
ret 1

#opt nowarnings 1


#sub PickColor
function hDlg

__MinimizeDialog m.Minimize(hDlg); 0.5

RECT r; POINT p; int color
if(!CaptureImageOrColor(color 15 0 "" r)) ret
p.x=r.left; p.y=r.top

 let the window control get handle of window from p (0 if Screen), fill its controls etc, and return the handle or -1 on error
int h=SendDlgItemMessage(hDlg 1001 __TWM_DRAGDROP 1 &p); if(h=-1) ret

sub.FillColorAndXY hDlg color h p


#sub FillColorAndXY
function hDlg color hwnd POINT&p

EditReplaceSel hDlg 1402 F"0x{color%%06X}" 1
sub_to.MouseXY hDlg hwnd 1404 1405 1406 p


#sub Cursor
function hDlg action ;;action: 1 on list of cursors selected, 2 on hotkey, 3 show-hide controls

__RegisterHotKey-- t_hkcur
__GdiHandle-- t_hcur
int i

sel action
	case 1
	int c1(IDC_ARROW) c2(IDC_IBEAM) c3(IDC_WAIT) c4(IDC_CROSS) c5(IDC_SIZENWSE) c6(IDC_SIZENESW) c7(IDC_SIZEWE) c8(IDC_SIZENS) c9(IDC_SIZEALL) c10(IDC_NO) c11(IDC_HAND) c12(IDC_APPSTARTING) c13(IDC_HELP)
	int* pCur=&c1; int hcur
	i=TO_Selected(hDlg 1701)
	if(i>=0 and i<=&c13-&c1/4) hcur=LoadCursor(0 +pCur[i]); else if(i=13) hcur=t_hcur
	SendDlgItemMessage(hDlg 1702 STM_SETIMAGE IMAGE_CURSOR hcur)
	TO_Show hDlg "1703 1704" i=13
	if(i=13) if(!t_hkcur.Register(hDlg 2468 MOD_SHIFT VK_F11)) mes "Failed to register hotkey Shift+F11."
	else t_hkcur.Unregister
	
	case 2
	CURSORINFO ci.cbSize=sizeof(ci)
	if(!GetCursorInfo(&ci)) ret
	ICONINFO ii
	if(!GetIconInfo(ci.hCursor &ii)) ret
	if(ii.hbmColor) DeleteObject(ii.hbmColor)
	__GdiHandle h=CopyImage(ii.hbmMask IMAGE_BITMAP 0 0 LR_COPYDELETEORG|LR_CREATEDIBSECTION)
	BITMAP b
	if(!GetObjectW(h sizeof(b) &b) or !b.bmBits) ret
	_s.fromn(b.bmBits b.bmHeight*b.bmWidthBytes)
	_s.format("0x%I64X" ConvertSignedUnsigned(Crc32(_s _s.len))|0x100000000)
	_s.setwintext(id(1704 hDlg))
	t_hcur.Delete; t_hcur=CopyIcon(ci.hCursor)
	SendDlgItemMessage(hDlg 1702 STM_SETIMAGE IMAGE_CURSOR t_hcur)
	OnScreenDisplay "Cursor captured" 1
	
	case 3
	i=TO_Selected(hDlg 1701)
	TO_Show hDlg "1703 1704" i=13


#sub KeyCodes
function str&s
s=
  A	Alt	        O	Num Lock
  B	Backspace	        P	Page Up
  C	Ctrl	        Q	Page Down
  D	Down	        R	Right
  E	End	        S	Shift
  F1-F24	F1-F24	        T	Tab
  G	Pause	        U	Up
  H	Home	        V	Space
  I	Insert	        W	Win
  J	Scroll lock	        X	Delete
  K	Caps Lock	        Y	Enter
  L	Left	        Z	Esc
  M	Context Menu	
  N0-N9	Numpad 0-9	        (44)	Print Screen
  N/-N.	Numpad / * - + .
 
 For { } ( ) ; " keys, use [ ] 9 0 : '. For other text keys, use lowercase letters and other characters. For other non-text keys, use virtual key code (VK) in parentheses.
