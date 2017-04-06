function# $path

dll shell32 #SHParseDisplayName @*pszName !*pbc ITEMIDLIST**ppidl sfgaoIn *psfgaoOut

BSTR b=_s.expandpath(path)
ret !SHParseDisplayName(b 0 &pidl 0 &_i)
