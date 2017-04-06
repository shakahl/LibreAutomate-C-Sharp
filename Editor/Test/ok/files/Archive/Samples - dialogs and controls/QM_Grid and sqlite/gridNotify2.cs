 /dlg_QM_Grid_Sqlite
function NMHDR*nh

ref GRID
Sqlite- t_db

int hgrid=nh.hwndFrom
NMLVDISPINFO* di=+nh
QM_NMLVDATA* cd=+nh
NMLISTVIEW* lv=+nh
sel nh.code
	case LVN_QG_CHANGE ;;the message is sent when the user changes something in the grid
	
	case LVN_COLUMNCLICK
	SetFocus 0; SetFocus hgrid ;;turn off cell editing mode
	_s=lv.iSubItem+1
	t_db.SortQmGrid(hgrid "table1" _s 2)
