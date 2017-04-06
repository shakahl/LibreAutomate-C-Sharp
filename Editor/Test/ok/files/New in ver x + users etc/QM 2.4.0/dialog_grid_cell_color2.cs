\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 2-dim array for colors of all rows and cells: {row0 cell(0 0) cell(0 1)},{row1 cell(1 0) cell(1 1)}...
ARRAY(int)- a

str controls = "3"
str qmg3x
if(!ShowDialog("" &dialog_grid_cell_color2 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_Grid 0x56031041 0x0 0 0 224 114 "0x2,0,0,0,0x0[]Computer name,,,[]IP,,,"
 4 Button 0x54032000 0x0 68 116 48 14 "SAVE"
 5 Button 0x54032000 0x0 2 116 48 14 "Set color"
 END DIALOG
 DIALOG EDITOR: "" 0x2030605 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG goto gInit
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto gNotify
ret
 messages2
sel wParam
	case 4 ;;Save
	memset &a[0 0] 0 a.len*a.len(1)*sizeof(int) ;;set all elements 0
	InvalidateRect id(3 hDlg) 0 1 ;;redraw grid
	
	case 5 ;;Set color
	int redrawRow=2
	a[0 redrawRow]=0x8080 ;;row color
	a[2 redrawRow]=0x80 ;;cell color
	SendMessage id(3 hDlg) LVM_REDRAWITEMS redrawRow redrawRow
	
	case IDOK
	case IDCANCEL
ret 1
 _______________________

 gInit

   ======= BEGIN: INITIAL FONT SETTINGS
__Font-- f fBold
f.Create("Courier New" 10 0)
fBold.Create("" 0 1 0 f 4)
f.SetDialogFont(hDlg "3")

int- defColor=0xff0000 ;;default color

int hlv=id(3 hDlg)
SendMessage hlv LVM_SETTEXTCOLOR 0 defColor
SendMessage hlv CCM_SETVERSION 5 0
   ======= END: INITIAL FONT SETTINGS

str csv=
 test1,192.168.0.1
 tes21,192.168.0.2
 test3,192.168.0.3

ICsv- t_csv=CreateCsv(1)
t_csv.FromString(csv)
t_csv.ToQmGrid(id(3 hDlg))

a.create(t_csv.ColumnCount+1 t_csv.RowCount)

ret
 _______________________

 gNotify
NMHDR* nh=+lParam
DlgGrid g.Init(nh.hwndFrom)
sel nh.idFrom
	case 3
	GRID.QM_NMLVDATA* d=+nh
	NMLVDISPINFO* di=+nh
	NMLISTVIEW* nlv=+nh
	NMITEMACTIVATE* na=+nh
	sel nh.code
		case GRID.LVN_QG_CHANGE ;;when user changes grid content
		if(d.hctrl)
			out "text changed: item=%i, subitem=%i, text=%s, newtext=%s" d.item d.subitem d.txt _s.getwintext(d.hctrl)
			if(d.item>=a.len) a.redim(d.item+1) ;;if more rows added to grid, add to a too
			a[0 d.item]=0x8000 ;;set changed row color
			a[d.subitem+1 d.item]=0xff ;;set changed cell color
		else ;;eg row deleted
			out "grid changed"
			_i=g.RowsCountGet; if(_i!=a.len) a.redim(_i)
		
		case LVN_ENDLABELEDIT ;;when ends cell edit mode
		out "end edit: item=%i subitem=%i text=%s" di.item.iItem di.item.iSubItem di.item.pszText
		if di.item.iItem
			g.CellSet(di.item.iItem 0 "*") ;;change first cell in the row to "*"
		
		case NM_CUSTOMDRAW goto gCustomDraw
ret
 _______________________

 gCustomDraw
NMCUSTOMDRAW* cd=+lParam
NMLVCUSTOMDRAW* cd2=+lParam
int R ;;the return value
int x(cd2.iSubItem+1) y(cd.dwItemSpec)
sel cd.dwDrawStage
	case CDDS_PREPAINT
	R=CDRF_NOTIFYITEMDRAW ;;notify to draw items
	
	case CDDS_ITEMPREPAINT ;;now draw item
	if y<a.len and a[0 y] ;;if this row changed
		cd2.clrText=a[0 y]
		SelectObject cd.hdc fBold
	else
		cd2.clrText=defColor
		SelectObject cd.hdc f
	R=CDRF_NEWFONT|CDRF_NOTIFYSUBITEMDRAW ;;change font and notify to draw subitems
		
	case CDDS_ITEMPREPAINT|CDDS_SUBITEM ;;now draw subitem
	if(y<a.len and a[x y]) cd2.clrText=a[x y] ;;if this cell changed
	else if(a[0 y]) cd2.clrText=a[0 y] ;;color of row
	else cd2.clrText=defColor
	 R=CDRF_NEWFONT

ret DT_Ret(hDlg R)
