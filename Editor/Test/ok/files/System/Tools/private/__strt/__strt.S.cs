function$ [$_default] [int&isVariable]

 Use with text fields, when variable/expression can be specified as (var).
 If variable, strips () etc, else escapes and adds "".
 Sets default value if empty.
 Returns this.

if !s.len
	if(empty(_default)) s="''''"; else s=_default; goto g1
else if findrx(s "^\( *(.+) *\)$" 0 0 _s 1)>=0
	s=_s
	_i=1
else
	 g1
	s.escape(1); s-"''"; s+"''"

if(&isVariable) isVariable=_i
ret s
