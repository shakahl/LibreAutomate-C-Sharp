function i

dll "qm.exe" GetEsp

int ni=GetEsp
if(!i)
	out
	i=ni
else
	out i-ni
	if(i-ni>0xc000) ret

Function119 i
