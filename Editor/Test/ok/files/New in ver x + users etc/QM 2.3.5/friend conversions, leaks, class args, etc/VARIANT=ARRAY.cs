  BSTR a
  IUnknown a
  IDispatch a
  IXml a
  Excel.Application a
  VARIANT a=1
 ARRAY(BSTR) a.create(1)
  ARRAY(POINT) a.create(1)
  VARIANT v=a
  a=v
 VARIANT v.attach(a)
 outx v.vt


 ARRAY(int) a.create(1)
ARRAY(long) a.create(1)
 ARRAY(BSTR) a.create(1)
 ARRAY(VARIANT) a.create(1)
 ARRAY(str) a.create(1)
 ARRAY(lpstr) a.create(1)
 ARRAY(IUnknown) a.create(1)
 ARRAY(IDispatch) a.create(1)
 ARRAY(POINT) a.create(1)
 ARRAY(POINT*) a.create(1)
 ARRAY(__MemBmp) a.create(1)
out F"0x{a.psa.fFeatures} {a.psa.cbElements}"

 word vt
 int hr=SafeArrayGetVartype(a &vt)
 out F"0x{vt}  0x{hr}"
 if(hr) out _s.dllerror("" "" hr)

VARIANT v=a
outx v.vt

 a=v
