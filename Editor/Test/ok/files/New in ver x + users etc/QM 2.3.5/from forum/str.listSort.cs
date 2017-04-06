function [sort] ;;sort: 1 descending, 2 case insens., 4 ling, 6 ling/insens, 8 numbers/ling/insens, 0x100 date.

 EXAMPLE
 str s=
  oNe
  five
  ą non ASCII
 s.listSort(6)
 out s


ARRAY(str) a=this

int i
for i 0 a.len
	str& s=a[i]
	if(!s.len) continue
	s.lcase
	if s[0]<128
		s[0]=toupper(s[0])
	else
		BSTR b=s
		b[0]=CharUpperW(+b[0])
		s=b

a.sort(sort)
this=a
