out
str s rx; int r
s="aa 7word bb 8;; cc"
 rx="\d(?<na>\w+)"
int n=1000; _s.RandomString(n n "a-z"); s-_s
rx="\d(\w+)"

WakeCPU
IRegex x=CreateRegex(rx 0 1)
PF
rep 100
	 r=findrx(s rx 0 0 _s) ;;80 400(n=100) 3000(n=1000)
	 r=RegexFind(s rx 0 _s) ;;130 150(n=100) 260(n=1000)
	r=x.Find(s 0 _s) ;;170(n=1000), JIT 110
PN;PO
out r
out _s
 out _i
