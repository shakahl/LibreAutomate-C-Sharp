 /Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 245 158 "Options"
 1001 Button 0x54012003 0x0 8 24 216 12 "Add/update show-dialog code in macro" "On Save, add/update control variables and ShowDialog line in &controls containing dialog definition.[]If unchecked, shows the code in QM output, then you can copy/paste it anywhere."
 1010 Static 0x54000000 0x0 8 40 94 12 "Add variables for controls"
 1011 Edit 0x54030080 0x200 104 40 134 12 "Ids" "Dialog Editor adds variables only for some control types. Here you can specify more controls.[]A list of control ids, like ''5 7 10-15''."
 1004 Static 0x54020000 0x4 8 56 94 12 "Use this type for variables"
 1005 Edit 0x54030080 0x204 104 56 134 12 "Type" "If you want to store dialog variables in a single variable of a user defined type instead of multiple str variables, enter type name here. The type definition will be created/updated on Save."
 1008 Static 0x54000000 0x0 8 72 94 12 "Map pages"
 1009 Edit 0x54030080 0x200 104 72 134 12 "Map" "Can be used with multipage dialogs. If you use function DT_Page with a map string, use the same string here."
 1006 Button 0x54012003 0x0 8 88 48 12 "Unicode" "Check this if this dialog uses dialog procedure that handles Unicode messages."
 1101 Static 0x44020000 0x4 8 24 94 12 "Snap to grid"
 1102 Edit 0x44032000 0x204 104 24 32 12 "[9]Grid" "Dialog Editor does not move/resize controls by less than this value.[]The unit is dialog units, like in dialog definition. It depends on font, and normally is ~1.5 pixels.[]Valid values are 0-20."
 1103 Button 0x44032000 0x4 160 24 78 14 "Background color..." "Dialog color in Dialog Editor."
 1104 Static 0x44020000 0x4 8 44 94 21 "Add variables for controls of these classes"
 1105 Edit 0x44231044 0x204 104 42 134 36 "Cla" "Dialog Editor adds variables only for some control types. Here you can specify more classes.[]See also QmSetWindowClassFlags."
 1106 Static 0x54000000 0x0 8 84 94 13 "Hotkey to capture controls"
 1108 msctls_hotkey32 0x54030000 0x200 104 84 134 13 "" "You can use this hotkey to add a control that is similar to an existing control from mouse in any window."
 1107 Edit 0x44030080 0x204 200 30 6 6 "Bac"
 1 Button 0x54030001 0x4 4 140 48 14 "OK"
 2 Button 0x54030000 0x4 54 140 48 14 "Cancel"
 4 Button 0x54032000 0x4 104 140 16 14 "?"
 3 SysTabControl32 0x50000040 0x0 0 0 246 134 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040200 "*" "0" "" ""

str controls = "1001 1011 1005 1009 1006 1102 1105 1107"
__strt c1001Add e1011Ids e1005Typ e1009Map c1006Uni e1102Gri e1105Cla e1107Bac

 this dialog
c1001Add=_updateCode
sel(_dialogFlags&3) case 1 c1006Uni=1; case 2 if(_unicode) c1006Uni=1 ;;2 fbc
e1005Typ=_userType
e1011Ids=_userIdsVarAdd
e1009Map=_pageMap

 dialog editor
e1102Gri=_grid
e1107Bac=_bgColor
e1105Cla=_userClassesVarAdd

if(!ShowDialog(dd &sub.DlgProcOptions &controls _hwnd 0 0 0 &this)) ret

 this dialog
e1005Typ.VN
int isUpdate(val(c1001Add)) isUnicode(val(c1006Uni))
if(_updateCode!=isUpdate or isUnicode!=_dialogFlags or _userType!=e1005Typ or _userIdsVarAdd!=e1011Ids or _pageMap!=e1009Map) _save=1

_updateCode=isUpdate
_dialogFlags=isUnicode
_userType=e1005Typ
_userIdsVarAdd=e1011Ids
int updatePage=(_pageMap!=e1009Map and _page>=0)
_pageMap=e1009Map
if(updatePage) DT_Page _hform _page _pageMap

 dialog editor
_grid=val(e1102Gri); if(_grid>20) _grid=20
if(val(e1107Bac)!_bgColor) _bgColor=val(e1107Bac); _brushBack.Delete
RedrawWindow(_hform 0 0 RDW_INVALIDATE|RDW_ERASE)
if(!e1105Cla.len) e1105Cla.all
_userClassesVarAdd=e1105Cla

rset _grid "Grid" "\Dialog Editor"
rset _bgColor "bgcol" "\Dialog Editor"
 rset _flags "Flags" "\Dialog Editor"
rset _userClassesVarAdd "Classes" "\Dialog Editor"


#sub DlgProcOptions
function# hDlg message wParam lParam

__DE* d=+DT_GetParam(hDlg)
ret d.sub._DlgProcOptions(hDlg message wParam lParam)


#sub _DlgProcOptions c
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	TO_TabAddTabs hDlg 3 "This dialog[]Dialog Editor" 1
	DT_Page hDlg 0
	
	if(rget(_i "Hotkey capture" "\Dialog Editor") and _i) SendDlgItemMessage(hDlg 1108 HKM_SETHOTKEY _i 0)
	
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	_i=SendDlgItemMessage(hDlg 1108 HKM_GETHOTKEY _i 0)
	rset _i "Hotkey capture" "\Dialog Editor"
	
	if(_i) _hkCapture.Register(_hwnd 1 _i 0)
	else _hkCapture.Unregister
	
	case 1003
	str s.getwintext(id(1002 hDlg 1))
	if(findrx(s "(?m)^\*$")<0) s.addline("*"); s.setwintext(id(1002 hDlg 1))
	
	case 1103 if(ColorDialog(0 &s)) s.setwintext(id(1107 hDlg 1))
	 
	case 4 QmHelp "IDH_DIALOG_EDITOR"
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.code
	case TCN_SELCHANGE
	DT_Page hDlg SendMessage(nh.hwndFrom TCM_GETCURSEL 0 0)
