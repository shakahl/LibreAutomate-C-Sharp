\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str- t_dbfile="$desktop$\test80.db3"

if !dir(t_dbfile) ;;create database for testing
	Sqlite dbTest.Open(t_dbfile)
	str sql=
	 CREATE TABLE table1 (A PRIMARY KEY, B,C,D);
	 INSERT INTO table1 VALUES
	 ('a','z','Yes',3),
	 ('z','a',null,1),
	 ('k','k','Yes',8);
	dbTest.Exec(sql)
	dbTest.Close

str controls = "8 10"
str cb8 e10
cb8="A[]B[]C[]D"
if(!ShowDialog("" &dlg_QM_Grid_Sqlite &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 231 200 "QM_Grid"
 3 QM_Grid 0x54210009 0x200 0 0 232 140 ""
 12 Button 0x54032000 0x0 2 142 52 14 "Select 2 rows"
 5 Button 0x54032000 0x0 56 142 52 14 "Get Selected"
 13 Button 0x54032000 0x0 182 142 48 14 "Focus grid"
 6 Button 0x54032000 0x0 2 158 52 14 "Find rows"
 7 Static 0x54000000 0x0 56 160 54 12 "where in column"
 8 ComboBox 0x54230243 0x0 114 160 24 213 ""
 9 Static 0x54000000 0x0 140 160 20 12 "cell is"
 10 Edit 0x54030080 0x200 162 158 68 14 ""
 1 Button 0x54030001 0x0 82 184 48 14 "OK"
 4 Button 0x54032000 0x0 132 184 48 14 "Apply"
 2 Button 0x54030000 0x0 182 184 48 14 "Cancel"
 14 Button 0x54032000 0x0 122 142 48 14 "Reload"
 11 Static 0x54000010 0x20000 2 178 220 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "" "" ""

ret
 messages
ref GRID
Sqlite- t_db
DlgGrid g.Init(hDlg 3)
sel message
	case WM_INITDIALOG
	t_db.Open(t_dbfile)
	
	g.ColumnsAdd("A[]B,,8[]C,,2[]D,,1" 1)
	t_db.ToQmGrid(g "SELECT * FROM table1")
	 
	 g.GridStyleSet(9)
	  g.GridStyleSet(1)
	 g.ColumnsAdd("#,20[]A[]B,,8[]C,,2[]D,,1")
	 t_db.ToQmGrid(g "SELECT * FROM table1" 1)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
str ss
int r
ARRAY(str) as
err-
g.Init(hDlg 3)
sel wParam
	case [IDOK,4]
	t_db.FromQmGrid(g "table1" 2)
	
	case 14 ;;Reload
	t_db.ToQmGrid(g "SELECT * FROM table1")
	 t_db.ToQmGrid(g "SELECT * FROM table1" 1)
	 t_db.ToQmGrid(g "SELECT C,D FROM table1")
	 t_db.ToQmGrid(g "SELECT C,D FROM table1" 1)
	 t_db.ToQmGrid(g "SELECT * FROM table1 WHERE A='v'")
	
	case 13 ;;Focus grid
	SetFocus g
	
	case 12 ;;Select 2 rows
	SetFocus g
	g.RowSelect(0 2)
	g.RowSelect(1 1)
	
	case 5 ;;Get selected
	g.ToCsv(ss "," 4)
	ShowText "" ss
	
	case 6 ;;Find rows
	str s1.getwintext(id(8 hDlg)) s2.getwintext(id(10 hDlg))
	if(!s1.len) mes "Select a column" "" "i"; ret
	t_db.FromQmGrid(g "temp" 0 "table1") ;;get results into temporary table
	t_db.Exec(F"SELECT * FROM temp WHERE {s1}='{s2}' COLLATE NOCASE" as)
	
	ICsv csv._create
	for(r 0 as.len) csv.AddRowSA(r as.len(1) &as[0 r])
	csv.ToString(ss)
	ShowText "" ss
	
err+ mes _error.description; ret
ret 1
 messages3
NMHDR* nh=+lParam
if(nh.idFrom=3) ret DT_Ret(hDlg gridNotify2(nh))
