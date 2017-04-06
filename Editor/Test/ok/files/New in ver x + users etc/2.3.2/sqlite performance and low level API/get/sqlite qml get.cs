out
int t1 t2
Sqlite x.Open("$desktop$\qml.db3")

t1=timeGetTime

ARRAY(str) a
x.Exec("SELECT * FROM items" a)
 x.Exec("SELECT text FROM items" a)

t2=timeGetTime
out t2-t1
out a.len

 Times:
 Now QM opens file: 20 ms.
 This code: 200 ms.
 If 1 column (name): 75 ms.
 If 1 column (text): 125 ms.

 int i
 for i 0 a.len
	 out "----------------------------------"
	 out a[0 i]
