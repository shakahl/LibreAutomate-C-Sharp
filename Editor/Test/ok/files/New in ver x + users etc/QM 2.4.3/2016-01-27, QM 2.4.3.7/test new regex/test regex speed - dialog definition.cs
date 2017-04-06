out
WakeCPU
str rx=__S_RX_DD
ARRAY(QMITEMIDLEVEL) a; int i r; str s dd
GetQmItemsInFolder "\System" a
IRegex x=CreateRegex(rx 0 1)
PerfOut 1
for i 0 a.len
	s.getmacro(a[i].id)
	PF
	 r=findrx(s rx 0 0 _i) ;;3600
	 r=RegexFind(s rx 0 0 _i) ;;9600
	r=x.Find(s 0 0 _i); ;;5800, JIT 2000
	PN
	PerfOut 2
	 out "%i %i" r _i
PerfOut 3
