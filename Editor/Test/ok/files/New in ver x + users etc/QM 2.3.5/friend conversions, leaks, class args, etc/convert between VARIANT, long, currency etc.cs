 long k=100
 CURRENCY c=k
 k=c
 out c
 out k

 long k=100
 VARIANT v=k
 CURRENCY c=v
 out c
 v=c
 k=v
 out k

 long k=1000000000000000L
 VARIANT v=k
 CURRENCY c=v
 out c

 VARIANT v=1000000000000L
 long k=v
 out k

 VARIANT v=0xffff0000ffff0000
 long k=~v
 out F"0x{k}"

 VARIANT v=1000000000000L
 double k=v
 out k

long k=10
 CURRENCY k=10
 DECIMAL k=10
VARIANT v=k
outx v.vt
long kk=v
 CURRENCY kk=v
 DECIMAL kk=v
out kk
