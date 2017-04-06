out
str s
ARRAY(str) a; int i
str rk="SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\"
RegGetSubkeys(a rk)
for i 0 a.len
	str& r=a[i]
	 out r
	str progId; if(!rget(progId "ProgId" F"{rk}{r}\UserChoice")) continue
	out F"{r}        {progId}"
	
	progId+":"
	ITEMIDLIST* pidl
	int hr=SHParseDisplayName(@progId 0 &pidl 0 0)
	if(hr) end "" 8|16 hr; continue
	
	
	
	
	CoTaskMemFree pidl
	
	 si.findreplace("''")
	 if(si="%1") continue
	 out "%-40s %s" r si
