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

str s="INSERT  INTO items VALUES (?1, ?2, ?3, ?4, ?5, ?6, ?7, ?8, ?9, ?10)"
if(replace) s.set("REPLACE")
 out s

SqliteStatement p.Prepare(x s)

rep
	i=qmitem(-i 2 &q -1~64); if(!i) break
	n+1
	
	 if(__sqlite.sqlite3_bind_int(p.p 1 i)) end "error"
	 if(__sqlite.sqlite3_bind_text(p.p 10 q.linktarget -1 0)) end "error"
	
	p.BindInt(1 i)
	p.BindInt(2 q.folderid)
	p.BindInt(3 q.itype)
	p.BindDouble(4 q.datemod.date)
	p.BindText(5 q.name)
	p.BindText(6 q.text)
	 p.BindBlob(6 q.text q.text.len)
	p.BindText(7 q.trigger)
	p.BindText(8 q.programs)
	p.BindText(9 q.filter)
	p.BindText(10 q.linktarget)
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
 