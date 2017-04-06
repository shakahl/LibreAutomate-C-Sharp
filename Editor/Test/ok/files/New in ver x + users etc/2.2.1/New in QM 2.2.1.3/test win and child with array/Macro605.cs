out
 opt hidden 1
 zw win("" "" "qm" 0 0 0 1)
ARRAY(int) a; int i
 a.create(10)
zw win("" "" "" 0 0 0 a)
 zw win("" "" "qm" 0 0 0 a)
 zw win("" "#32770" "qm" 0 0 0 a)
 zw win("" "#32770" "" 0 0 0 a)
 zw win("" "" "qm" 0x8000 &Function55 0 a)
 zw win("" "" "" 0x8000 &Function55 0)
out "---"
for i 0 a.len
	zw a[i]
	