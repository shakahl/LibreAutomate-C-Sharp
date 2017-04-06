 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 5 1002 1003 1004 1005 1010 1101 1102 1103 1201 1202 1302"
__strt lb3Act qmt5 cb1002X cb1003Y c1004Cli c1005Res cb1010opt lb1101rel e1102rX e1103rY lb1201get c1202Cli e1302Whe

TO_FavSel wParam lb3Act "Left click[]Double click[]Right click[]Middle click[]Move[]Relative move[]Save mouse position[]Restore saved position[]Restore initial position[]Get mouse position[]Wheel"
cb1010opt="&Click[]Press[]Release"
lb1101rel="&Move from current position[]Move from position set by previous mouse command"
lb1201get="&Get x and y[]Get x[]Get y"
cb1002X="[]0,,,Pixels[]0.0,,,''Fraction of window/control/screen, eg 0.5 is 50%''"; cb1003Y="[]0,,,Pixels[]0.0,,,''Fraction of window/control/screen, eg 0.5 is 50%''"
c1004Cli=1; c1202Cli=1

if(!ShowDialog("" &TO_Mouse &controls _hwndqm)) ret

str s winVar
__strt vd

int i=val(lb3Act)

if(i<=4 or i=9) qmt5.Win(winVar "0")

sel i
	case [0,1,2,3,4]
		s.gett("lef dou rig mid mou" i)
		sel(i) case [0,2,3] s+cb1010opt.SelC(" +-")
		if cb1002X.len or cb1003Y.len or winVar!0 or i=4
			int fl=val(c1004Cli); if(c1005Res=1 and i!4) fl|4
			s+F" {cb1002X.N(`0.5`)} {cb1003Y.N(`0.5`)} {winVar} {fl}"
			sub_to.Trim s " 0 0"
	case 5 s=F"mou{lb1101rel.SelC(`+-`)} {e1102rX.N} {e1103rY.N}"
	case 6 s=F"{vd.VD(`-id POINT _m; ` _s)}xm _m ;;save mouse position in _m"; goto gIns
	case 7 s="mou _m.x _m.y"; goto gIns
	case 8 s="mou"; goto gIns
	case 9
		_i=val(lb1201get)
		sel _i
			case 0 s=F"{vd.VD(`POINT p; ` _s)}xm({_s}"
			case 1 s=F"{vd.VD(`int x`)}=xm(0"
			case 2 s=F"{vd.VD(`int y`)}=ym(0"
		s+F" {winVar} {c1202Cli})"; sub_to.Trim s "0 0 0"
		if(_i=0) s+F" ;;get mouse position into {_s}.x and {_s}.y"
	case 10 s=F"MouseWheel({e1302Whe.N})"

qmt5.WinEnd(s 0 i=9)

 gIns
 sub_to.TestDialog s i
