str s=
 >Menu 1
 1 Voice 1 ; Tag 1
 2 Voice 2
 3 Voice 3 ; Tag 1
 <

str tag=" ; Tag 1"
ARRAY(str) a; int j
if findrx(s F"^(.+){tag}$" 0 4|8 a) ;;get all tagged lines
	s.findreplace(tag) ;;remove tags
	s.addline(">Menu 2")
	for(j 0 a.len) s.addline(a[1 j])
	s.addline("<")

int i=ShowMenu(s)
out i
