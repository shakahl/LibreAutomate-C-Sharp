int w=win("SQLite2009 Pro - The Best GUI to manage your SQLite3 databases." "ThunderRT6FormDC"); if(w) clo w
str sf="$my qm$\test\ok.db3"
 if(!ConvertQmlToSqlite("$qm$\ok.qml" sf 1024*0)) out "FAILED"

Sqlite x.Open(sf 0 2)
x.Exec("ALTER TABLE items ADD 'ord' INT")

ARRAY(int) a.create(x.ExecGetInt("SELECT count(*) FROM items"))
int i
for(i 0 a.len) a[i]=i
a.shuffle
for(i 0 a.len) x.Exec(F"UPDATE items SET ord={a[i]} WHERE id={i}")


run sf
