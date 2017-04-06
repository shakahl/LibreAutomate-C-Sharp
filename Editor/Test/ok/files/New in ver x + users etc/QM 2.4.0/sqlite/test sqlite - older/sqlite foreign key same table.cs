out
Sqlite x.Open("")
str sql=
 PRAGMA foreign_keys=ON;
 CREATE TABLE items (id INTEGER PRIMARY KEY,pid INTEGER REFERENCES items(id) ON DELETE CASCADE ON UPDATE CASCADE,name TEXT);
 INSERT INTO items VALUES (1,NULL,'name1'),(2,1,'name2'),(3,1,'name3'),(4,2,'name3'),(5,NULL,'name3');
x.Exec(sql)

sql=
 DELETE FROM items WHERE id=1;
x.Exec(sql)
 UPDATE items SET id=7 WHERE id=1;

ARRAY(str) a; int i
x.Exec("SELECT * FROM items" a)
for i 0 a.len
	out F"{a[0 i]} {a[1 i]} '{a[2 i]}'"
