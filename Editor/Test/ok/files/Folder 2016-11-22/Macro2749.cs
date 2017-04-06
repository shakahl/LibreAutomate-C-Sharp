out
str s
ARRAY(str) a; int i
RegGetSubkeys(a "" HKEY_CLASSES_ROOT)
for i 0 a.len
	str& r=a[i]
	if(r[0]!='.') continue
	 out r
	str progId; if(!rget(progId "" r HKEY_CLASSES_ROOT) or !progId.len) continue
	out F"{r}        {progId}"
	if(RegOpenKeyExW(HKEY_CLASSES_ROOT @progId 0 KEY_READ &_i)) out "NO CLASS KEY"; continue
	RegCloseKey _i
	
	progId+":"
	ITEMIDLIST* pidl
	int hr=SHParseDisplayName(@progId 0 &pidl 0 0)
	if(hr) end "" 8|16 hr; continue
	
	
	
	
	CoTaskMemFree pidl
