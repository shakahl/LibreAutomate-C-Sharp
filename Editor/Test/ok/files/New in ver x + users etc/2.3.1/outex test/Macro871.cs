out
act outex_create_window
str s
s="test"
 s.getmacro("winapiv.txt") ;;116749
 s.fix(100)
 out s.len

outex s
int i
for i 1 20
	_s.from(i)
	Q &q
	outex _s _s
	Q &qq
	outq
