ARRAY(str) a="str"
 ARRAY(lpstr) a.create(1); a[0]="lpstr"
 ARRAY(BSTR) a="bstr"
VARIANT v=a
BSTR b=a
 BSTR b=v
out b

 a=0
 a=b
 out a
