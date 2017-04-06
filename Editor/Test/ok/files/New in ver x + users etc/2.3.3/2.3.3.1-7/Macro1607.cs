out
dll "qm.exe" Test1 str* $fmt ...

str a1 a2
lpstr null
BSTR b="BSTR ąčę"

 _s.format("%S" b.pstr)
 _s=b

 str f="/%s/%5i/%f/%s/%S/%c/%C/%C/%C/%n/%Z/%wc/%#x/%.4m/%.*m/%m/%.5m/"
str f="/%s/%5i/%f/%s/%S/%c/%C/%C/%C/%n/%Z/%wc/%#x/%.4m/%.*m/%m/%.5m/"

Q &q
rep 1
	a1.format(f, "STRING", 55, 25,43, null, b.pstr 'A' b[1] b[5] 200 &_i 200 16 b.pstr 2 b.pstr 0 'A')
Q &qq
outq

out a1
outb a1 a1.len 1
out a2
outb a2 a2.len 1
