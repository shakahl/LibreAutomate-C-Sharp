out
int t1 t2
Sqlite x.Open("$desktop$\qml.db3")

t1=timeGetTime

ARRAY(str) a
x.Exec2("SELECT * FROM items" a)
 x.Exec2("SELECT * FROM items LIMIT 10" a)
 x.Exec2("SELECT text FROM items" a)

t2=timeGetTime
out t2-t1
out a.len

 Times:
 Now QM opens file: 20 ms.
 This code: 230 ms.
 If 1 column (name): 70 ms.
 If 1 column (text): 120 ms.
 sqlite3_get_table: 180. Fill array: 50.

 int i
 for i 0 a.len
	 out "----------------------------------"
	 out a[0 i]
