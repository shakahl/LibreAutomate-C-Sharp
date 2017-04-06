ARRAY(str) a
ARRAY( str ) a2
ARRAY(str*) a3
ARRAY(str**) a4
ARRAY(str***) a5
 ARRAY(str****) a6
 ARRAY(str&) a7
 ARRAY(str *) a8
 ARRAY(str[10]) a9
ARRAY(str)- a10

type TED ARRAY(str**)'a b
TED t

str s
 a3[]=&s
 a3[0].from(1 2)
 out *a3[0]

str* sp=&s
a4[]=&sp
a4[0].from(1 2)
str* sp2=*a4[0]
out *sp2

string
_s

