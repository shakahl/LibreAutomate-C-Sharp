int w=win("SQLite2009 Pro - The Best GUI to manage your SQLite3 databases." "ThunderRT6FormDC"); if(w) clo w
str sf="$my qm$\test\ok.db3"
 if(!ConvertQmlToSqlite("$qm$\ok.qml" sf 1024*0)) out "FAILED"
PF
Sqlite x.Open(sf 0 2)
x.Exec("PRAGMA foreign_keys=ON")
PN
x.Exec("BEGIN")
int i
str sql=
F
 UPDATE items SET ord=ord+1 WHERE ord>1;
rep(1) x.Exec(sql)

 rep(1000) int r=RandomInt(5 1000); x.Exec(F"UPDATE items SET ord=ord+{RandomInt(-3 10)} WHERE ord>{r};")
x.Exec("END")
PN;PO
 UPDATE items SET ord=(ord+100001);
 UPDATE items SET ord=(ord-100000);

 run sf
