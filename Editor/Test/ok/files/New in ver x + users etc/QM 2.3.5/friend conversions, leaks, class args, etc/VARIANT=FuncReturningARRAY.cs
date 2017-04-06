 VARIANT v=RetArrayStr
 outx v.vt

CsScript x.Init

ARRAY(BSTR) as="one"
 ARRAY(int) as.create(1)
 ARRAY(str) as="one"
VARIANT v=as
 VARIANT v=&as
 VARIANT v=x.x.RetArray
outx v.vt
 out v

 ArgArray as
 ArgVariant2 as
 ArgVariant2 x.x.RetArray

 ARRAY(BSTR) a=x.x.RetArray
 out a.len
 outx a.psa.fFeatures

  good
 ARRAY(BSTR) a=x.x.RetArray
 out a.len
 for(_i 0 a.len) out a[_i]
