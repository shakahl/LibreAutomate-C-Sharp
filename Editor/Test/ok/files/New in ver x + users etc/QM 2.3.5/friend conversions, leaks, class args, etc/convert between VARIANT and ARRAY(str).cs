out
ARRAY(str) a.create(2)
 ARRAY(lpstr) a.create(2)
a[0]="one"; a[1]="two"
VARIANT v=a
 VARIANT v.attach(a) ;;will not convert ARRAY(str) to ARRAY(BSTR), but it's OK
outx v.vt

 BSTR* b=v.parray.psa.pvData
 int i
 for i 0 v.parray.psa.rgsabound[0].cElements
	 out b[i]

ARRAY(BSTR) aa=v
int i
for i 0 aa.len
	out aa[i]

 ARRAY(str) aa=v
 int i
 for i 0 aa.len
	 out aa[i]

 ARRAY(str) aa=a
 int i
 for i 0 aa.len
	 out aa[i]


 out "---- call"
  TestVariantArrayStr(a 5)
 v=TestVariantArrayStr(a 5)
  ARRAY(str) aa=TestVariantArrayStr(a 5)
 out "---- returned"
  out aa
