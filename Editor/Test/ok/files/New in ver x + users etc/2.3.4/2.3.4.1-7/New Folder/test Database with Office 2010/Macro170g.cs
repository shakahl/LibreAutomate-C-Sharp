out
Database db.Open(db.CsAccess("$personal$\db1.mdb"))
 Database db.Open(db.CsAccess("$personal$\db1.mdb" 1))
ARRAY(str) a; int r c
db.QueryArr("SELECT * FROM Table1" a)
for r 0 a.len(2)
	out "-- Record %i --" r+1
	for c 0 a.len(1)
		out a[c r]
