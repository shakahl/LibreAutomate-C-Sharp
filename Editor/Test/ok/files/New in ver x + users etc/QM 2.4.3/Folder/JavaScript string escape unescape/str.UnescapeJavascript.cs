function$

 Replaces JavaScript string escape sequences.
 Returns self.


if(!this.len) ret
REPLACERX r.frepl=&sub.Callback
replacerx("\\([\\'''rntbfv]|u\w\w\w\w)" r)

ret this


#sub Callback
function# REPLACERXCB&x

str& s=x.match
sel s[1]
	case 34 s="''"
	case 39 s="'"
	case '\' s="\"
	case 'r' s="[13]"
	case 'n' s="[10]"
	case 't' s="[9]"
	case 'b' s="[8]"
	case 'f' s="[12]"
	case 'v' s="[11]"
	case 'u' _i=strtoul(s+2 0 16); lpstr t=+&_i; s.ansi(t)
