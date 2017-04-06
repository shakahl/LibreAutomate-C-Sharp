str s
int t1=perf
long i=1234567890123

rep 1000
	s.format("%s %i %X %1.6f %c %I64i" "string" 1000 1000 24.56 'a' i)
out s

int t2=perf
out t2-t1
