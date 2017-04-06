out

Sqlite& x=_qmfile.SqliteBegin
 Sqlite& x=_qmfile.SqliteBegin(qmitem("TestR3"))
ARRAY(str) a; int i
x.Exec(F"SELECT id,name FROM resources" a)
for(i 0 a.len) out "%s %s" a[0 i] a[1 i]
_qmfile.SqliteEnd
