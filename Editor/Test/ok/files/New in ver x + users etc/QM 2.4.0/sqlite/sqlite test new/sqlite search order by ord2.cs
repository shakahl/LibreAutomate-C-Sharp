 out
str sf="$my qm$\test\ok.db3"
Sqlite x.Open(sf 0 2)
PF
 int ord=x.ExecGetInt("SELECT max(ord) FROM items WHERE ord<50")
 int ord=x.ExecGetInt("SELECT min(ord) FROM items WHERE ord>50")
 double ord=x.ExecGetDouble("SELECT max(ord) FROM items WHERE ord<50")
SqliteStatement p.Prepare(x "SELECT max(ord),id FROM items WHERE ord<50")
if(p.FetchRow) int ord(p.GetInt(0)) rowid=p.GetInt(1)
p.Delete
PN;PO
out ord
out rowid
