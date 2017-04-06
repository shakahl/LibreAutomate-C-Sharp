 /
function# str&s [$lib] [tab] [noAW]
if(!s.len) ret

s.replacerx("(_In_|_Inout_|_Out_|_Out_opt_|_Reserved_)" "" 2)
AC_Prepare s 3
str ss.from(" " s "[]")
ARRAY(str) a
if(findrx(s "(?s)(\w+)\s*\((.*)\);" 0 1 a)<0) ret
if(noAW)
	if(a[1].end("A")) a[1].fix((a[1].len-1)); else if(a[1].end("W")) ret
AC_ConvertTypeNames a[2]

if(tab) s.format("	#%s %s[]" a[1] a[2].rtrim)
else
	if(!len(lib)) lib="user32"
	s.format("dll %s #%s %s[]" lib a[1] a[2].rtrim)
s+ss
ret 1
