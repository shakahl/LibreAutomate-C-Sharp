out
typelib Scripting {420B2830-E718-11CF-893D-00A0C9054228} 1.0

Scripting.Dictionary m._create

int i n(100000)
srand(5554)
ARRAY(str) a.create(n)
for(i 0 n) a[i].from(rand "-" i)
str s v
VARIANT vk vv

int t1=perf

for i 0 n
	vk=a[i]
	m.Add(vk vk)

int t2=perf

for i 0 n
	vk=a[i]
	v=m.Item(vk)

int t3=perf
out "%i %i" t2-t1 t3-t2
out v

5
