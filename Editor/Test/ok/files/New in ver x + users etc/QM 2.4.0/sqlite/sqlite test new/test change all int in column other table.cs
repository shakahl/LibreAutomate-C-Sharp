int w=win("SQLite2009 Pro - The Best GUI to manage your SQLite3 databases." "ThunderRT6FormDC"); if(w) clo w
str sf="$my qm$\test\ok.db3"
Sqlite x

 create 'tree' table
#if 0
if(!ConvertQmlToSqlite("$qm$\ok.qml" sf 1024*0)) out "FAILED"
x.Open(sf 0 2)
x.Exec("PRAGMA foreign_keys=ON")
x.Exec("BEGIN")
x.Exec("CREATE TABLE tree(id INTEGER PRIMARY KEY REFERENCES items(id) ON DELETE CASCADE ON UPDATE CASCADE,ord INT,pid INT)")
int i n=x.ExecGetInt("SELECT count(*) FROM items")
for(i 0 n) x.Exec(F"INSERT INTO tree VALUES({i},{i},(SELECT ia FROM items WHERE id={i}))")
x.Exec("END")
x.Close
Sqlite_Defragment sf
#endif

x.Open(sf 0 2)
x.Exec("PRAGMA foreign_keys=ON")
PF
x.Exec("BEGIN")
str sql=
F
 UPDATE tree SET ord=ord+1 WHERE ord>1;
rep(1) x.Exec(sql)

 rep(1000) int r=RandomInt(5 1000); x.Exec(F"UPDATE tree SET ord=ord+{RandomInt(-3 10)} WHERE ord>{r};")
x.Exec("END")
PN;PO
 UPDATE items SET ia=(ia+100001);
 UPDATE items SET ia=(ia-100000);

 run sf

 RESULTS:
 Don't use separate table for order/tree.
 Speed with 1 table 37 ms, with 2 tables 27 ms. With sync saving and 1 table 180 ms.
 Changing ord column in main items table does not make DB fragmented and does not slow down loading even after 1000 insertions (1000*11000 changes).
