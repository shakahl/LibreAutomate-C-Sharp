function [`a1] [`a2]

 outb &a1 16
outx a1.vt
out a2
#ret

ARRAY(str) a.psa=a1.parray
 out a.len
out a.psa.cbElements
out a[0]
out a[1]

a.psa=0
