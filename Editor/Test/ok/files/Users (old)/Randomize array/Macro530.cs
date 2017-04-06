int t1=perf
int i j
for(i 0 1000) j=Uniform(1 100)
int t2=perf
srand GetTickCount
for(i 0 1000) j=rand/650
int t3=perf
out "%i %i" t2-t1 t3-t2
