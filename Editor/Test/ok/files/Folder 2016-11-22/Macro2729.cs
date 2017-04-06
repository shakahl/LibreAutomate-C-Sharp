out
Dir d
foreach(d "$qm$\*" FE_Dir)
	str path=d.FullPath
	out path
	sub.GetIcon(path)


#sub GetIcon
function# $s

ITEMIDLIST* pidl
if(SHParseDisplayName(@s 0 &pidl 0 0)) ret

 int shFlags=SHGFI_ICONLOCATION|SHGFI_PIDL
 SHFILEINFOW x
 if(!SHGetFileInfoW(+pidl 0 &x sizeof(x) shFlags)) ret
 outb &x.szDisplayName 260 1
 out _s.ansi(&x.szDisplayName)
