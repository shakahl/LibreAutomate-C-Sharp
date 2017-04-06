\Dialog_Editor

 Shows how to use grid control with Sqlite database.

str- t_dbfile="$desktop$\test81.db3"

 ________________________________________________

if !FileExists(t_dbfile) ;;create database for testing
	Sqlite dbTest.Open(t_dbfile)
	str sql=
	 CREATE TABLE table1 (A PRIMARY KEY, B,C,D);
	 INSERT INTO table1 VALUES
	 ('a','z','Yes',3),
	 ('z','a',null,1),
	 ('k','k','Yes',8);
	dbTest.Exec(sql)
	dbTest.Close
 ________________________________________________

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 338 135 "Dialog"
 3 QM_Grid 0x54030000 0x200 0 0 338 114 ""
 1 Button 0x54030001 0x4 4 118 48 14 "OK"
 2 Button 0x54030000 0x4 56 118 48 14 "Cancel"
 4 Static 0x54000000 0x0 176 122 160 12 "Click a cell, add text. OK will save the changes."
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "" "" ""

if(!ShowDialog(dd &sub.DlgProc)) ret


#sub DlgProc
function# hDlg message wParam lParam

str- t_dbfile
Sqlite- t_db

DlgGrid g.Init(hDlg 3)
sel message
	case WM_INITDIALOG
	 add columns to the grid control, if not added in dialog editor
	g.ColumnsAdd("A[]B,,8[]C,,2[]D,,1" 1)
	 open database and add table1 to the grid control
	t_db.Open(t_dbfile)
	t_db.ToQmGrid(g "SELECT * FROM table1")
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	 save changes
	t_db.FromQmGrid(g "table1" 2)
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 3
	GRID.QM_NMLVDATA* cd=+nh
	sel nh.code
		case GRID.LVN_QG_COMBOFILL
		TO_CBFill cd.hcb "one[]two[]three"
