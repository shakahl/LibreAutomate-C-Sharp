function# $url

 Attempts to connect to a website.
 If successful, returns time (ms) spent to connect. If cannot connect, returns 0.

 Added in: QM 2.3.2.


long t1=perf
if(!InternetCheckConnectionW(@url FLAG_ICC_FORCE_CONNECTION 0)) ret
long t2=perf
int i=t2-t1/1000
if(!i) i=1
ret i
