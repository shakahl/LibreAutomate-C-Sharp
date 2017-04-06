out
int w=win("SQLite2009 Pro - The Best GUI to manage your SQLite3 databases." "ThunderRT6FormDC"); if(w) clo w
str sf="$my qm$\test\ok.db3"
if(!ConvertQmlToSqlite("$qm$\ok.qml" sf 1024*0)) out "FAILED"
PF
Sqlite x.Open(sf 0 2)
PN
x.Exec("PRAGMA foreign_keys=ON")
x.Exec("BEGIN")
 x.Exec("UPDATE items SET ord=25 WHERE ord=10")
int i j
 SqliteStatement k.Prepare(x "UPDATE items SET id=?1+1 WHERE id=?1")
 SqliteStatement t.Prepare(x "UPDATE texts SET id=?1+1 WHERE id=?1")
 for(i 0 a.len)
	 j=val(a[0 i])+m
	 k.BindInt(1 j); k.Exec; k.Reset
	 t.BindInt(1 j); t.Exec; t.Reset
ARRAY(str) a; x.Exec("SELECT ord FROM items ORDER BY ord LIMIT 2 OFFSET 1" a) ;;test
 ARRAY(str) a; x.Exec("SELECT ord FROM items WHERE id IN(1,2) ORDER BY ord" a) ;;real (1 id before, 2 id after (still in place of the new item))
for(i 0 a.len) out a[0 i]
#ret
int ord(val(a[0 1])) ordPrev(val(a[0 0]))
PN
out "ord=%i ordPrev=%i diff=%i" ord ordPrev ord-ordPrev
if ord-ordPrev<=1
	x.Exec(F"UPDATE items SET ord=ord+10 WHERE ord>{ordPrev};")
	ord+9
else ord-1
out ord
x.Exec(F"INSERT INTO items(ord,name) VALUES({ord}, 'NEW');")
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

#ret
 