 \Dialog_Editor
function! [hwndOwner] [idCode] [capture] ;;capture: 1 drag-capture immediately

 Shows 'Find accessible object' dialog.
 Returns 1 on OK.

 idCode - if used, stores all code in it (caller must call __strt.ReplaceVar), else activates QM and inserts code in code editor.

#region DD
str dd=
 BEGIN DIALOG
 1 "" 0x90CF0A48 0x100 0 0 452 304 "Find accessible object"
 4 Static 0x54020003 0x4 2 2 16 16 "Drag" ".1 Drag and drop to capture object.[]Right click for more options."
 18 Static 0x54020000 0x4 38 6 31 12 "Window"
 10 QM_Tools 0x54030000 0x10000 70 4 292 14 "1 64"
 1020 Static 0x54020000 0x4 2 28 19 13 "Role"
 1012 ComboBox 0x54230342 0x4 22 26 95 212 "Rol"
 1019 Button 0x54013003 0x4 2 46 39 12 "Name"
 1029 Button 0x54012003 0x4 66 46 35 12 "Use *" "Full or with wildcard characters *?.[]If unchecked, can be partial."
 1031 Button 0x54012003 0x4 102 46 43 12 "Regexp." "Regular expression"
 1033 Button 0x54032000 0x4 146 46 20 13 "RX" "Regular expression menu"
 1011 Edit 0x54231044 0x204 2 60 164 26 "Nam"
 1001 Static 0x54000000 0x0 180 28 70 10 "Other properties"
 1026 Button 0x54012003 0x4 270 26 35 12 "Use *" "Value/description is full or with wildcard characters *?.[]If unchecked, can be partial."
 1032 Button 0x54012003 0x4 306 26 43 12 "Regexp." "Value/description is regular expression."
 1014 QM_Grid 0x56035041 0x200 180 40 168 66 "0x27,0,0,6,0x10000000[],30%,,[],69%,," "Variables must be enclosed in { }"
 1160 Static 0x54000000 0x0 2 28 19 13 "Tag"
 1161 Edit 0x54030080 0x200 22 26 84 14 "Tag"
 1101 Button 0x54013003 0x0 2 46 39 12 "Text"
 1102 Button 0x54012003 0x0 66 46 35 12 "Use *" "Full or with wildcard characters *?.[]If unchecked, can be partial."
 1103 Button 0x54012003 0x0 102 46 43 12 "Regexp." "Regular expression"
 1104 Button 0x54032000 0x0 146 46 20 13 "RX" "Regular expression menu"
 1157 Edit 0x54231044 0x200 2 60 164 26 "Tex"
 1158 Static 0x54000000 0x0 180 28 70 10 "Attributes"
 1105 Button 0x54012003 0x0 270 26 35 12 "Use *" "Full or with wildcard characters *?.[]If unchecked, can be partial."
 1106 Button 0x54012003 0x0 306 26 43 12 "Regexp." "Regular expression"
 1155 QM_Grid 0x56035041 0x200 180 40 168 66 "0x23,0,0,6,0x10000000[],30%,,[],69%,8," "Variables must be enclosed in { }"
 1162 Static 0x54000000 0x0 118 24 60 20 ""
 1053 Button 0x54012003 0x4 368 41 74 13 "+ invisible objects"
 1054 Button 0x54012003 0x4 368 52 74 13 "+ useless objects"
 56 Button 0x54012003 0x4 368 65 74 13 "in reverse order" "Faster if the object is near the bottom of the object tree"
 52 Button 0x54012003 0x0 368 76 74 13 "in web page" "Search only in current web page.[]Faster and more reliable."
 6 Button 0x44012003 0x0 368 89 74 13 "as Firefox node" "Usually faster.[]Uncheck if does not find."
 23 Button 0x54013003 0x4 2 92 39 13 "Navigate"
 45 Edit 0x54030080 0x204 42 92 108 14 "Nav" "Use if need other object than the selected object.[]Check 'Navigate' and create path from this object to the object that you actually need.[]Look at the object tree below to see what path you need.[]Use the ... button to add path parts, and the Test button to see is it correct.[]To see all objects, check 'Show invisible' and 'Show useless'."
 50 Button 0x54032000 0x0 150 92 16 15 "..."
 24 Button 0x54013003 0x0 2 116 39 13 "Match #"
 51 Edit 0x54030080 0x200 42 116 23 14 "mat" "1-based match index.[]For example, use 2 for the second matching object."
 17 Static 0x54020000 0x4 88 118 30 12 "Wait, s"
 27 Edit 0x54030080 0x204 120 116 30 14 "Wai"
 47 Button 0x54012003 0x4 180 116 74 14 "Error if not found" "End macro if not found, unless error handled with err.[]Click the arrow to see example."
 12 Button 0x54030000 0x0 256 118 16 10 "6"
 37 Edit 0x4C030080 0x204 414 116 31 14 "Var"
 1 Button 0x54030001 0x4 4 146 48 14 "OK"
 2 Button 0x54030000 0x4 54 146 48 14 "Cancel"
 41 Button 0x54032000 0x4 104 146 48 14 "Test" "Test whether code created by this dialog will work, and how fast"
 28 Button 0x54032000 0x4 154 146 16 14 "?"
 48 Static 0x54000000 0x0 206 148 143 13 ""
 13 QM_DlgInfo 0x44000000 0x20000 180 140 204 26 ""
 11 Button 0x54032000 0x0 396 146 48 14 "Options..."
 5 Button 0x54012003 0x0 4 175 64 14 "Show invisible"
 7 Button 0x54012003 0x0 70 175 63 14 "Show useless"
 63 Button 0x54012003 0x0 134 175 56 14 "Only web"
 43 Button 0x54032000 0x4 210 175 48 14 "Properties..."
 8 Button 0x54032000 0x4 368 175 38 14 "Refresh"
 9 Button 0x54012003 0x4 408 175 36 14 "Auto"
 3 SysTreeView32 0x54010027 0x204 0 191 448 114 ""
 30 Static 0x54000000 0x0 134 210 51 13 "Please wait..."
 49 Static 0x54000010 0x20000 0 136 988 1 ""
 36 Static 0x54000010 0x20004 4 111 440 1 ""
 40 Static 0x54000010 0x20004 0 169 988 1 ""
 25 Button 0x54020007 0x0 362 28 83 77 "Search"
 END DIALOG
 DIALOG EDITOR: "___EA_CONTROLS" 0x2040202 "" "0" "" ""
