function` ICsScript&x $name [`a1] [`a2] [`a3] [`a4] [`a5] [`a6] [`a7] [`a8] [`a9] [`a10]
 function` ICsScript&x $name [`a1] [`a2] [`a3] [`a4]

 ret

#if 0

int i na=getopt(nargs)-2
VARIANT* p=&a1
ARRAY(VARIANT) a.create(na)
for i 0 na
	 a[i]=p[i]
	a[i].attach(p[i])
ret x.Call(name a)

#else

int vt=VT_VARIANT
SAFEARRAY sa.cbElements=16; sa.cDims=1
sa.fFeatures=FADF_VARIANT|FADF_HAVEVARTYPE
sa.rgsabound[0].cElements=getopt(nargs)-2
sa.pvData=&a1
ret x.Call(name &sa)
