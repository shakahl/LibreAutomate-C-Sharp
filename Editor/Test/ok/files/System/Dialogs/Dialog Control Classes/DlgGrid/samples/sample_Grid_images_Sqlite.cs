\Dialog_Editor

 Shows how to use icons and Sqlite with QM grid control.
 Columns and styles are defined in dialog editor.
 More info in QM Help.

str- t_dbfile="$desktop$\test84.db3"

 ________________________________________________

if !FileExists(t_dbfile) ;;create database for testing
	Sqlite dbTest.Open(t_dbfile)
	str sql=
	 CREATE TABLE table1 (A PRIMARY KEY, B,C);
	 INSERT INTO table1 VALUES
	 ('<//2>a1','b1','c1'),
	 ('<//4>a2','b2','c2');
	dbTest.Exec(sql)
	dbTest.Close
 ________________________________________________

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 206 159 "QM_Grid"
 3 QM_Grid 0x56831041 0x200 0 0 206 134 "0x7,0,0,0[]A,50%,,[]B,20%,,[]C,20%,1,[]"
 1 Button 0x54030001 0x4 2 142 48 14 "OK"
 2 Button 0x54030000 0x4 52 142 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040100 "*" "" "" ""

str controls = "3"
str qmg3x
if(!ShowDialog(dd &sub.DlgProc &controls _hwndqm)) ret
out qmg3x


#sub DlgProc
function# hDlg message wParam lParam

str- t_dbfile
Sqlite- t_db

DlgGrid g.Init(hDlg 3)
sel message
	case WM_INITDIALOG
	 set imagelist for the grid control
	__ImageList- il.Load("$qm$\il_dlg.bmp")
	g.SetImagelist(il)
	
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
	t_db.FromQmGrid(g "table1")
ret 1

 messages3
 NMHDR* nh=+lParam
 sel nh.idFrom
	 case 3
	 sel nh.code
		 case LVN_ITEMCHANGED
