out
dll "qm.exe" !CopyFolder $folderFrom $folderTo #flags
dll "qm.exe" !DeleteFolder $folder

 str from.expandpath("$desktop$\A")
 str to.expandpath("$desktop$\test\A")
str from="$desktop$\A"
str to="$desktop$\test\A"

int ok
ok=CopyFolder(from to 0)
if(!ok) end _s.dllerror("error: ")
3
ok=DeleteFolder(to)
if(!ok) end _s.dllerror("error: ")
