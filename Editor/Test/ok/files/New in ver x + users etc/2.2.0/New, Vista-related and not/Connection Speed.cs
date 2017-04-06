out
str src.expandpath("$qm$\ok old 2.qml")
str dest.expandpath("$desktop$\ok old.qml")
del- dest; err
1

int t1=perf
 5
 cop src dest
 out CopyFile(src dest 0)
str s.getfile(src)

int t2=perf
t1=t2-t1/1000 ;;ms
int sz=s.len/1000 ;;KB
out "time=%ims, size=%iKB, speed=%iKBS" t1 sz sz*1000/t1
 out s.len
 size/time=x/1000
 x=size/time*1000
