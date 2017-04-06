int t1=perf
int hp
StartProcess 5 "$my qm$\exe.exe" "" "" &hp
wait 0 H hp
int t2=perf
out t2-t1
