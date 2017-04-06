out
sub.GetIconPaths "QM toolbar"
sub.GetIconPaths "TB Main"
sub.GetIconPaths "misc tools"


#sub GetIconPaths
function $macro

str s.getmacro(macro)
ARRAY(str) a; int i
if findrx(s " :run ''(.+?)''" 0 4 a 1)
	for(i 0 a.len) out a[0 i]
if findrx(s " \* *([^\r\n]{4,})" 0 4 a 1)
	for(i 0 a.len)
		a[0 i].replacerx("(?m) \* *(?=\d+$)" ",")
		out a[0 i].trim("''")
