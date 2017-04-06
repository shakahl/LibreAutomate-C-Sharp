\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str- t_dbfile="$desktop$\test82.db3"
Sqlite- t_db

 ________________________________________________

 create database for testing
if !dir(t_dbfile)
	Sqlite db1.Open(t_dbfile)
	 db1.Exec("DROP TABLE table1"); err
	db1.Exec("CREATE TABLE IF NOT EXISTS table1 (A,B)")
	db1.Exec("INSERT INTO table1 VALUES ('a','b')")
	db1.Exec("INSERT INTO table1 VALUES ('c','d')")
	db1.Exec("INSERT INTO table1 VALUES ('e','f')")
	db1.Exec("INSERT INTO table1 VALUES ('g','h')")
	db1.Exec("INSERT INTO table1 VALUES ('i','j')")
	db1.Close

str controls = "3"
str qmg3x
if(!ShowDialog("" &Dialog_grid_delete &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 QM_Grid 0x56031041 0x0 0 0 192 112 "0x2,0,0,0,0x0[]A,,,[]B,,,"
 4 Button 0x54032000 0x0 34 118 48 14 "Delete"
 END DIALOG
 DIALOG EDITOR: "" 0x2030304 "" "" ""

ret
 messages
DlgGrid g.Init(hDlg 3)
int i n p
sel message
	case WM_INITDIALOG
	t_db.Open(t_dbfile)
	t_db.ToQmGrid(g "SELECT * FROM table1")
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case 4 ;;Delete
	ARRAY(int) a
	g.RowsSelectedGet(a)
	for i a.len-1 -1 -1
		g.RowDelete(a[i])
	
	case IDOK
	case IDCANCEL
ret 1
 messages3
 OutWinMsg message wParam lParam
NMHDR* nh=+lParam
sel nh.idFrom
	case 3 ;;grid
	GRID.QM_NMLVDATA* cd=+nh
	NMLVDISPINFO* di=+nh
	NMLISTVIEW* nlv=+nh
	sel nh.code
		case GRID.LVN_QG_CHANGE ;;when user changes grid content
		if(cd.hctrl) out "text changed: item=%i, subitem=%i, text=%s, newtext=%s" cd.item cd.subitem cd.txt _s.getwintext(cd.hctrl)
		else
			out "grid changed" ;;eg row deleted
			 n=g.RowsCountGet
			 for i 0 n
				 g.RowPropGet(i p)
				 out p
				
		case LVN_ENDLABELEDIT ;;when ends cell edit mode
		out "end edit: item=%i subitem=%i text=%s" di.item.iItem di.item.iSubItem di.item.pszText
		
		case LVN_INSERTITEM ;;when user inserts new row
		out "inserted %i" nlv.iItem
