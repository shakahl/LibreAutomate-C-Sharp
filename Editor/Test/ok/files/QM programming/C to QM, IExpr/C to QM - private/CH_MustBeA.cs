 /get dll names
function str&s str&sout

 For some FuncA functions, also exist Func function.
 I don't know, maybe such functions are invalid.
 If in header file such function is declared as FuncA, QM declaration should be [FuncA]Func.
 This function retrieves names of such functions.

IStringMap m=CreateStringMap(2)
m.AddList(s "")
m.EnumBegin
str sk sv sk2 ss; lpstr sv2
int n
rep
	if(!m.EnumNext(sk sv)) break
	if(!sk.end("A")) continue
	sk2.left(sk sk.len-1)
	sv2=m.Get(sk2); if(!sv2) continue
	 if(m.Get(_s.from(sk2 "W"))) continue
	out "%s (%s), %s (%s)" sk sv sk2 sv2
	sout.formata("%s[]" sk)
	n+1
out n

	