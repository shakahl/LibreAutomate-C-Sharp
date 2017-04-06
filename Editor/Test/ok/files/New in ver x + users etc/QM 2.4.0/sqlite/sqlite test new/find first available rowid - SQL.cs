out
int w=win("SQLite2009 Pro - The Best GUI to manage your SQLite3 databases." "ThunderRT6FormDC"); if(w) clo w
str sf="$my qm$\test\ok.db3"
if(!ConvertQmlToSqlite("$qm$\ok.qml" sf 1024*0)) out "FAILED"
Sqlite y.Open(sf 0 2)
 y.Exec("DELETE FROM items WHERE id=11000")
 y.Exec("DELETE FROM items WHERE id>1000 AND id<11000")
y.Exec("DELETE FROM items WHERE id=10")
y.Close

PF
Sqlite x.Open(sf 0 2)

str sql
sql=
 SELECT  MIN(id) + 1
 FROM    items mo
 WHERE   NOT EXISTS
        (
        SELECT  NULL
        FROM    items mi
        WHERE   mi.id = mo.id + 1
        )

 sql=
 SELECT  id + 1
 FROM    items mo
 WHERE   NOT EXISTS
         (
         SELECT  NULL
         FROM    items mi
         WHERE   mi.id = mo.id + 1
         )
 ORDER BY
         id
 LIMIT 1


ARRAY(str) a; int i
PN
x.Exec(sql a)
PN;PO

for i 0 a.len
	out a[0 i]

 speed: 12000
