Database db.Open(db.CsExcel("$documents$\book1.xls"))
str s; ARRAY(str) a; int c
 note: data starts from row 2. Row 1 must be column names: User, Password.
db.QueryArr("SELECT User FROM [Sheet1$]" a)
for(c 0 a.len) s.addline(a[0 c])

int i=ListDialog(s "" "Accounts")-1; if(i<0) ret
str u p
u.getl(s i)
int j=ShowMenu("4 Paste user[]5 Paste password[]6 Paste user && password[]-[]Cancel"); if(j=0) ret

if j!=4
	db.QueryArr(F"SELECT Password FROM [Sheet1$] WHERE User='{u}'" a 1)
	p=a[0]
 out F"{u} {p}"

 paste u and/or p
act
sel j
	case 4 paste u
	case 5 paste p
	case 6 AutoPassword u p
