out
Sqlite x.Open("")
x.Exec("PRAGMA foreign_keys=ON")
x.Exec("CREATE TABLE items (id INTEGER PRIMARY KEY, name TEXT)")
x.Exec("INSERT INTO items(name) VALUES('one'),('two'),('three'),('four'),('five'),('six'),('seven'),('eith')")

PF
str trig=
 CREATE TEMP TRIGGER onDelete AFTER DELETE ON items
 BEGIN
 UPDATE items SET id=old.id WHERE (id=(SELECT id FROM items ORDER BY id DESC LIMIT 1) AND id>old.id);
 END
x.Exec(trig)
 UPDATE items SET id=old.id WHERE (id=(SELECT max(id) FROM items) AND id>old.id);
 UPDATE items SET id=old.id WHERE (id>old.id AND id=(SELECT max(id) FROM items));
PN
x.Exec("BEGIN TRANSACTION")
x.Exec("DELETE FROM items WHERE (id>=3 AND id<=6)")
 x.Exec("DELETE FROM items WHERE (id>=8)")
 x.Exec("DELETE FROM items WHERE id=3")
x.Exec("END TRANSACTION")
PN;PO

ARRAY(str) a
x.Exec("SELECT id,name FROM items" a)
for(_i 0 a.len) out "%s %s" a[0 _i] a[1 _i]
