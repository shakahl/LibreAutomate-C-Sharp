 out
str sf0="$my qm$\test\ok.db3"
str sf="$my qm$\test\ok2.db3"
cop- sf0 sf
 out GetFileFragmentation(sf)
Sqlite x.Open(sf 0 2)
int t0=timeGetTime
PF
 x.Exec("PRAGMA foreign_keys=ON;BEGIN")
x.Exec("PRAGMA foreign_keys=OFF;PRAGMA journal_mode=OFF;BEGIN")
x.Exec("UPDATE items SET id=-id")
PN
x.Exec("UPDATE texts SET id=-id")
PN
ARRAY(int) a.create(x.ExecGetInt("SELECT count(*) FROM items"))
SqliteStatement p.Prepare(x "SELECT id FROM items ORDER BY ord")
int i
rep
	if(!p.FetchRow) break
	a[i]=__sqlite.sqlite3_column_int(p.p 0)
	i+1
PN
SqliteStatement pp
pp.Prepare(x "UPDATE items SET id=?1,ord=?3 WHERE id=?2")
for i 1 a.len
	pp.BindInt(1 i)
	pp.BindInt(2 a[i])
	pp.BindInt(3 i*1000)
	pp.Exec
	pp.Reset
PN
x.Exec("CREATE INDEX items_pid ON items(pid)")
pp.Prepare(x "UPDATE items SET pid=?1 WHERE pid=?2")
for i 1 a.len
	pp.BindInt(1 i)
	pp.BindInt(2 a[i])
	pp.Exec
	pp.Reset
x.Exec("DROP INDEX items_pid")
PN
pp.Prepare(x "UPDATE texts SET id=?1 WHERE id=?2")
for i 1 a.len
	pp.BindInt(1 i)
	pp.BindInt(2 a[i])
	pp.Exec
	pp.Reset
PN
x.Exec("END")
PN;PO
out "time=%i" timeGetTime-t0
out GetFileFragmentation(sf)
