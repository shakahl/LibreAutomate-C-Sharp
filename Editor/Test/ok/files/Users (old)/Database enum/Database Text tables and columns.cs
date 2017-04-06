out
Database d
d.Open(d.CsText("$personal$"))
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

 table names (or text/csv file names)
out "TABLES:"
int i
for i 0 a.len
	out a[2 i]

 column names
out "COLUMNS IN %s:" a[2 0]
str sql.format("SELECT TOP 1 * FROM %s" a[2 0])
ARRAY(str) ac
d.QueryArr(sql ac 1)
for i 0 ac.len
	out ac[i]
