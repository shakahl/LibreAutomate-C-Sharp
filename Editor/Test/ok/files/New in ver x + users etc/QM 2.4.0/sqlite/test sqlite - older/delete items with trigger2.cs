dll "qm.exe" !ConvertQmlToSqlite $qmlFile $db3File [pageSize]

out
str sf
sf="$my qm$\test\ok.db3"
if(sf.len)
	if(!ConvertQmlToSqlite("$qm$\ok.qml" sf 1024*0)) out "FAILED"

Sqlite x.Open(sf 0 2)
x.Exec("PRAGMA foreign_keys=ON;")
 x.Exec(F"UPDATE items SET flags={0x80000000} WHERE (id>=3 AND id<=11410);")
rep(1000) x.Exec(F"UPDATE items SET flags={0x80000000} WHERE id={RandomInt(5 11415)};")
x.Exec("BEGIN TRANSACTION")

PF
str trig=
F
 CREATE TEMP TRIGGER IF NOT EXISTS onDelete AFTER DELETE ON items
 BEGIN
 UPDATE items SET id=old.id WHERE (id=(SELECT id FROM items ORDER BY id DESC LIMIT 1) AND id>old.id AND NOT(flags&{0x80000000}));
 END
x.Exec(trig)
 UPDATE items SET id=old.id WHERE (id=(SELECT max(id) FROM items) AND id>old.id AND NOT(flags&{0x80000000}));
 UPDATE items SET id=old.id WHERE (id=(SELECT max(id) FROM items) AND id>old.id);
 UPDATE items SET id=old.id WHERE (id>old.id AND id=(SELECT max(id) FROM items));
PN
x.Exec(F"DELETE FROM items WHERE flags&{0x80000000}")
PN
x.Exec("END TRANSACTION")
PN;PO
ret
ARRAY(str) a
 x.Exec("SELECT id,name FROM items" a)
x.Exec("SELECT id,name FROM items" a)
for(_i 0 a.len) out "%s %s" a[0 _i] a[1 _i]
