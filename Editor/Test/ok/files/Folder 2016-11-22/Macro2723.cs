out
str s
ARRAY(str) a; int i
RegGetSubkeys(a "" HKEY_CLASSES_ROOT)
for i 0 a.len
	str& r=a[i]
	if(r[0]='.' or r[0]='*') continue
	 out r
	str si; if(!rget(si "" F"{r}\DefaultIcon" HKEY_CLASSES_ROOT)) continue
	si.findreplace("''")
	if(si="%1") continue
	out "%-40s %s" r si