#endregion

type ___EA_CONTROLS __strt'controls __strt'si4Dra __strt'qmt10 __strt'cb1012Rol __strt'c1019Nam __strt'c1029Use __strt'c1031Reg __strt'e1011Nam __strt'c1026Use __strt'c1032Reg __strt'qmg1014x __strt'e1161Tag __strt'c1101Tex __strt'c1102Use __strt'c1103Reg __strt'e1157Tex __strt'c1105Use __strt'c1106Reg __strt'qmg1155x __strt'c1053inv __strt'c1054use __strt'c56in __strt'c52in __strt'c6as __strt'c23Nav __strt'e45Nav __strt'c24Mat __strt'e51mat __strt'e27Wai __strt'c47Err __strt'e37Var __strt'c5Sho __strt'c7Sho __strt'c63Onl __strt'c9Aut
___EA_CONTROLS d.controls="4 10 1012 1019 1029 1031 1011 1026 1032 1014 1161 1101 1102 1103 1157 1105 1106 1155 1053 1054 56 52 6 23 45 24 51 27 47 37 5 7 63 9"

d.si4Dra="&$qm$\target.ico"
d.c1019Nam=1
d.cb1012Rol="&[]TITLEBAR[]MENUBAR[]SCROLLBAR[]GRIP[]SOUND[]CURSOR[]CARET[]ALERT[]WINDOW[]CLIENT[]MENUPOPUP[]MENUITEM[]TOOLTIP[]APPLICATION[]DOCUMENT[]PANE[]CHART[]DIALOG[]BORDER[]GROUPING[]SEPARATOR[]TOOLBAR[]STATUSBAR[]TABLE[]COLUMNHEADER[]ROWHEADER[]COLUMN[]ROW[]CELL[]LINK[]HELPBALLOON[]CHARACTER[]LIST[]LISTITEM[]OUTLINE[]OUTLINEITEM[]PAGETAB[]PROPERTYPAGE[]INDICATOR[]GRAPHIC[]STATICTEXT[]TEXT[]PUSHBUTTON[]CHECKBUTTON[]RADIOBUTTON[]COMBOBOX[]DROPLIST[]PROGRESSBAR[]DIAL[]HOTKEYFIELD[]SLIDER[]SPINBUTTON[]DIAGRAM[]ANIMATION[]EQUATION[]BUTTONDROPDOWN[]BUTTONMENU[]BUTTONDROPDOWNGRID[]WHITESPACE[]PAGETABLIST[]CLOCK[]SPLITBUTTON[]IPADDRESS[]OUTLINEBUTTON"
d.c47Err=1
rget _i "Auto" "\EA" 0 1; d.c9Aut=_i
rget _i "Filters" "\EA"; TO_FlagsToCheckboxes _i d.c5Sho d.c7Sho d.c63Onl

