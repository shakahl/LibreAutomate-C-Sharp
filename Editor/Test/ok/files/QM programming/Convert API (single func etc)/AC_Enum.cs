
 Before running, replace all defs to numbers.

str s.getsel sss ss
AC_Prepare &s
if(findrx(s "(?s)(?<={).+(?=})" 0 0 ss)<0) ret
ss.trim
int i j value(-1)
for i 0 1000000
	if(s.getl(ss -i)<0) break
	s.trim(" 	,"); if(!s.len) continue
	j=findc(s '=')
	if(j>=0) value=val(s+j+1); s.fix(j); s.rtrim; else value+1
	if(value<256) sss.formata("def %s %i[]" s value)
	else sss.formata("def %s 0x%X[]" s value)
	
CH_Compact sss &ss
ss.setsel
