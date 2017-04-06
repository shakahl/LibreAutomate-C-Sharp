 /CtoQM
function# $iname $fname $base str&sb

 In header files, some interface declarations contain declarations of functions of base interfaces.
 Since it is not supported in QM, need to remove such functions.
 Returns 1 if function (fname) also belongs to some of base interfaces.

sel(fname) case ["QueryInterface","AddRef","Release"] ret 1
if(!strcmp(base "IUnknown")) ret

if(!sb.len and !m_mi.Get2(base sb)) ret
 out sb

int i lenn
rep
	i=findw(sb fname i); if(i<0) break
	lenn=len(fname)
	if(sb[i+lenn]='(') ret 1
	i+lenn

str s ss
if(findrx(sb "(?<=:)\w+\b" 0 0 s)<0 or s="IUnknown") ret
if(!IsBaseMember(base fname s ss)) ret
ret 1
