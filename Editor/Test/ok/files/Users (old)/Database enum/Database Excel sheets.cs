out
Database d
d.Open(d.CsExcel("$personal$\book1.xls"))
ADO.Recordset rs=d.conn.OpenSchema(ADO.adSchemaTables)
 out rs.RecordCount
ARRAY(str) a
d.RsGetAll(rs a)

  all info
 int i j
 for i 0 a.len
	 for j 0 a.len(1)
		 out a[j i]
	 out "--"

 table names (or Excel sheet names)
int i
for i 0 a.len
	out a[2 i]
