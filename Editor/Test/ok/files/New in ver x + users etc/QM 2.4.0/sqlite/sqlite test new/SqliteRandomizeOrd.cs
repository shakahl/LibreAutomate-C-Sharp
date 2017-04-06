function $sf

Sqlite x.Open(sf 0 2)

ARRAY(int) a.create(x.ExecGetInt("SELECT count(*) FROM items"))
int i
for(i 0 a.len) a[i]=i
a.shuffle
for(i 1 a.len) if(a[i]=0) a[i]=a[0]; a[0]=0; break
for(i 1 a.len) x.Exec(F"UPDATE items SET ord={a[i]} WHERE id={i}")
