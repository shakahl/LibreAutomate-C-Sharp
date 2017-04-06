out
 Sqlite x.Open("$qm$\ok - copy.qml")
Sqlite& x=_qmfile.SqliteBegin
SqliteStatement p.Prepare(x "SELECT ord,name FROM items ORDER BY ord")
rep
	if(!p.FetchRow) break
	byte* b=p.GetBlob(0 &_i)
	_s.outb(b _i); _s+"        "; _s+p.GetText(1)
	out _s

 out x.ExecGetInt("SELECT max(length(ord)) FROM items")
 out x.ExecGetInt("SELECT sum(length(ord))/count(*) FROM items")
 out x.ExecGetInt("SELECT max(length(ord))*sum(length(ord))/count(*) FROM items")
