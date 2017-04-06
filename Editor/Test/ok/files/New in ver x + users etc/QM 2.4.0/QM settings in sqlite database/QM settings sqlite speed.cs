 SLOWER

 out
str s1 s2; int i1 i2
PF
Sqlite x.Open("$my qm$\settings.db3" 0 4)
SqliteStatement p.Prepare(x "SELECT value FROM qm WHERE name=?1")

PN
 i1=x.ExecGetInt("SELECT value FROM qm WHERE name='unicode'")
p.Reset; p.BindText(1 "unicode"); if(p.FetchRow) i1=p.GetInt(0)
PN
 i2=x.ExecGetInt("SELECT value FROM qm WHERE name='acclog flags'")
p.Reset; p.BindText(1 "acclog flags"); if(p.FetchRow) i2=p.GetInt(0)
PN
 x.ExecGetText(s1 "SELECT value FROM qm WHERE name='file'")
p.Reset; p.BindText(1 "file"); if(p.FetchRow) s1=p.GetText(0)
PN
 x.ExecGetText(s2 "SELECT value FROM qm WHERE name='backups'")
p.Reset; p.BindText(1 "backups"); if(p.FetchRow) s2=p.GetText(0)
PN;PO
out i1
out i2
out s1
out s2

