out
int w=win("SQLite2009 Pro - The Best GUI to manage your SQLite3 databases." "ThunderRT6FormDC"); if(w) clo w
str sf="$my qm$\test\ok.db3"
if(!ConvertQmlToSqlite("$qm$\ok.qml" sf 1024*0)) out "FAILED"
PF
Sqlite x.Open(sf 0 2)
PN
 x.Exec("PRAGMA foreign_keys=ON")
x.Exec("BEGIN")
int i j
ARRAY(str) a
x.Exec("SELECT id FROM items WHERE id>1 ORDER BY id DESC" a)
PN
int m
for m 0 10
	 for(i 0 a.len) j=val(a[0 i])+m; x.Exec(F"UPDATE items SET id={j+1} WHERE id={j}")
	SqliteStatement k.Prepare(x "UPDATE items SET id=?1+1 WHERE id=?1")
	SqliteStatement t.Prepare(x "UPDATE texts SET id=?1+1 WHERE id=?1")
	for(i 0 a.len)
		j=val(a[0 i])+m
		k.BindInt(1 j); k.Exec; k.Reset
		t.BindInt(1 j); t.Exec; t.Reset
str sql=
F
 INSERT INTO items(id,name) VALUES(2, 'NEW');
x.Exec(sql)
x.Exec("END")
PN;PO
 UPDATE items SET id=-(id+1) WHERE (id>1);
 UPDATE items SET id=-id WHERE (id<0);
 UPDATE items SET id=(id+100000) WHERE id IN(SELECT id FROM items ORDER BY id DESC LIMIT count(items.id)-10);
 UPDATE items SET id=(id+101) WHERE id IN(SELECT id FROM items ORDER BY id DESC LIMIT (SELECT count(id) FROM items)-2);
 UPDATE items SET id=(id+101) WHERE id IN(SELECT id FROM items ORDER BY id DESC);
 UPDATE items SET id=(id+100001) WHERE id>1;
 UPDATE items SET id=(id-100000) WHERE id>100000;
 UPDATE items SET id=(id+100001) WHERE id>1;
 UPDATE items SET id=(id-100000) WHERE id>100000;

 run sf
