 /CtoQM
function! str&s

 case "*(*){*}*" ;;inline function definition, or class containing inline functions

int is=SelStr(4 s "typedef " "struct " "_struct_from_class ")
if(!is or !s.end(";")) ret

 out s

int i1 i2
i1=findrx(s "\w+ \w+\{(.+)\}" (is=1)*8 0 i2 1)
if(i1<0) ret
 out i1
str ss.get(s i1 i2)
rep
	int i=findc(ss '{' i); if(i<0) break
	if(ss[i-1]!=')') ret ;;class in class
	lpstr s2=SkipEnclosed(ss+i '}'); if(!s2) ret
	ss.replace(";" i s2-ss-i) ;;func definition to declaration

s.replace(ss i1 i2)
 out "rem body: %s" s
ret 1
