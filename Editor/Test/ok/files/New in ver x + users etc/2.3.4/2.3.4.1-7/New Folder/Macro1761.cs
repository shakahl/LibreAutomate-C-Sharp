out
int w=win("Calculator")
Acc a.Find(w "" "Calculator" "" 0x1005)

ARRAY(Acc) b
int i

a.GetChildObjects(b -1 "PUSHBUTTON")

for i 0 b.len
	 out b[i].Name
	int x y
	b[i].Location(x y)
	str s=i
	OnScreenDisplay s 1 x y 0 14 0 2
