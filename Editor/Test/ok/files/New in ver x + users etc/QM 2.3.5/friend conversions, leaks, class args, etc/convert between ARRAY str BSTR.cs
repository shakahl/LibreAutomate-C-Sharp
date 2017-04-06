out

 ARRAY(str) a.create(2)
  ARRAY(lpstr) a.create(2)
 a[0]="one"; a[1]="two"
 
  ARRAY(str) aa=a
 ARRAY(BSTR) aa=a
 int i
 for i 0 aa.len
	 out aa[i]
 
 ARRAY(str) aaa=aa
 out aaa


ARRAY(str) a.create(2 3)
 ARRAY(lpstr) a.create(2 3)
a[0 0]="one"; a[1 0]="two"
a[0 1]="one1"; a[1 1]="two1"
a[0 2]="one2"; a[1 2]="two2"
out a.len

 ARRAY(str) aa=a
ARRAY(BSTR) aa=a
out aa.len
int i
for i 0 aa.len
	out aa[0 i]
	out aa[1 i]

ARRAY(str) aaa=aa
out aaa.len
for i 0 aaa.len
	out aaa[0 i]
	out aaa[1 i]
