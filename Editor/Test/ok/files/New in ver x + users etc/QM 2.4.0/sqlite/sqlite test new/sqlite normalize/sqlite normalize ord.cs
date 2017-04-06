out
str sf="$my qm$\test\ok.db3"
if(!ConvertQmlToSqlite("$qm$\ok.qml" sf 1024*0)) out "FAILED"
Sqlite x.Open(sf 0 2)
PF
ARRAY(int) a.create(x.ExecGetInt("SELECT count(*) FROM items"))
SqliteStatement p.Prepare(x "SELECT id FROM items ORDER BY ord")
int i
rep
	if(!p.FetchRow) break
	a[i]=__sqlite.sqlite3_column_int(p.p 0)
	i+1
PN
x.Exec("BEGIN")
SqliteStatement pp.Prepare(x "UPDATE items SET ord=?1 WHERE id=?2")
for i 0 a.len
	pp.BindInt(1 i*1000)
	pp.BindInt(2 a[i])
	pp.Exec
	pp.Reset
x.Exec("END")
PN;PO
out i
