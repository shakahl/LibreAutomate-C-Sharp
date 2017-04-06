out

lpstr macro=+qmitem
 lpstr macro="init"

 _qmfile.SettingAddB(macro "test" "TEST" 4)

int iid rowid; GUID guid
iid=_qmfile.SqliteItemProp(macro rowid guid)
 out iid
 out rowid
 outb &guid 16

 out _s.SqlBlob(&guid 16)
 out _s.SqlBlob(0 0)

 Sqlite& x=_qmfile.SqliteBegin
Sqlite& x=_qmfile.SqliteBegin(iid)
 _qmfile.SqliteBegin

ARRAY(str) a; int i
 x.Exec("SELECT name FROM items" a)
 for(i 0 a.len) out a[0 i]
 x.Exec(F"SELECT name FROM items WHERE rowid={rowid}" a)
 x.Exec(F"SELECT name FROM items WHERE guid={_s.SqlBlob(&guid 16)}" a)
 for(i 0 a.len) out a[0 i]

 str s1.SqlBlob(&guid 16) s2.SqlBlob(0 0)
 x.Exec("DROP TABLE IF EXISTS nnn")
 x.Exec(F"CREATE TABLE IF NOT EXISTS nnn(a INT,b BLOB);INSERT INTO nnn(a,b)VALUES(5,{s1}),(6,{s2});")
  x.Exec(F"SELECT a FROM nnn WHERE b={_s.SqlBlob(&guid 16)}" a)
  for(i 0 a.len) out a[0 i]
 SqliteStatement p.Prepare(x "SELECT a FROM nnn WHERE b=?")
 p.BindBlob(1 &guid sizeof(GUID))
 rep() if(p.FetchRow) out p.GetInt(0); else break

x.Exec(F"SELECT DISTINCT name FROM xSett WHERE name GLOB 'tb *'" a)
for(i 0 a.len) out a[0 i]

 mes 1
_qmfile.SqliteEnd
 mes 2
 _qmfile
