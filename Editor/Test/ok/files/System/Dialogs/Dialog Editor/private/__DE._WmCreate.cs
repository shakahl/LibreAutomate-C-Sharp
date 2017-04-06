 \Dialog_Editor
function! CREATESTRUCTW*cs

int flags=cs.lpCreateParams
_newDialog=flags&1
_il.Load("$qm$\il_de.bmp")
_brushMark=CreateSolidBrush(0xFF)
rget _bgColor "bgcol" "\Dialog Editor" 0 0xff8080
rget _grid "Grid" "\Dialog Editor" 0 4
rget _userClassesVarAdd "Classes" "\Dialog Editor"
if(rget(_i "Hotkey capture" "\Dialog Editor") and _i) _hkCapture.Register(_hwnd 1 _i 0)
_page=-1
SetProp(_hwnd "qm_design_mode" 1) ;;enable design mode for ActiveX controls
_monitor=_hwnd ;;show msgboxes etc on monitor of _hwnd

 child dialog with most controls
str dd=
 BEGIN DIALOG
 1 "" 0x50000048 0x10000 0 20 109 210 ""
 1010 SysTreeView32 0x54038C00 0x200 2 36 106 114 ""
 3 Static 0x54000000 0x0 4 0 64 10 "" ".1 Control variable.[]Dialog Editor creates the name from control type, id and some text."
 1020 Edit 0x540310C4 0x200 2 14 106 14 "" "Control text or dialog name. Also used for control variable name.[]Will not set text of editable Edit controls and controls that don't display text."
 4 Static 0x54000000 0x0 4 160 32 12 "Page"
 1013 Edit 0x54032080 0x200 38 158 36 14 "" "0- based page index that will be used with function DT_Page.[]Empty - always visible controls."
 1011 Button 0x5C032000 0x0 76 158 16 14 "<"
 1012 Button 0x54032000 0x0 92 158 16 14 ">"
 END DIALOG
 DIALOG EDITOR: "" 0x2040200 "" "0" "" ""
ShowDialog(dd &sub.DlgProcLeftPane 0 _hwnd 0x100)

 form offset
RECT r; GetClientRect _hpane &r
_xform=r.right+8
GetWinXY _hpane 0 _yform _xform 0 _hwnd
_xform+8

 toolbar
_htb=CreateControl(0 "ToolbarWindow32" 0 TBSTYLE_LIST|TBSTYLE_FLAT|TBSTYLE_TOOLTIPS|CCS_NOPARENTALIGN|CCS_NODIVIDER|CCS_NORESIZE 0 0 800 _yform _hwnd 1000)
SendMessage _htb TB_SETEXTENDEDSTYLE 0 TBSTYLE_EX_MIXEDBUTTONS
lpstr btns=
 1022 25 "Text[]Control text or dialog name.[]Same as the edit field below. Use to edit bigger text."
 1007 29 "Tooltip[]Control tooltip text, or dialog tooltip properties."
 1009 30 "Styles[]Control or dialog window styles."
 1021 31 "Control properties[]Available for: Grid." 0 4
 1016 32 "Events[]Adds code to receive Windows messages or ActiveX control events."
 1013 34 "Copy/paste control    Alt+E"
 1008 33 "Delete control    Delete" 0 4
 -
 1014 16 "Undo    Ctrl+Z" 0 4
 1015 17 "Redo    Ctrl+Y" 0 4
 -
 1004 18 "Test dialog[]Tip: you can press Esc to close."
 1006 19 Options
 1030 43 "Menu Editor"
 -
 1001 15 "Save" 0x40
 1002 27 Close 0x40
 -
 1005 20 "Help[]_____________________[]Add control: click in toolbox, move mouse, click to drop.[]Move control: drag. Or arrows.[]Resize control or dialog: right-drag. Or Shift+arrows.[]Select control or dialog: click. Or Tab, Shift+Tab.[]Z-order top: Shift+click.[]Z-order bottom: Ctrl+click.[]Z-order behind selected: Ctrl+Shift+click.[]Move multiple: Shift+drag Group control.[]Move/resize precisely: Alt+drag."
TO_TbInit _htb btns _il 0 BTNS_AUTOSIZE 0 15

 red marker
_hmark=CreateControl(0 "Static" "" WS_DISABLED 0 0 0 0 _hwnd 1000)

 create form (load from dialog definition or create new)
int cancel
if !sub.Open(cancel)
	if(cancel or !_CreateForm) ret ;;let wm_create return -1 to destroy the window. Not end(), because on wm_create it crashes QM on Windows 8+.

_Page(_page) ;;set initial page
_save=0
CenterWindow _hwnd
DT_MouseWheelRedirect
SetFocus _htv; SetFocus _hwnd ;;treeview control bug: on click does not expand folder if never focused before

ret 1


#sub DlgProcLeftPane
function# hDlg message wParam lParam
__DE* d=+GetWindowLong(GetParent(hDlg) 0)
ret d.sub._DlgProcLeftPane(hDlg message wParam lParam)


#sub _DlgProcLeftPane c
function# hDlg message wParam lParam

sel message
	case WM_CREATE
	_hpane=hDlg
	
	 controls toolbox
	_htv=id(1010 _hpane)
	_FillControlsToolbox
	
	 focused control props
	_hselName=id(3 _hpane)
	_hselText=id(1020 _hpane)
	
	case WM_COMMAND
	_WmCommand(wParam lParam)
	ret 1
	
	case WM_NOTIFY
	NMHDR& nh=+lParam
	sel nh.idFrom
		case 1010 goto gNotifyToolbox
ret

 gNotifyToolbox
sel nh.code
	case NM_CLICK
	TVHITTESTINFO ht; xm ht.pt nh.hwndFrom 1
	SendMessage(nh.hwndFrom TVM_HITTEST 0 &ht)
	if(ht.flags&0x46=0) ret
	_AddControl(sub_to.TvGetParam(nh.hwndFrom ht.hItem) 1)


#sub Open c
function# &cancel

int i nDD ddMacro
str s
__Subs x

 get macro text, find all subs and dialog definitions
ddMacro=qmitem
nDD=x.Init(ddMacro); if(!nDD) ret

 get dialog definition
if nDD=1 or _newDialog ;;open the last DD
	for(i x.a.len-1 0 -1) if(x.a[i].dd.len) break
else ;;show list of subs containing DD, let user select
	x.List(s 0 ddMacro)
	i=ListDialog(s "Which dialog to edit?" "Dialog Editor" 2)-1
	if(i<0) cancel=1; ret
__Sub& r=x.a[i]
s.get(x.sText r.dd.offset r.dd.len)
s+"[]"

_ddMacro=ddMacro
_ddSub=r.name

 get dialog editor options
if r.dd.optionsOffset
	_s.getl(x.sText+r.dd.optionsOffset 0)
	ARRAY(str) a; tok(_s a -1 " ''" 6)
	if(a.len>=1 and a[0].len) _userType=a[0]
	if(a.len>=3 and a[2].len) if(findc(a[2] '*')>=0) _updateCode=1 ;;fbc use * instead of a flag
	_page=iif((a.len>=4 and a[3].len and isdigit(a[3][0])) val(a[3]) -1)
	if(a.len>=5) _pageMap=a[4]
	if(a.len>=6) _userIdsVarAdd=a[5]

ret _CreateForm(s)
