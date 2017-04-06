 no password
 Database d.Open(d.CsAccess("$personal$\db1.mdb" 0))

 db password
 Database d.Open(d.CsAccess("$personal$\db2.mdb" 1 "p"))
Database d.Open(d.CsAccess("$personal$\db2.mdb" 1 "[*99C55271630471AA07*]"))

 user password
 Database d.Open(d.CsAccess("$personal$\db3.mdb" 1 "" "$personal$\System1.mdw") "Admin" "p2")
 Database d.Open(d.CsAccess("$personal$\db3.mdb" 1 "" "$personal$\System1.mdw") "Admin" "[*A47740B78E45B5EE06*]")

ARRAY(str) a
d.QueryArr("SELECT * FROM Sheet1 WHERE ID=2" a 1)

int r c
for c 0 a.len
	out a[c]
