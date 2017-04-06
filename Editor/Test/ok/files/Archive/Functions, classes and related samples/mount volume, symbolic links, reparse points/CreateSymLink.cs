 /
function $symlink $target [flags] ;;flags: 1 folder

 Creates symbolic link.
 Error if fails.
 If symlink exists and is not folder, deletes.
 Symbolic links are supported on Vista and later.


dll- kernel32 !CreateSymbolicLinkW @*lpSymlinkFileName @*lpTargetFileName dwFlags
str s1.expandpath(symlink)
if(dir(symlink)) del symlink; err
str s2.expandpath(target)
if(!CreateSymbolicLinkW(@s1 @s2 flags)) end _s.dllerror
SHChangeNotify SHCNE_CREATE SHCNF_PATHW +@s1 0
