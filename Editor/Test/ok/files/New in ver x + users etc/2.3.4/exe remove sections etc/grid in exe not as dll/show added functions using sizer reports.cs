out
str s k d
IStringMap m=CreateStringMap
s.getmacro("without grid")
m.AddList(s "[]")
s.getmacro("with grid")
foreach k s
	if(!k.len) continue
	if(!m.Get(k)) d.addline(k)
out d
