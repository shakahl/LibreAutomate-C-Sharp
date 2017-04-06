out
int t1 t2
int replace
Sqlite x.Open("$desktop$\qml.db3")
x.Exec("BEGIN TRANSACTION")
x.Exec("DROP TABLE items"); err
x.Exec("CREATE TABLE items (id INTEGER PRIMARY KEY, folderid INTEGER, flags INTEGER, datemod REAL, name TEXT, text TEXT, trigger TEXT, programs TEXT, ff TEXT, linktarget TEXT)")
err replace=1

QMITEM q; int i n
t1=timeGetTime

str s="INSERT  INTO items VALUES ($a, $b, $c, $d, $e, $f, $g, $h, $i, $j)"
if(replace) s.set("REPLACE")
 out s

SqliteStatement p.Prepare(x s)

rep
	i=qmitem(-i 2 &q -1~64); if(!i) break
	n+1
	
	 if(__sqlite.sqlite3_bind_int(p.p 1 i)) end "error"
	 if(__sqlite.sqlite3_bind_text(p.p 10 q.linktarget -1 0)) end "error"
	
	p.BindInt(+"$a" i)
	p.BindInt(+"$b" q.folderid)
	p.BindInt(+"$c" q.itype)
	p.BindDouble(+"$d" q.datemod.date)
	p.BindText(+"$e" q.name)
	p.BindText(+"$f" q.text)
	 p.BindBlob(6 q.text q.text.len)
	p.BindText(+"$g" q.trigger)
	p.BindText(+"$h" q.programs)
	p.BindText(+"$i" q.filter)
	p.BindText(+"$j" q.linktarget)
	 p.BindNull(10)
	
	p.Exec
	p.Reset

x.Exec("END TRANSACTION")
t2=timeGetTime
out t2-t1
 out n

 Times:
 Now QM saves file: 155 ms.
 This code when INSERT: 470 ms.
 This code when REPLACE: 500 ms.
 