 /
function $script [VARIANT'p1] [VARIANT'p2] [VARIANT'p3] [VARIANT'p4] ;;up to p30

str s.from(script "(") ss
int i
VARIANT* a=&p1
for i 0 getopt(nargs)-1
	ss=a[i]
	sel a[i].vt
		case [VT_BSTR,VT_DATE] s.formata("''%s''," ss)
		case else s.formata("%s," ss)
s.rtrim(','); s+")"

 out s

run "ppbang.exe" s
