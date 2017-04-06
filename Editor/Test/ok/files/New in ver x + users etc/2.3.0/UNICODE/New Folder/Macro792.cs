out
Database d.Open(d.CsAccess("$desktop$\ąčę.mdb"))
ARRAY(str) a
d.QueryArr("SELECT * FROM tš" a)
int i
for i 0 a.len
	out a[0 i]
	out a[1 i]
	out a[2 i]
	
