function$ [$_default] [int&usedDefault] ;;"0" if _default omitted; if need empty, use N("") or NE().

 Use with number/expression fields (not text or variable with possible declaration).
 Encloses in () if need, eg if expression.
 Sets default value if empty.
 Returns this.

 usedDefault - receives 1 if used default, 0 if not.

s.trim
if s.len
	if(&usedDefault) usedDefault=0
	s.findreplace("[]" " ")
	int j=findcs(s " [9]["); if(j<0) goto gr
	if(!findrx(s "^[\-\+\!\~\&\*]*[\.0-9A-Z_a-z]*\(.*\)$")) goto gr
	if(!findrx(s "^[\-\+\!\~\&\*]*[\.0-9A-Z_a-z]+\[.+\]$")) goto gr
	s-"("; s+")"
else
	if(&usedDefault) usedDefault=1
	if(!getopt(nargs)) s="0"
	else s=_default
 gr
ret s
