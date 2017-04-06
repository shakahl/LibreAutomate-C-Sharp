 /CtoQM
function $dll_list

str s; int i
foreach s dll_list
	i+1
	CHDLL& r=m_adll[m_adll.redim(i)]
	r.h=LoadLibrary(_s.expandpath(s))
	if(!r.h) out "dll not found: %s" s; i-1; continue
	if(findt(s 1)>0) s-"''"; s+"''"
	r.name=s
