ARRAY(POINT) a
 out findrx("a,b," "\b" 0 4 a)
out findrx("a,b," "." 0 4 a)
int i
for i 0 a.len
	out "%i %i" a[0 i].x a[0 i].y
	

 str s="a,b,"
 out s.replacerx("\b" "R")
  out s.replacerx("." "R")
 out s
