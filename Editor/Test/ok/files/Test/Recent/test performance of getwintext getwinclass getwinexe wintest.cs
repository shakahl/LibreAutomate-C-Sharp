ARRAY(int) a
int i tavg
long t1 t2

win("" "" "" 0 0 0 a); if(!a.len) ret
out "Testing with %i windows. Average time in ns." a.len

t1=perf
for(i 0 a.len) rep(1000) _s.getwinclass(a[i]); _s.all
t2=perf; tavg=t2-t1/a.len
out "getwinclass: %i" tavg

t1=perf
for(i 0 a.len) rep(1000) wintest(a[i] "" "test class")
t2=perf; tavg=t2-t1/a.len
out "wintest(hwnd '''' ''test class''): %i" tavg

t1=perf
for(i 0 a.len) rep(1000) _s.getwintext(a[i]); _s.all
t2=perf; tavg=t2-t1/a.len
out "getwintext: %i" tavg

t1=perf
for(i 0 a.len) rep(1000) _s.getwinexe(a[i]); _s.all
t2=perf; tavg=t2-t1/a.len
out "getwinexe (filename): %i" tavg

t1=perf
for(i 0 a.len) rep(1000) _s.getwinexe(a[i] 1); _s.all
t2=perf; tavg=t2-t1/a.len
out "getwinexe (full path): %i" tavg
