out

str si
if(!inp(si)) ret

str s
s.getfile("$temp$\test.txt"); err

s.listSort(6)
out s

IStringMap m=CreateStringMap(1|2)
if(s.len) m.AddList(s "[]")
if m.Get(si)
	out "exists"
else
	m.Add(si)
	m.GetList(s)
	s.setfile("$temp$\test.txt")
