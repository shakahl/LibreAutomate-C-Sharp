out
Sqlite x.Open("")
str sql=
 CREATE TABLE test(id INT, tag TEXT COLLATE NOCASE);
 INSERT INTO test VALUES(1,'aaa'),(2,'bbb'),(3,'aaa'),(4,'BBB'),(1,'ccc');
x.Exec(sql)
ARRAY(str) a c; int i

 get list of tags
 x.Exec("SELECT DISTINCT tag FROM test" a)
  x.Exec("SELECT group_concat(DISTINCT tag) FROM test" a) ;;note with DISTINCT cannot use custom separator, default is comma


 get list of tags, and tags of id (for the dialog)
x.Exec("SELECT DISTINCT tag FROM test" a)
x.Exec("SELECT tag FROM test WHERE id=1" c)


 get list of tags of id
 x.Exec("SELECT tag FROM test WHERE id=1" a) ;;as array
 x.Exec("SELECT group_concat(tag) FROM test WHERE id=1" a) ;;as comma-separated list


 get all tags of ids as strings (for FTS)
 ARRAY(STRINT) m; int j
 SqliteStatement p.Prepare(x "SELECT id,tag FROM test")
 rep
	 if(!p.FetchRow) break
	 int k=p.GetInt(0); lpstr t=p.GetText(1)
	 for(j 0 m.len) if(k=m[j].i) break
	 if(j=m.len) STRINT& r=m[]; r.i=k; r.s=t
	 else m[j].s.from(m[j].s "," t)
 for(i 0 m.len) out F"{m[i].i} {m[i].s}"
 ret


 get ids of tag
 x.Exec("SELECT * FROM test WHERE tag = 'bbb'" a)


 delete tag
 x.Exec("DELETE FROM test WHERE tag = 'bbb'" a)


 rename tag
 x.Exec("UPDATE test SET tag='renamed' WHERE tag = 'bbb'" a)


 ___________________

 x.Exec("SELECT * FROM test" a)
if(!a.len) out "NONE"; ret
if(a.len(1)=1) for(i 0 a.len) out a[0 i]
else for(i 0 a.len) out F"{a[0 i]} {a[1 i]}"

if(c.len)
	out "---"
	for(i 0 c.len) out c[0 i]
	