int style; if(!hwndOwner and GetWinStyle(win 1)&WS_EX_TOPMOST) style|DS_SYSMODAL
if(!ShowDialog(dd &sub.DlgProc &d hwndOwner 0 style 0 capture)) ret

str s
if(!EA_Format(d s)) ret

if idCode
	act hwndOwner; err
	s.setwintext(id(idCode hwndOwner))
else
	__strt v.ReplaceVar(s)
	act _hwndqm; err
	InsertStatement s

ret 1


#sub DlgProc
function# hDlg message wParam lParam

type ___EA_ENUM tv ARRAY(int)ha
type ___EA_ARRAY Acc'a htvi RECT'r
type ___EA
	hwnd Acc'ai
	ARRAY(___EA_ARRAY)ar !working
	__Settings'o
	hwndCapturing
	!isCaptureSmallest !isIgnoreTreeWmNotify !isFilled !isTreePlusInvisible !isFirefoxDocAlwaysBusy
	BSTR'bGDI ;;for tvn_getdispinfow

___EA- dA
int i

if(sub_to.ToolDlgCommon(&hDlg "[]$qm$\acc.ico[]EA_Main2" "" 1)) ret wParam
sel message
	case WM_INITDIALOG
	___EA _e; dA=_e
	dA.o.Init("defWait,Default wait,[]defWaitWeb,Default wait in web page,3[]FF,<2>Prefer 'as Firefox node',[]acc,<2>Use acc (for QM < 2.3.3)," "Options" "\EA" 1)
	DT_Page hDlg 0
	sub_to.CheckboxPushlikeNoTheme hDlg
	TO_ButtonsAddArrowSimple hDlg "12"
	RegWinPos(hDlg "winpos2" "\EA" 0)
	if(DT_GetParam(hDlg)&1) EA_Capture hDlg WM_LBUTTONDOWN
	
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
	
	case WM_DESTROY
	rset but(9 hDlg) "Auto" "\EA"
	rset (but(63 hDlg)*4)|(but(7 hDlg)*2)|but(5 hDlg) "Filters" "\EA"
	RegWinPos(hDlg "winpos2" "\EA" 1)
	
	case [WM_LBUTTONDOWN,WM_RBUTTONDOWN]
	if(GetDlgCtrlID(child(mouse))=4) EA_Capture hDlg message
	
	case WM_SIZE
	int tv=id(3 hDlg)
	GetWinXY(tv 0 i 0 0 hDlg)
	siz lParam&0xffff lParam>>16-i tv
	
	case __TWN_WINDOWCHANGED ;;window control: text changed. wParam is 0 or handle of selected window
	if(!wParam or wParam=dA.hwnd) ret
	dA.ai=acc(wParam); err ret
	dA.hwnd=wParam
	dA.isFilled=0
	EA_Proc hDlg
	if(!dA.isFilled) EA_Fill hDlg dA.ai
	
ret
 messages2
