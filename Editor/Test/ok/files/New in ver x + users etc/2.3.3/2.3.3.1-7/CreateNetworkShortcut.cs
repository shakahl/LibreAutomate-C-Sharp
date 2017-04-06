 /
function $name $target

 Creates network shortcut in $nethood$ folder.
 Error if fails.

 EXAMPLE
 CreateNetworkShortcut "test" "\\computer\sharedfolder"


str sd.expandpath(F"$NetHood$\{name}")
if(dir(sd 2)) del- sd
mkdir sd
SetAttr sd FILE_ATTRIBUTE_READONLY 1

 if flags&1 ;;FTP/HTTP. Does not work.
	 _s=F"{sd}\t.url"
	 CreateInternetShortcut _s target
	 target=_s
if(!CreateShortcut(F"{sd}\target.lnk" target)) end "failed. Probably invalid target"

str s=
 [.ShellClassInfo]
 CLSID2={0AFACED1-E828-11D1-9187-B532F1E9575D}
 Flags=2
 
s.setfile(F"{sd}\desktop.ini")

err+ end _error
