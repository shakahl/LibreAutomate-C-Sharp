dll "qm.exe" %VariantToInt64 VARIANT*v

VARIANT v
 v=10
 v=1000000000
 v=10.55
 v="11111111"
 v="1111111111222222222"
 CURRENCY cy=78.5678; v=cy
 v.vt=VT_I8; v.cyVal.int64=1111111111222222222
 DATE d.getclock; v=d; out v.vt
 word w=50000; v=w
 byte w=200; v=w
 word w=-1; v=w; v.vt=VT_BOOL
 CURRENCY cy=78.5678; v=&cy; out "0x%X" v.vt
 v.vt=VT_ARRAY

out VariantToInt64(&v)

long k=v
out k
