 Compare QM and C speed in low level string processing.
 Run this several times.

str s.RandomString(100 100)
str s2=s
str s3=s

int t1=perf

 QM
int i j
for(i 0 1000)
	for(j 0 s2.len) if(s2[j]>='A' && s2[j]<='Z') s2[j]+=32;

int t2=perf

 C

lpstr c=
 void main(char* s2, int n)
 {
 int i, j;
 for(i=0; i<1000; i++)
	 {
	 for(j=0; j<n; j++) if(s2[j]>='A' && s2[j]<='Z') s2[j]+=32;
	 }
 }
__Tcc+ f_tccsptest
if(!f_tccsptest.f)
	f_tccsptest.Compile(c "main")
call f_tccsptest.f s3 s3.len

int t3=perf
out "QM %i, C %i.  C is %i times faster." t2-t1 t3-t2 (t2-t1)/(t3-t2)

out s
out s2
out s3
