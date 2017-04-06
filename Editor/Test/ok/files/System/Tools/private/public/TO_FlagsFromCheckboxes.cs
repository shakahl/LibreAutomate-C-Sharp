 /
function# ~&flags ~&c1 f1 [~&c2] [f2] [~&c3] [f3] [~&c4] [f4] [~&c5] [f5] [~&c6] [f6] [~&c7] [f7] [~&c8] [f8] [~&c9] [f9] [~&c10] [f10]

 Returns flags as integer.
 Stores flags into str variable flags. The format is like 1|2|128|0x300.

 flags - in/out string for flags. Can be 0 if not needed.
 fN - checkbox variable.
 cN - flag.


int i f ff
if(&flags) ff=val(flags)
str** pp=+(&f1-4); str& s

for i 0 10
	if(!*pp) break
	&s=*pp; pp+4; f=*pp; pp+4
	sel val(s)
		case 1 ff|f
		case -1 ff~f

if(&flags) __strt _f.Flags(ff); flags=_f
ret ff
