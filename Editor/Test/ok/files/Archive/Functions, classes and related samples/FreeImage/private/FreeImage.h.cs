out
 ConvertCtoQM "$desktop$\freeimage.h" "$desktop$\freeimage.txt" "" "" 0 "$qm$\winapiqmaz_fdn.txt" "$qm$\winapiqmaz_fan.txt" "$qm$\winapiv_pch.txt"

 note: will be 2 errors because for function typedef not used func return type (int assumed)
 note: replace red_mask'a etc to red_mask etc.
 note: FreeImage functions are exported like _Func@nn. Use this code to convert ref function names from Func to [_Func@nn]Func:

#ret
str ss=
 copy/paste all _FreeImage_x@nn function names from depends
str sm.getmacro("ref_FreeImage") ;;or getfile("$desktop$\freeimage.txt")
str s1 s2
foreach s1 ss
	s2.gett(s1 0 "@")
	s2.get(s2 1)
	 out s2
	if(sm.replacerx(F"'' (.*{s2})" F"'' [{s1}]$1" 4)<0)
		out "no %s" s1 ;;several exported functions are not in the h file
out sm
