out
 VARIANT v="11111111112222"
 VARIANT v="1111111111222222222"
VARIANT v="11111111112222222"

 int hr=VariantChangeTypeEx(&v, &v, 0x409, 0, VT_CY)
 int hr=VariantChangeTypeEx(&v, &v, 0x409, 0, VT_I8)
int hr=VariantChangeTypeEx(&v, &v, 0x409, 0, VT_R8)
out hr
out v.cyVal
out v.cyVal.int64
long k=v.dblVal
out k
