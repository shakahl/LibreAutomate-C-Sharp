function$ [flags] ;;flags: 1 F"string", 2 add flag 1 if contains {...}

 Escapes text and encloses in "".
 If flags&1, interprets as text with variables. Does not escape {variable}, but replaces " with `. Prepends F.
 Does not support (variable) like S().

if s.len
	if(flags&3=2 and findrx(s "\{.+?\}")>=0) flags|1
	s.escape(iif(flags&1 17 1))

s-"''"; s+"''"
if(flags&1) s-"F"

ret s
