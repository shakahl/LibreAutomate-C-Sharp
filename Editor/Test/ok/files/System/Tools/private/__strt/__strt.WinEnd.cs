function str&code [str&winFind] [flags] ;;flags: 1 no comments

 Call for window/control selector control variable (this) after formatting code.
 Prepends winFind. If winFind not used, prepends "int w=win(...)" line from this var, if available.
 Inserts control comments into code.
 Does nothing if Win() not called for this variable, or if screen/none.

if(s.flags&128=0) ret
ARRAY(str) a=s
if a[3].len and !(flags&1)
	if(findc(code 10)<0) code+a[3] ;;regex would fail if eg a[1] is "0" if code trimmed
	else code.replacerx(F"\b\Q{a[1]}\E[^[]'']*$" F"$0{a[3]}" 4|8)
if(&winFind) code-winFind; else if(a[2].len) code=F"{a[2]}[]{code}"
