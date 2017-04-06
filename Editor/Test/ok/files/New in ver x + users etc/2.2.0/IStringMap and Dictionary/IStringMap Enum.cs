out

IStringMap m=CreateStringMap(1)
int i n(10)
srand(5554)
ARRAY(str) a.create(n)
for(i 0 n) a[i].from(rand "-" i)
VARIANT vk

for(i 0 n) m.Add(a[i] a[i])

m.GetAll(a 0)

str sk sv
m.EnumBegin
for i 0 1000000000
	if(!m.EnumNext(sk sv)) break
	out "%s=%s" sk sv
	if(i=3)
		 m.Add("19" "19")
		 m.Add("19" "19")
		m.Add("0k" "0k")
		m.Add("2k" "2k")
		 m.Add("kk" "kk")
		 m.Remove(a[1])
		 m.Remove(a[8])
		 m.Remove(a[4])
	 if(i=8)
		 m.Add("kk" "kk")
		 m.Remove(a[9])

 1072-1=1072-1
 17342-2=17342-2
 17537-4=17537-4
 18175-0=18175-0
 22177-3=22177-3
 26182-6=26182-6
 27129-7=27129-7
 28500-9=28500-9
 30135-5=30135-5
 30958-8=30958-8