TO_EditCheckOnChange hDlg 1011 1019  45 23  51 24  1157 1101
sel wParam
	case 41 EA_Test hDlg
	case 28 QmHelp "IDP_ACCESSIBLE"
	case 43 sub.Info hDlg ;;Properties
	case [1029,1031,1033] sub.WCRX hDlg wParam 1029 1031 1033 1011
	case [1026,1032] sub.WCRX hDlg wParam 1026 1032
	case [1102,1103,1104] sub.WCRX hDlg wParam 1102 1103 1104 1157
	case [1105,1106] sub.WCRX hDlg wParam 1105 1106
	case 50 sub.NavigMenu hDlg 45 ;;Navigate
	
	case 6 DT_Page hDlg but(lParam) ;;Firefox
	case 11 dA.o.OptionsDialog(hDlg)
	case 12 sub.ExampleNotFound hDlg
	
	case 8 if(!dA.working) EA_Proc hDlg; else dA.working~1 ;;Refresh/Stop
	case IDCANCEL if(dA.working) dA.working~1; ret
	case 9 if(but(lParam)) EA_Proc hDlg ;;Auto
	case [5,7,63] EA_Proc hDlg ;;Show invisible/useless/onlyweb
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 3
	if(dA.isIgnoreTreeWmNotify) ret
	ret DT_Ret(hDlg sub.OnNotifyTree(hDlg nh.code +nh))
	
	case [1014,1155]
	ret DT_Ret(hDlg sub.OnNotifyGrid(hDlg nh.code nh))


#sub NavigMenu
function# hDlg idedit

str s="Next[]Previous[]Parent[]First child[]Last child[]Child n[]-[]Up[]Down[]Left[]Right"
int i=ShowMenu(s hDlg 0 2); if(!i) ret
s.getl(s i-1); s.gett(s 0); s.lcase
if(i=6) if(inp(_i "1-based child index" "" "1") && _i>0) s+_i; else ret
s+" "
EditReplaceSel(hDlg idedit s 2)
ret i


#sub Info
function hDlg

___EA- dA
Acc& a=dA.ai; if(!a.a) ret

str role name value descr defa state cls c1 c2 help javaActions
int x y cx cy h cid isJava
opt err 1
a.Role(role)
name=a.Name
value=a.Value
descr=a.a.Description(a.elem)
defa=a.a.DefaultAction(a.elem)
a.State(state)
a.Location(x y cx cy)
if(!a.elem) help=a.a.Help; if(help="QM Java object") isJava=1; if(defa.len) javaActions=a.a.DefaultAction(1)
h=child(a); if(h) cls.getwinclass(h); cid=GetDlgCtrlID(h)
c1=0xffffff; c2=0xc0ffe0
str* p
for p &name &defa sizeof(str)
	str& r=p
	int i=findl(r 3); if(i>0 and i<r.len) r.fix(i); r+"..."
	r.findreplace("[10]" "[10][9]")

str dd=
 BEGIN DIALOG
 0 "" 0x90CE0AC8 0x0 0 0 256 134 "Accessible object properties"
 3 QM_DlgInfo 0x54000005 0x20000 0 0 256 112 ""
 2 Button 0x54030000 0x4 104 116 48 14 "Close"
 END DIALOG
 DIALOG EDITOR: "" 0x2040201 "*" "" "" "3"

str controls = "3"
str qmdi3
qmdi3=
F
 <><Z {c1}>Name:	{name}</Z>
 <Z {c2}>Role:	{role}</Z>
 <Z {c1}>Value:	{value}</Z>
 <Z {c2}>Description:	{descr}</Z>
 <Z {c1}>State:	{state}</Z>
 <Z {c2}>Location:	x={x}  y={y}  width={cx}  height={cy}</Z>
 <Z {c1}>elem:	{a.elem}</Z>
 <Z {c2}>Control class:	{cls}</Z>
 <Z {c1}>Control id:	{cid}</Z>
 <Z {c2}>Default action:	{defa}</Z>
if(isJava) qmdi3+F"[]<Z {c1}>Java actions:	{javaActions}</Z>"

ShowDialog(dd &sub.DlgProcInfo &controls hDlg 1)


#sub DlgProcInfo
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	DT_SetAutoSizeControls hDlg "3s 2mv"
	SendMessage id(3 hDlg) SCI.SCI_SETTABWIDTH 28 0
	case WM_COMMAND ret 1
	case else ret


#sub OnNotifyTree
function# hDlg code NMTREEVIEWW&nm

___EA- dA

