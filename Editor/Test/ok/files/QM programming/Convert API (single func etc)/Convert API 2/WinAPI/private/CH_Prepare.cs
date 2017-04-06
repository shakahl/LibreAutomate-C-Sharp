 /Macro18
function str&s
int i j
 replace possible \n to \r\n
i=findc(s 10 i)
if(i>=0 and (!i || s[i-1]!=13)) s.findreplace("[10]" "[]")
 remove comments
rep
	i=findc(s '/' i); if(i<0) break
	if(s[i+1]='/' and !CH_IsString(&s i))
		j=findc(s 13 i)
		s.remove(i j-i)
		if(j<0) break
	else if(s[i+1]='*' and !CH_IsString(&s i))
		j=find(s "*/" i)
		s.remove(i j-i+2)
		if(j<0) break
	else i+1
 join broken lines
s.findreplace("\[]" " ")
 remove empty lines
s.findreplace("[][]" "[]" 8)
