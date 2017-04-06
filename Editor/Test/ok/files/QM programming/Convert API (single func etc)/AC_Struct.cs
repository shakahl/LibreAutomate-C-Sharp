str s.getsel; if(!s.len) ret
AC_Prepare &s 7
ARRAY(str) a
if(s.beg("typedef"))
	if(findrx(s "(?s){(.*)\}\s*(\w+)" 0 1 a)<0) ret
	AC_ConvertTypeNames a[1]; a[1].trim
	s.format("type %s %s[]" a[2] a[1])
else	
	if(findrx(s "(?s)struct\s+(\w+)\s*\{(.*)\}" 0 1 a)<0) ret
	AC_ConvertTypeNames a[2]; a[1].trim
	s.format("type %s %s[]" a[1] a[2])
s.setsel
