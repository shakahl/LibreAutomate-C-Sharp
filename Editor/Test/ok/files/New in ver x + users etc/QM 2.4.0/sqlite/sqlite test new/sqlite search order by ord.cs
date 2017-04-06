out
str sf="$my qm$\test\ok.db3"
Sqlite x.Open(sf 0 2)
int i j
 ARRAY(str) a
PF
 x.Exec("SELECT id FROM items" a)
 x.Exec("SELECT id FROM items ORDER BY ord" a)
 x.Exec("SELECT * FROM items" a)
 x.Exec("SELECT * FROM items ORDER BY ord" a)
 str sql="SELECT id FROM items"
 str sql="SELECT id FROM items ORDER BY ord"
 str sql="SELECT * FROM items"
str sql="SELECT * FROM items ORDER BY ord"
SqliteStatement p.Prepare(x sql)
rep
	if(!p.FetchRow) break
	 p.GetInt(0)
	__sqlite.sqlite3_column_int(p.p 0)
PN;PO

 out a.len
 ret
 for i 0 a.len
	 _s.getl(a[0 i] 0)
	 out _s
	 