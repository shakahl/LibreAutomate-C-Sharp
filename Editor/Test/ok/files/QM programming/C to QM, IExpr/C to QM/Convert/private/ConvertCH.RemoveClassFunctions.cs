 /CtoQM
function! str&s

 removes function declarations in class. Bodies ({...}} must be already replaced with ;.

 out s
s.replacerx("\(operator \S+\)\(" "F(")
int r=s.replacerx("((?<=[;:])|^)(\w+[ \*&]*|~|)(\w+::)?(\w+|operator\W+)\(.*?\);")
if(!r) ret
 out "rem decl: %s" s
ret 1