InsertStatement s
ret

 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 365 140 "Mouse"
 3 ListBox 0x54230101 0x204 4 4 104 102 "Action"
 5 QM_Tools 0x54030000 0x10000 122 4 240 54 "1 0xC00"
 1001 Static 0x54000000 0x4 124 76 18 10 "X, Y"
 1002 QM_ComboBox 0x54230242 0x0 144 74 48 13 "X" "Move mouse to this position.[]To click without moving, don't change anything in this dialog."
 1003 QM_ComboBox 0x54230242 0x0 196 74 48 13 "Y" "Move mouse to this position.[]To click without moving, don't change anything in this dialog."
 1004 Button 0x54012003 0x0 248 74 54 13 "Client area" "Coordinates are relative to the client area of the window or control, or work area of screen.[]Client area usually is all except border, caption, standard menubar and scrollbars.[]It is recommended to use client area because size of non-client area depends on Windows version, theme etc."
 1005 Button 0x54012003 0x0 124 94 94 13 "Restore mouse position" "After, move the mouse pointer back"
 1006 Static 0x54000000 0x0 260 96 44 12 "How to click"
 1010 ComboBox 0x54230243 0x0 306 94 56 213 "opt"
 1105 Static 0x54000000 0x0 122 66 24 12 "From"
 1101 ListBox 0x54230101 0x200 148 64 214 22 "rel"
 1104 Static 0x54000000 0x4 122 92 24 13 "X, Y"
 1102 Edit 0x54030080 0x204 148 90 38 14 "rX" "Distance"
 1103 Edit 0x54030080 0x204 190 90 38 14 "rY" "Distance"
 1201 ListBox 0x54230101 0x200 122 70 98 36 "get"
 1202 Button 0x54012003 0x0 228 70 60 13 "Client area" "Get mouse coordinates relative to the client area of the window or control, or work area of screen."
 1301 Static 0x54020000 0x0 122 76 190 13 "Number of wheel clicks, > 0 forward, < 0 backward"
 1302 Edit 0x54030080 0x200 122 90 48 14 "Whe"
 1 Button 0x54030001 0x4 4 121 48 14 "OK"
 2 Button 0x54010000 0x4 54 121 50 14 "Cancel"
 8 Button 0x54032000 0x4 106 121 16 14 "?"
 6 Button 0x54032000 0x0 124 121 34 14 "? More"
 7 Static 0x54000010 0x20004 0 113 380 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

 messages
if(sub_to.ToolDlgCommon(&hDlg "3[]$qm$\mouse.ico" "" 1)) ret wParam
sel message
	case WM_INITDIALOG goto g11
	case WM_COMMAND goto messages2
	case __TWN_DRAGEND ;;finder drop
	if(TO_Selected(hDlg 3)<=4) sub_to.MouseXY hDlg wParam 1002 1003 1004 +lParam 1
ret
 messages2
sel wParam
	case 8 QmHelp "IDP_LEF[]*[]*[]*[]IDP_MOU[]*[]IDP_XMYM[]IDP_MOU[]*[]IDP_XMYM[]MouseWheel" TO_Selected(hDlg 3)
	case 6 goto gMore
	case LBN_SELCHANGE<<16|3
	 g11
	i=TO_Selected(hDlg 3)
	DT_Page hDlg i "0 0 0 0 0 1 -1 -1 -1 2 3"
	TO_Show hDlg "5" i<=4||i=9
	if(i=1 or i=4) TO_Show hDlg "1010 1006" 0; if(i=4) TO_Show hDlg "1005" 0
ret 1

 gMore
lpstr st=
 <b>Other ways to find and click UI objects</b>
;
 Accessible object functions. Can find and click buttons, links and other objects in windows and web pages. Can use mouse or not.
 Dialogs: <macro "EA_Main">find</macro>, <macro "TO_Accessible">actions</macro>.
;
 Html element functions. Similar to accessible objects. Works only in web pages in Internet Explorer and IE-based browsers and controls.
 Dialogs: <macro "EH_Main">find</macro>, <macro "TO_Html">actions</macro>.
;
 Click a standard menu item. <macro "TO_Menu">Dialog</macro>. Does not use mouse.
;
 Click a check box, select combo box, list box or tab control item. <macro "TO_Controls">Dialog</macro>. Does not use mouse. Works only with standard controls, not in web pages.
;
 Find image on screen. <macro "TO_Scan">Dialog</macro>.
;
 Find and click text in a window. <macro "TO_WindowText">Dialog</macro>. Works not with all windows.
;
 If possible, use keyboard to click menu items and dialog controls. Usually it's more reliable than lef because does not depend on object position. You can record or use the "Keys" dialog.
;
 In macros never use mouse to click a taskbar button or double-click a desktop icon. Instead activate the window with <macro "TO_Window">act</macro> or launch the program with <macro "TO_FileRun">run</macro>.
;
 <b>Other ways to add a mouse command</b>
;
 Move mouse over the object and press Ctrl+Shift+Alt+W. You can change the hotkey in Options.
;
 Record. The default global hotkey is Ctrl+Shift+Alt+R.
;
QmHelp st 0 6
