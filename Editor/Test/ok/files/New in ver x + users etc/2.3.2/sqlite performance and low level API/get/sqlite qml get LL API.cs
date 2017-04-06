out
int t1 t2
Sqlite x.Open("$desktop$\qml.db3")

t1=timeGetTime

ARRAY(str) a.create(10 0)
int i j all

str s="SELECT * FROM items"
SqliteStatement p.Prepare(x s)

rep
	if(!p.FetchRow) break
	 out "-----------------------------"
	 i=a.redim(-1)
	 if(i&255=0) a.redim(i+256)
	 if(i=all) all+all/4+4; a.redim(all)
	
	 int iid=__sqlite.sqlite3_column_int(p 0)
	 lpstr name=__sqlite.sqlite3_column_text(p 4)
	 lpstr text=__sqlite.sqlite3_column_text(p 5)
	 lpstr trigger=__sqlite.sqlite3_column_text(p 6)
	
	 for(j 0 10) a[j i]=p.GetText(j)
	 for(j 0 10) p.GetText(j)
	 for(j 0 10) __sqlite.sqlite3_column_text(p.p j)
	for(j 0 10)
		 lpstr sv=__sqlite.sqlite3_column_text(p.p j)
		byte* v=__sqlite.sqlite3_column_value(p.p j)
		 if(j=0) out __sqlite.sqlite3_value_int(v)
		if(j=4) out __sqlite.sqlite3_value_text(v)
	i+1

a.redim(i)

t2=timeGetTime
out t2-t1
out i

 Times:
 Now QM opens file: 20 ms.
 This code: 220.
 When not using a:
   without get column: 110 ms.
   get 1 text column: 120 ms.
   get 4 various columns without conversion: 120 ms.


 for i 0 a.len
	 out "----------------------------------"
	 out a[4 i]
