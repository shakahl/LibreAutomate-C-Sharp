Database d; ARRAY(str) a2; int c2
d.Open(d.CsExcel("$personal$\book1.xls"))
d.QueryArr("SELECT * FROM [Sheet1$]" a2 1)

for c2 0 a2.len
	out a2[c2]
