out
 ARRAY(int) a.create(2)
 ARRAY(BSTR) a="bstr"
 ARRAY(str) a="str"
 ARRAY(lpstr) a.create(1); a[0]="lpstr"
ARRAY(POINT) a.create(2)
 IXml a._create
 BSTR a="llklkl"
 VARIANT v.attach(a)
VARIANT v=a
outx v.vt
 out a.psa
 out a
 out v
 ARRAY(int) b=v
 ARRAY(BSTR) b=v
 ARRAY(str) b=v
 ARRAY(lpstr) b=v
ARRAY(POINT) b=v
out b; err out "len=%i" b.len
