out
BSTR b="dd"
ARRAY(int) a.create(3)
 ARRAY(int) a.create(3 3)
VARIANT v="k"

SetLastError 5
 b="oo"
 out a[1 1]
 a.redim(5)
 a=0
 b=0
 VariantClear &v
 out "%i %i" v.vt v.lVal
out GetLastError