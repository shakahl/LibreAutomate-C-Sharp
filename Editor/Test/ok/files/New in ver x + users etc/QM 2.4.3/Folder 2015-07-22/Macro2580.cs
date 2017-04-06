Database d.Open(d.CsExcel("$documents$\Book1.xls"))
ARRAY(str) a; int c
d.QueryArr("SELECT c FROM [Sheet1$] WHERE c IS NOT NULL" a) ;;here c is first row cell text
for c 0 a.len
	out a[0 c]
