out
Sqlite x.Open("")
x.Exec("PRAGMA foreign_keys=ON")
x.Exec("CREATE TABLE items (id INTEGER PRIMARY KEY, b BLOB)")
str sql=
 INSERT INTO items(b) VALUES
 (x'200F'),
 (x'100F')
x.Exec(sql)

ARRAY(str) a
 x.Exec("SELECT id,b FROM items" a)
x.Exec("SELECT id,b FROM items ORDER BY b" a)
for(_i 0 a.len)
	out "%s %s" a[0 _i] a[1 _i]

#ret
A
	B
	C
		D
		E
	F
G

0
1.0
1.1
1.1.0
1.1.1
1.2
2
