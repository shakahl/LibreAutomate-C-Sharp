str s=
 This is an example to show what i want. This test is a test to see how this can be done.

 split s into words and add to array
ARRAY(str) a
tok s a

 add unique words to string map
IStringMap m=CreateStringMap(1)
int i n
for i 0 a.len
	str& r=a[i]
	if(m.IntGet(r n)) m.IntSet(r n+1); else m.IntAdd(r 1)

 results
m.GetList(s)
s.replacerx("^.+ 1[]" "" 8)
out s
