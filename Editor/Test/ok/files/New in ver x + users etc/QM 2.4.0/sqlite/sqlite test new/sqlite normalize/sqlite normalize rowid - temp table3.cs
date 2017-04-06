 out
str sf0="$my qm$\test\ok.db3"
str sf="$my qm$\test\ok2.db3"
cop- sf0 sf
 out GetFileFragmentation(sf)
Sqlite x.Open(sf 0 2)
int t0=timeGetTime
PF
x.Exec("PRAGMA foreign_keys=OFF;PRAGMA journal_mode=OFF;BEGIN")
x.Exec("CREATE TEMP TABLE temp_items AS SELECT pid,ord,flags,date,name,trigger,image,link FROM items ORDER BY ord")
x.Exec("DELETE FROM items")
PN
 x.Exec("INSERT INTO items(pid,ord,flags,date,name,trigger,image,link) SELECT * FROM temp_items WHERE rowid=?1")
SqliteStatement pp.Prepare(x "INSERT INTO items(pid,ord,flags,date,name,trigger,image,link) SELECT * FROM temp_items WHERE rowid=?1")
int i n=x.ExecGetInt("SELECT count(*) FROM temp_items")
for i 0 n
	pp.BindInt(1 i)
	pp.Exec; pp.Reset
PN
x.Exec("UPDATE items SET id=0 WHERE id=1")
PN
x.Exec("END")
PN;PO
out "time=%i" timeGetTime-t0
out GetFileFragmentation(sf)
