Database d.Open(d.CsText("$personal$"))
ADO.Recordset rs
d.QueryRs("SELECT * FROM Sheet1.txt" rs)

int r c
for c 0 rs.Fields.Count
	out rs.Fields.Item(c).Value
