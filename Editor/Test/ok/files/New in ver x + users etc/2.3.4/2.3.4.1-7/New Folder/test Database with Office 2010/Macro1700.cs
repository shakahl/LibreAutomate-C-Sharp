out
Database db2.Open(db2.CsExcel("$personal$\book1.xls"))
 Database db2.Open(db2.CsExcel("$personal$\book1.xlsx"))
ARRAY(str) a2; int r2
db2.QueryArr("SELECT * FROM [Sheet1$]" a2)
for r2 0 a2.len
	out a2[0 r2]
