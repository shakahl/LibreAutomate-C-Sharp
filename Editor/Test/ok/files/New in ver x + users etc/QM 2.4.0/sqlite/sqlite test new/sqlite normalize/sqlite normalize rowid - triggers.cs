 out
str sf0="$my qm$\test\ok.db3"
str sf="$my qm$\test\ok2.db3"
cop- sf0 sf
 out GetFileFragmentation(sf)
Sqlite x.Open(sf 0 2)
int t0=timeGetTime
PF
x.Exec("PRAGMA foreign_keys=OFF;PRAGMA journal_mode=OFF;BEGIN")
 x.Exec("CREATE TEMP TRIGGER trigger_id AFTER UPDATE OF id ON items BEGIN UPDATE texts SET id=new.id WHERE id=old.id; END")
x.Exec("CREATE TEMP TRIGGER trigger_id AFTER UPDATE OF id ON items BEGIN UPDATE texts SET id=new.id WHERE id=old.id; UPDATE items SET pid=new.id WHERE pid=old.id; END") ;;slooooooooooooow when trigger on pid
PN
x.Exec("UPDATE items SET id=-id")
PN
ARRAY(int) a.create(x.ExecGetInt("SELECT count(*) FROM items"))
SqliteStatement p.Prepare(x "SELECT id FROM items ORDER BY ord")
int i
rep
	if(!p.FetchRow) break
	a[i]=__sqlite.sqlite3_column_int(p.p 0)
	i+1
PN
SqliteStatement pp.Prepare(x "UPDATE items SET id=?1,ord=?3 WHERE id=?2")
for i 1 a.len
	pp.BindInt(1 i)
	pp.BindInt(2 a[i])
	pp.BindInt(3 i*1000)
	pp.Exec
	pp.Reset
PN
x.Exec("END")
PN;PO
out "time=%i" timeGetTime-t0
out GetFileFragmentation(sf)

 info:
 Foreign key on pid makes very slow, don't know why. Creating index on pid does not help.
 Foreign key on text makes slower just 2 times.
