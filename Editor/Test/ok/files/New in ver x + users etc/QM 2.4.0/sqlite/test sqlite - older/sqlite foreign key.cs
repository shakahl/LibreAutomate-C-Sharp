out
Sqlite x.Open("")
str sql=
 PRAGMA foreign_keys=ON;
 CREATE TABLE items (id INTEGER PRIMARY KEY,name TEXT);
 CREATE TABLE texts (id INTEGER PRIMARY KEY REFERENCES items(id) ON DELETE CASCADE ON UPDATE CASCADE,text TEXT);
 INSERT INTO items (name) VALUES ('name1'),('name2'),('name3');
 INSERT INTO texts (text) VALUES ('text1'),('text2'),('text3');
x.Exec(sql)
 CREATE TABLE texts (id INTEGER PRIMARY KEY,text TEXT, FOREIGN KEY(id) REFERENCES items(id));

sql=
 DELETE FROM items WHERE id=2;
 UPDATE items SET id=7 WHERE id==1;
x.Exec(sql)
 DELETE FROM texts WHERE id=2;
 INSERT INTO items (name) VALUES ('name4');
 INSERT INTO texts (text) VALUES ('text44');
 UPDATE items SET id=(id-1) WHERE id>2;
 INSERT INTO texts (id,text) VALUES (4,'text44');

ARRAY(str) a; int i
out "items:"
x.Exec("SELECT * FROM items" a)
for i 0 a.len
	out F"{a[0 i]} '{a[1 i]}'"
out "texts:"
x.Exec("SELECT * FROM texts" a)
for i 0 a.len
	out F"{a[0 i]} '{a[1 i]}'"
