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
	
	a[0 i]=p.GetText(+"id")
	a[1 i]=p.GetText(+"folderid")
	a[2 i]=p.GetText(+"flags")
	a[3 i]=p.GetText(+"datemod")
	a[4 i]=p.GetText(+"name")
	a[5 i]=p.GetText(+"text")
	a[6 i]=p.GetText(+"trigger")
	a[7 i]=p.GetText(+"programs")
	a[8 i]=p.GetText(+"ff")
	a[9 i]=p.GetText(+"linktarget")
	i+1

a.redim(i)

t2=timeGetTime
out t2-t1
out i

 Times:
 Now QM opens file: 20 ms.
 This code: 370 (without col name 220; with col name caching 330).
 When not using a:
   without get column: 110 ms.
   get 1 text column: 120 ms.
   get 4 various columns without conversion: 120 ms.


 for i 0 a.len
	 out "----------------------------------"
	 out a[4 i]
