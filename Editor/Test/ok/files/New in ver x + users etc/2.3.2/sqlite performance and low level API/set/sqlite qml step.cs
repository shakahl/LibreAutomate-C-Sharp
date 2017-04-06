out
int t1 t2
int replace
Sqlite x.Open("$desktop$\qml.db3")
x.Exec("BEGIN TRANSACTION")
x.Exec("DROP TABLE items"); err
x.Exec("CREATE TABLE items (id INTEGER, folderid INTEGER, flags INTEGER, datemod REAL, name TEXT, text TEXT, trigger TEXT, programs TEXT, ff TEXT, linktarget TEXT)")
err replace=1

QMITEM q; int i n
t1=timeGetTime
rep
	i=qmitem(-i 2 &q -1~64); if(!i) break
	n+1
	str s.format("INSERT  INTO items VALUES (%i, %i, %i, %g, '%s', '%s', '%s', '%s', '%s', '%s')" i q.folderid q.itype q.datemod.date q.name.SqlEscape q.text.SqlEscape q.trigger.SqlEscape q.programs.SqlEscape q.filter.SqlEscape q.linktarget.SqlEscape)
	if(replace) s.set("REPLACE")
	 out s
	
	SqliteStatement p.Prepare(x s)
	p.Exec
	
x.Exec("END TRANSACTION")
t2=timeGetTime
out t2-t1
 out n

 Times:
 Now QM saves file: 155 ms.
 This code when INSERT: 810 ms (170 s.format, other step etc).
 This code when REPLACE: 650 ms.
 