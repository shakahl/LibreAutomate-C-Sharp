\Dialog_Editor
function# hDlg message wParam lParam

 Shows how to use grid control with Sqlite database.

if(hDlg) goto messages

str- t_dbfile="$desktop$\test82.db3"
Sqlite- t_db

 ________________________________________________

 create database for testing
Sqlite db1.Open(t_dbfile)
db1.Exec("CREATE TABLE IF NOT EXISTS table1 (A)")
str name="O'Connor"
name.SqlEscape
db1.Exec(F"INSERT INTO table1 VALUES ('{name}')")
db1.Close
 g1
 ________________________________________________


if(!ShowDialog("" &sample_Grid_Sqlite2)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 338 135 "Dialog"
 3 QM_Grid 0x54030000 0x0 0 0 338 114 "0x0,0,0,0,0x0[]A,,,"
 1 Button 0x54030001 0x4 4 118 48 14 "OK"
 2 Button 0x54030000 0x4 56 118 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "" "" ""

ret
 messages
DlgGrid g.Init(hDlg 3)
sel message
	case WM_INITDIALOG
	 open database and add table1 to the grid control
	t_db.Open(t_dbfile)
	t_db.ToQmGrid(g "SELECT * FROM table1")
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
ret 1
