out

IStringMap m=CreateStringMap(0)
int i n(10000)
lpstr v
srand(5554)
ARRAY(str) a.create(n)
for(i 0 n) a[i].from(rand "-" i)
VARIANT vk

int t1=perf

for(i 0 n)
	 vk=a[i]
	m.Add(a[i] a[i])

 out "----"
int t2=perf

for(i 0 n)
	 vk=a[i]
	v=m.Get(a[i]);; if(!v) out i

int t3=perf
out "%i %i" t2-t1 t3-t2
 out v

 5
 ret
 _________________________________

 t1=perf
 
 str sk sv
 m.EnumBegin
 rep
	 if(!m.EnumNext(sk sv)) break
	  sk="kjlkjhgf"; sv="jhgkjhgkjh"
	  out "%s=%s" sk sv
 
 t2=perf
 
 ARRAY(str) ak av
 m.GetAll(ak av)
 
 t3=perf
 out "%i %i" t2-t1 t3-t2
