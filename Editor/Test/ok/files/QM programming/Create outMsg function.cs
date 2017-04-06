 Copy block of #define and run this function.
 It creates C++ function that maps constant value to constant name.


str s.getclip
 str s=
 #define SCN_STYLENEEDED 2000
 #define SCN_CHARADDED 2001
 #define SCN_SAVEPOINTREACHED 2002

str ss=
 #ifdef _DEBUG
 void outSCN(int x, int skip, ...)
 {
 	for(int* p=&skip; *p; p++) if(x==*p) return;
 
 	static const DWORDLPSTR a[ ]={
;
ARRAY(str) a; int i
findrx(s "^#define\s+(\w+)\s+(\d+)" 0 8|4 a)
for i 0 a.len
	ss.formata("[9]{%s, ''%s''},[]" a[2 i] a[1 i])

_s=
 	};
 
 	zz(MapDwordToLpstr(x, a, lenof(a)));
 }
 #endif
;
ss+_s

 out ss
ss.setclip
