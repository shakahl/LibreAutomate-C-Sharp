 /Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 292 210 "Grid properties"
 3 Static 0x54000001 0x0 4 4 8 54 "S[]t[]y[]l[]e"
 4 QM_Grid 0x56035041 0x200 14 4 128 77 "0x37,0,0,4,0x8000[],,,"
 5 Static 0x54000001 0x0 4 104 8 76 "C[]o[]l[]u[]m[]n[]s"
 6 QM_Grid 0x56030000 0x200 14 88 276 92 "0,1,0,2[]Text,40%,[]Width,15%,1[]Control,27%,1[]Flags,0[]+button,15%,2"
 7 Button 0x54012003 0x0 156 46 126 9 "Skip empty rows"
 11 Button 0x54012003 0x0 156 56 126 10 "Get only selected or checked"
 13 Button 0x54012003 0x0 156 66 126 10 "Remove <row type>"
 10 Button 0x54032000 0x0 156 4 58 14 "More styles..."
 12 Edit 0x44030080 0x200 218 4 70 14 "exs"
 1 Button 0x54030001 0x4 4 192 48 14 "OK"
 2 Button 0x54030000 0x4 56 192 48 14 "Cancel"
 9 Button 0x54032000 0x0 108 192 16 14 "?"
 8 Button 0x54020007 0x0 150 34 138 47 "When getting content"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

str controls = "4 6 7 11 13 12"
str qmg4x qmg6 c7Ski c11Get c13Rem e12exs

DlgGrid g.Init(_hsel)
___DE_CONTROL& co=subs.GetControl(g)
ICsv c._create
str s1
int i v style stmask f1 f2 f3

 options
s1.getl(co.txt 0)
c.FromString(s1)
for i 0 c.ColumnCount
	v=c.CellInt(0 i)
	sel(i) case 0 style=v; case 1 f1=v; case 2 f2=v; case 3 f3=v~(2|4|8); TO_FlagsToCheckboxes(v 0 c7Ski c11Get c13Rem); case 4 e12exs=v
stmask=sub_to.FlagsToGridCsv(style "1,User cannot edit first column[]2,User cannot add/remove rows[]4,Can be set row control type[]8,First column - row numbers[]16,User cannot edit cells[]32,With check boxes[]64,Need 2 clicks to edit[]128,User can drag and drop rows[]0x100,Manage tree indentations when editing" qmg4x)

 columns
__MapIntStr m.AddList("0 edit[]8 edit multiline[]1 combo[]9 combo sorted[]2 check[]3 date[]11 time[]7 none (read-only)")
s1.getl(co.txt 1 2)
c.FromString(s1)
c.ColumnCount=5
for(i 0 c.RowCount)
	v=c.CellInt(i 2)
	c.Cell(i 4)=iif(v&16 "Yes" "")
	c.Cell(i 2)=m.MapIntStr(v&15)
c.ToString(qmg6)

if(!ShowDialog(dd &sub.DlgProcGrid &controls _hwnd 0 0 0 &m)) ret

 options
sub_to.FlagsFromGridCsv(qmg4x style stmask)
f3|TO_FlagsFromCheckboxes(0 c7Ski 2 c11Get 4 c13Rem 8)

 columns
c.FromString(qmg6)
for i 0 c.RowCount
	v=m.MapStrInt(c.Cell(i 2))
	if(!StrCompare(c.Cell(i 4) "Yes" 1)) v|16
	if(v) s1=v; else s1=""
	c.Cell(i 2)=s1
c.ColumnCount=4
c.ToString(qmg6); qmg6.rtrim("[]")

s1=F"0x{style},{f1},{f2},0x{f3},0x{val(e12exs)}[]{qmg6}"
 out s1

g.ColumnsAdd(s1 GRID.QG_AC_SETOPT)
_Undo
co.txt=s1
_Select(g)


#sub DlgProcGrid
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	DlgGrid g.Init(hDlg 4)
	g.ColumnsWidthAdjust
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case 10
	_i=val(_s.getwintext(id(12 hDlg)))
	_s=
	 0x8000,More space to click check box
	 8,LVS_EX_TRACKSELECT
	 0x10,LVS_EX_HEADERDRAGDROP
	 0x40,LVS_EX_ONECLICKACTIVATE
	 0x80,LVS_EX_TWOCLICKACTIVATE
	 0x840,LVS_EX_UNDERLINEHOT
	 0x1040,LVS_EX_UNDERLINECOLD
	 0x10000000,LVS_EX_AUTOSIZECOLUMNS (Vista/7/8/10)
	if(!sub_to.FlagsDialog(_i _s hDlg "List view ex styles")) ret
	_s=_i; _s.setwintext(id(12 hDlg))
	
	case 9
	QmHelp "IDP_QMGRID"
ret 1
 messages3
NMHDR* nh=+lParam
int i
sel nh.idFrom
	case 6
	GRID.QM_NMLVDATA* cd=+nh
	sel nh.code
		case GRID.LVN_QG_COMBOFILL
		sel cd.subitem
			case 1 TO_CBFill cd.hcb "[]10%[]20%[]30%[]40%[]50%[]60%[]70%[]80%[]90%[]50[]75[]100[]150[]200[]250[]300"
			case 2 __MapIntStr& m=+DT_GetParam(hDlg); for(i 0 m.a.len) CB_Add(cd.hcb m.a[i].s)
		case GRID.LVN_QG_CHANGE
		sel cd.subitem
			case 1 ;;allow only number and number%
			if(!cd.hctrl) ret
			_s.getwintext(cd.hctrl)
			if(_s.len and findrx(_s "^\d+\.?\d*%?$")<0) _s.setwintext(cd.hctrl)
