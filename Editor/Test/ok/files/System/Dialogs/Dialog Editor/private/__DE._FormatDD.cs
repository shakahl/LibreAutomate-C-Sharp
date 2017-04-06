 /Dialog_Editor
function str&sDD [str&controls1] [str&controls2]

str cls txt varName
__IdStringParser varids

 dialog
RECT r; GetClientRect(_hform &r)
TO_AdjustWindowRect r 0 0 _hform 4
r.right=MulDiv(r.right 4 _dbx); r.bottom=MulDiv(r.bottom 8 _dby)
___DE_CONTROL& c=_ac[0]
txt=c.txt; txt.escape(1)
int stl=c.style|DS_SETFONT; if(stl&WS_POPUP) stl~WS_CHILD
sDD=F" BEGIN DIALOG[] {_dialogFlags} ''{_dialogClass}'' 0x{stl} 0x{c.exstyle} {_xDlg} {_yDlg} {r.right} {r.bottom} ''{txt}''"
if(c.tooltip.len) _s=c.tooltip; sDD+F" ''{_s.escape(1)}''"
sDD+"[]"

if &controls1
	controls1.all; controls2.all
	if(_userIdsVarAdd.len) varids.Parse(0 _userIdsVarAdd); sub.AddControlVar(0 "d0" 0)

 controls
int h(_hform) gw(GW_CHILD)
rep
	h=GetWindow(h gw); gw=GW_HWNDNEXT; if(!h) break
	
	GetWindowRect(h &r); MapWindowPoints(0 _hform +&r 2)
	r.left=MulDiv(r.left 4 _dbx); r.right=MulDiv(r.right 4 _dbx); r.top=MulDiv(r.top 8 _dby); r.bottom=MulDiv(r.bottom 8 _dby)
	
	&c=subs.GetControl(h)
	stl=c.style
	cls.getwinclass(h)
	
	sel cls 1
		case ["ComboBox","ComboBoxEx32"]
		sel(stl&3) case [CBS_DROPDOWN,CBS_DROPDOWNLIST] if(c.flags&4) r.bottom=r.top; else r.bottom+200
		stl|WS_VSCROLL
		case "ListBox"
		if(stl&LBS_MULTICOLUMN) stl|WS_HSCROLL; else stl|WS_VSCROLL
	
	_Name(c h txt varName)
	sDD+F" {c.cid} {cls} 0x{stl} 0x{c.exstyle} {r.left} {r.top} {r.right-r.left} {r.bottom-r.top} ''{txt.escape(1)}''"
	if(c.tooltip.len) _s=c.tooltip; sDD+F" ''{_s.escape(1)}''"
	sDD+"[]"
	
	if(!&controls1) continue
	
	int add=0
	sel cls 1
		case ["Edit","ListBox","ComboBox","RichEdit20A","RichEdit20W","RichEdit50W"] add=1
		case "Button" sel(stl&BS_TYPEMASK) case [2,3,4,5,6,9] add=1 ;;check, option
		case "Static" sel(stl&SS_TYPEMASK) case [SS_ICON,SS_BITMAP] add=1 ;;icon or bitmap file
		case "ActiveX" if(txt.beg("SHDocVw.WebBrowser")) add=1 ;;open url
		case "QM_Grid" add=1; if(!_qmgridHelp) _qmgridHelp=1; out "<>QM_Grid control:  <help #IDP_QMGRID>help</help>, <open ''sample_Grid''>samples</open>, <help ''ExeQmGridDll''>using in exe</help>."
		case "QM_ComboBox" add=1; if(!_qmcbHelp) _qmcbHelp=1; out "<>QM_ComboBox control:  <help #IDP_QMCOMBOBOX#QM_ComboBox>help</help>, <help ''ExeQmGridDll''>using in exe</help>."
		case "QM_Edit" add=1; if(!_qmeditHelp) _qmeditHelp=1; out "<>QM_Edit control:  <help #IDP_QMCOMBOBOX#QM_Edit>help</help>, <help ''ExeQmGridDll''>using in exe</help>."
		case else if(QmSetWindowClassFlags(cls 0x80000000)&1 or findw(_userClassesVarAdd cls 0 "[]" 1)>=0) add=1 ;;classes added with QmSetWindowClassFlags and in DE Options
	if(add or varids.a.len) sub.AddControlVar(c.cid varName add)

sDD+" END DIALOG[]"


#sub AddControlVar v
function cid $varName !addAlways

if !addAlways
	int i found
	for(i 0 varids.a.len) if(cid=varids.a[i].hwnd) found=1; break
	if(!found) ret

if(controls1.len) controls1+" "
controls1+cid
controls2+" "; controls2+varName
