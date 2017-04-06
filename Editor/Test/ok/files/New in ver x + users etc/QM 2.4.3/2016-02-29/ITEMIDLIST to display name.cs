ITEMIDLIST* pidl=PidlFromStr(":: 14001F50E04FD020EA3A6910A2D808002B30309D")
 or ITEMIDLIST* pidl=PidlFromStr(_s.expandpath("$17$"))
SHFILEINFOW x
int ok=SHGetFileInfoW(+pidl 0 &x sizeof(x) SHGFI_PIDL|SHGFI_DISPLAYNAME)
CoTaskMemFree pidl
if(!ok) end "failed"
str s.ansi(&x.szDisplayName)
out s
