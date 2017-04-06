out
str csv=
 0
 1
 2
 3
 4

ICsv x._create
x.FromString(csv)

 ARRAY(int) a.create(1)
 a[0]=2
ARRAY(int) a.create(2)
 a[0]=0
 a[1]=1
a[0]=1
a[1]=3

x.MoveRows(a 5)
if(_hresult) out "_hresult=%i" _hresult

x.ToString(_s)
out _s
