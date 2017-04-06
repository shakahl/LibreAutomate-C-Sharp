 ARRAY Func(ARRAY ARRAY&)
out

 ARRAY(BSTR) a.create(2)

 ARRAY(str) a.create(1)
 ARRAY(str) a="one[]two"
ARRAY(BSTR) a.create(1); a[0]="test"
VARIANT v=a
outx v.vt
out v

 ARRAY(BSTR) as
ARRAY(str) as
as=v
out as