sel code
	case TVN_SELCHANGEDW
	if(nm.itemNew.hItem) EA_Fill hDlg dA.ar[nm.itemNew.lParam].a
	
	case TVN_GETDISPINFOW
	err-
	NMTVDISPINFOW& di=+&nm
	TVITEMW& ti=di.item
	if(ti.mask&TVIF_TEXT=0) ret
	Acc& a=dA.ar[ti.lParam].a
	str s rs ns vs ds
	int r=a.Role(rs)
	ns=a.Name; err if(r=0 and rs="unknown") ns="Failed to get object properties. Possibly it already does not exist."
	
	if(r=ROLE_SYSTEM_WINDOW)
		_s.getwinclass(child(a)); err
		s.format("%s  (%s)  ''%s''" rs _s ns)
	else
		vs=a.Value; err ;;in Chrome, text of TEXT is in Value
		ds=a.Description; err ;;in QT apps, text is in Description
		s.format("%s  ''%s''" rs ns)
		if(vs.len+ds.len) s.formata("  ''%s''" vs)
		if(ds.len) s.formata("  ''%s''" ds)
	if(s.len>100) s.fix(100); s+"..."
	
	err+ s="<error>"
	dA.bGDI=s
	ti.pszText=dA.bGDI
	ti.mask|TVIF_DI_SETITEM
	
	case NM_CUSTOMDRAW ;;customdraw for invisible
	NMTVCUSTOMDRAW& nt=+&nm
	NMCUSTOMDRAW& cd=nt.nmcd
	sel cd.dwDrawStage
		case CDDS_PREPAINT ret CDRF_NOTIFYITEMDRAW
		case CDDS_ITEMPREPAINT
		if(cd.uItemState&CDIS_SELECTED) nt.clrTextBk=0xffe0c0; nt.clrText=0
		if dA.isTreePlusInvisible
			_i=dA.ar[cd.lItemlParam].a.State; err ret
			if(_i&STATE_SYSTEM_INVISIBLE) nt.clrText=0x808080


#sub OnNotifyGrid
function# hDlg code NMHDR*nh

sel nh.code
	case GRID.LVN_QG_CHANGE
	TO_GridCheckOnChange nh nh.idFrom
	
	case GRID.LVN_QG_BUTTONCLICK
	if(nh.idFrom!1014) ret
	GRID.QM_NMLVDATA* cd=+nh
	DlgGrid g.Init(nh.hwndFrom)
	sel g.CellGet(cd.item 0)
		case "state"
		int state(val(cd.txt 0 _i)) mask(val(cd.txt+_i))
		if(!sub_to.FlagsMaskDialog(state mask "unavailable (disabled)[]selected[]focused[]pressed[]checked[]mixed (3-rd state)[]read only[]hot tracked[]default[]expanded[]collapsed[]busy[]floating[]marqueed[]animated[]invisible[]offscreen[]sizeable[]moveable[]self voicing[]focusable[]selectable[]linked[]traversed[]multiple selectable[]extended selectable[]alert low[]alert medium[]alert high[]protected (password)[]has popup" hDlg "State")) ret
		_s=F"0x{state} 0x{mask}"
		_s.setwintext(cd.hctrl); err mes "failed to update the grid, please retry"


#sub ExampleNotFound
function hDlg
if(!ShowMenu("1 Show 'if not found' example" hDlg)) ret
_s=
 <><code> if 'Error if not found' is checked, you can optionally handle the error with err
 Acc a.Find(...)
 err ;;not found, or other error
 	out "not found"
 	ret
;
  if 'Error if not found' is unchecked, you can use if/else
 Acc a.Find(...)
 if a.NotFound
 	out "not found"
 	ret
 </code>
out _s


#sub WCRX
function hDlg idClick idcWC idcRX [idbRX] [idEdit]

 Use if there are 4 related controls:
   checkbox "Use *" (idcWC),
   checkbox "Regexp" (idcRX),
   button "RX" (idbRX),
   edit control.
 Call on click of one of these buttons.


if idClick=idcWC
	TO_Check(hDlg F"{idcRX}" 0)
else
	TO_Check(hDlg F"{idcWC}" 0)
	if idClick=idbRX
		TO_Check(hDlg F"{idcRX}" 1)
		RegExpMenu id(idEdit hDlg)
