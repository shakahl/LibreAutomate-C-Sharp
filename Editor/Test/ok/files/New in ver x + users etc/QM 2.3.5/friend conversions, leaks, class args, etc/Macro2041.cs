out
 ARRAY(int) a.create(2)
 ARRAY(BSTR) a="bstr"
 ARRAY(str) a="str"
 ARRAY(lpstr) a.create(1); a[0]="lpstr"
ARRAY(POINT) a.create(2)

 ARRAY(int) b=a
 ARRAY(BSTR) b=a
 ARRAY(str) b=a
 ARRAY(lpstr) b=a
ARRAY(POINT) b=a
out F"0x{b.psa.fFeatures} {b.psa.cbElements}"
out b; err out "len=%i" b.len
