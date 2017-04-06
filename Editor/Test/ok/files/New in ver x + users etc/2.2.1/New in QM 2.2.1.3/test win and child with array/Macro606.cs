out
ARRAY(int) a; int i; str sc sn
out "[][9]ALL VISIBLE WINDOWS"
win("" "" "" 0 0 0 a)
for(i 0 a.len)
	sc.getwinclass(a[i])
	sn.getwintext(a[i])
	out "%i '%s' '%s'" a[i] sc sn

out "[][9]ALL INVISIBLE WINDOWS"
opt hidden 1
win("" "" "" 0 0 0 a)
for(i 0 a.len)
	if(!hid(a[i])) continue ;;this window is visible
	sc.getwinclass(a[i])
	sn.getwintext(a[i])
	out "%i '%s' '%s'" a[i] sc sn

out "[][9]ALL WINDOWS OF EXPLORER"
opt hidden 1
win("" "" "explorer" 0 0 0 a)
for(i 0 a.len)
	sc.getwinclass(a[i])
	sn.getwintext(a[i])
	out "%i '%s' '%s'" a[i] sc sn

  zw win("" "" "qm" 0 0 0 1)
 ARRAY(int) a; int i
  zw win("" "" "" 0 0 0 a)
