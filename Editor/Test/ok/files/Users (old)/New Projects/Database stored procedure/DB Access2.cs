ClearOutput
Database db
db.OpenAccess("$personal$\db1.mdb")

ARRAY(str) a
ADO.Command co._create
co.ActiveConnection=db.conn
co.CommandText="Proc1"
co.CommandType=ADO.adCmdStoredProc
co.CreateParameter

 db.QueryArr("PROCEDURE Proc3(*)" a)
int r c
for r 0 a.len(2)
	out "-- Row %i --" r+1
	for c 0 a.len(1)
		out a[c r]
