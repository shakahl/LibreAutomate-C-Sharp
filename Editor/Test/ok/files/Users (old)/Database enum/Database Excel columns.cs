out
Database d
d.Open(d.CsExcel("$personal$\book1.xls"))
ADO.Recordset rs=d.conn.OpenSchema(ADO.adSchemaColumns)
ARRAY(str) a
d.RsGetAll(rs a)

  all info
 int i j
 for i 0 a.len
	 for j 0 a.len(1)
		 out a[j i]
	 out "--"

 table names (or sheet names), column positions and column names
int i
for i 0 a.len
	out "%s %i %s" a[2 i] val(a[6 i]) a[3 i]
