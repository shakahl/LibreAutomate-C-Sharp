\Dialog_Editor

str databaseFile="$my qm$\test5725w.db3" ;;change this
int createTableNow=1 ;;change 1 to 0 if don't want to create table everytime here

Sqlite- db
db.Open(databaseFile 0 4)

 create an example table and add several items
if createTableNow
	str sql=
	 PRAGMA journal_mode=WAL;
	 BEGIN;
	 DROP TABLE IF EXISTS t;
	 CREATE TABLE t(name,tag);
	 INSERT INTO t VALUES
	 ('January','month'),
	 ('Apple','fruit'),
	 ('Banana','fruit'),
	 ('April','month'),
	 ('Other','');
	 COMMIT
	db.Exec(sql)
 Edit the above sql string.
 To add/remove/edit items later, you can anywhere use code like this (except the DROP TABLE and CREATE TABLE lines).
   Or use a database management program, eg SQLite Expert Personal.
   Then set createTableNow=0 or remove the creation code.

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 3 Edit 0x54030080 0x200 0 0 96 14 ""
 4 ListBox 0x54230101 0x200 0 16 96 102 ""
 5 Button 0x54032000 0x0 112 76 110 34 "Select an item in the list and click me; or double click an item"
 6 Static 0x54000000 0x0 104 0 18 12 "Tag"
 7 ComboBox 0x54230242 0x0 124 0 96 213 ""
 8 Button 0x54012003 0x0 124 16 48 10 "Sort"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040201 "*" "" "" ""

str controls = "3 4 7 8"
str e3 lb4 cb7 c8Sor
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

Sqlite- db
int hlb hcb i
str sql sName sTag
ARRAY(str) a

sel message
	case WM_INITDIALOG
	SetProp id(3 hDlg) "wndproc" SubclassWindow(id(3 hDlg) &sub.EditSubclassProc) ;;optional, just to select listbox items with arrow keys when the control is not focused
	
	 populate the Tag combo
	hcb=id(7 hDlg)
	CB_Add(hcb "")
	db.Exec("SELECT DISTINCT tag FROM t" a)
	for(i 0 a.len) CB_Add(hcb a[0 i])
	
	SetTimer hDlg 1 10 0
	
	case WM_DESTROY
	RemoveProp id(3 hDlg) "wndproc"
	
	case WM_COMMAND goto messages2
	
	case WM_TIMER
	sel wParam
		case 1 KillTimer hDlg wParam; goto gUpdateList
ret
 messages2
sel wParam
	case [EN_CHANGE<<16|3, CBN_EDITCHANGE<<16|7]
	SetTimer hDlg 1 300 0
	case [CBN_SELCHANGE<<16|7, 8]
	SetTimer hDlg 1 10 0
	
	case LBN_DBLCLK<<16|4 goto gShowSelected
	case 5 goto gShowSelected
	
	case IDOK
	case IDCANCEL
ret 1

 gUpdateList
hlb=id(4 hDlg)
SendMessage hlb LB_RESETCONTENT 0 0
sName.getwintext(id(3 hDlg)); sName.SqlEscape
sTag.getwintext(id(7 hDlg)); sTag.SqlEscape
 get items from database
sql="SELECT name FROM t"
if(sName.len) sql+F" WHERE name LIKE '%{sName}%'"; if(sTag.len) sql+F" AND tag='{sTag}'"
else if(sTag.len) sql+F" WHERE tag='{sTag}'"
if(but(8 hDlg)) sql+" ORDER BY name"
db.Exec(sql a)
for i 0 a.len
	LB_Add hlb a[0 i]

ret

 gShowSelected
hlb=id(4 hDlg)
_i=LB_SelectedItem(hlb); if(_i<0) ret
LB_GetItemText hlb _i _s
mes _s
ret


#sub EditSubclassProc
function# hWnd message wParam lParam

 OutWinMsg message wParam lParam
sel message
	case WM_DESTROY
	case [WM_KEYDOWN,WM_KEYUP]
	sel wParam ;;virtual key code
		case [VK_DOWN,VK_UP,VK_PRIOR,VK_NEXT]
		 relay these keys to the listbox and not to the edit box
		SendMessage id(4 GetParent(hWnd)) message wParam lParam
		ret

int wndproc=GetProp(hWnd "wndproc"); if(!wndproc) ret
ret CallWindowProcW(wndproc hWnd message wParam lParam)
