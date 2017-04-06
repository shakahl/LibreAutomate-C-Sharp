out

 Create test array
ARRAY(VARIANT) b.create(3 12)
int i j; str s ss
ss=
 january
 february
 march
 april
 may
 june
 july
 august
 september
 october
 november
 december
for i 0 b.len
	b[0 i]=i+1
	s.getl(ss -i)
	b[2 i]=s


ExcelSheet es.Init
Excel.Range f=es.ws.Range(_s.format("C1:C%i" b.len))
 f.Select

ARRAY(VARIANT) aa.create(b.len 1)
for(i b.lbound b.ubound+1) aa[i 0]=b[2 i]
f.Value=aa
