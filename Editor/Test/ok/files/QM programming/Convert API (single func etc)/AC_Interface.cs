str s.getsel()
if(!s.len) ret
AC_Prepare &s 2

ARRAY(str) a b
str guid
if(findrx(s "MIDL_INTERFACE\(''([^'']+)" 0 0 b)>=0)
	guid=b[1]
	s.getl(s 1 2)

if(findrx(s "(?s)(\w+)\s*:\s*public\s+(\w+)*\s+{(.+)}" 0 0 b)<0) ret

s.format("interface %s" b[1])
if(b[2].len) s.formata(" :%s" b[2])
s+"[]"

findrx(b[3] "(?s)\w+\s*\(.*?\)" 0 4|16 a)
int i
for(i 0 a.len)
	a[0 i].replacerx("\s{2,}" " ")
	a[0 i].findreplace("( " "(")
	AC_ConvertTypeNames(a[0 i])
	s+"[9]"; s+a[0 i]; s+"[]"
	
if(guid.len) s.formata("[9]{%s}[]" guid)

s.setsel
 out "%i members found" a.len