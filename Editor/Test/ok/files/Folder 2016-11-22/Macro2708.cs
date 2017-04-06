out
Dir d
foreach(d "$QM$\*" FE_Dir)
	str path=d.FullPath
	 path.GetFilenameExt; path-"."
	 out path
	str name=d.FileName
	
	int n=1000
	str icon.all(n)
	
	int hr=AssocQueryString(0 ASSOCSTR_DEFAULTICON path 0 icon &n)
	if(hr) _s.dllerror(path "" hr); out F"<><c 0xff>{_s}</c>"; continue
	icon.fix(n)
	out F"{name}    {icon}"
