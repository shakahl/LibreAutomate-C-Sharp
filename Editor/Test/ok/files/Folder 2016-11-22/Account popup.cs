
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Accounts"
 3 ListBox 0x54230101 0x200 4 16 108 114 ""
 4 Button 0x54032000 0x0 128 16 80 14 "User"
 5 Button 0x54032000 0x0 128 36 80 14 "Password"
 6 Button 0x54032000 0x0 128 56 80 14 "User && password"
 8 Static 0x54000000 0x0 4 4 48 10 "Users"
 7 Button 0x54020007 0x0 120 4 96 78 "Paste"
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

str controls = "3"
str lb3

Database db.Open(db.CsExcel("$documents$\book1.xls"))
ARRAY(str) a; int c
 note: data starts from row 2. Row 1 must be column names: User, Password.
db.QueryArr("SELECT User FROM [Sheet1$]" a)
for(c 0 a.len) lb3.addline(a[0 c])

if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc v
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case [4,5,6]
	str u p
	int i=LB_SelectedItem(id(3 hDlg) u)
	if(i<0) ret
	if wParam!=4
		db.QueryArr(F"SELECT Password FROM [Sheet1$] WHERE User='{u}'" a 1)
		p=a[0]
	 out F"{u} {p}"
	 paste u and/or p
	act
	sel wParam
		case 4 paste u
		case 5 paste p
		case 6 AutoPassword u p
ret 1
