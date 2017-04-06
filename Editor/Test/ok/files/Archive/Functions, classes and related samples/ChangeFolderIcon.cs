 /
function $folder $iconfile [iconindex]

 Changes folder icon (for example on desktop).
 Works on Windows XP and later. On older OS does nothing.

 EXAMPLE
 str folder="$desktop$\my music"
 mkdir folder
 ChangeFolderIcon folder "shell32.dll" 128


#if _winver>=0x501 ;;XP

dll shell32 [709]#SHGetSetFolderCustomSettings SHFOLDERCUSTOMSETTINGS*pfcs @*pszPath dwReadWrite

str sf.expandpath(folder) si.searchpath(iconfile)
if(!si.len) ret

SHFOLDERCUSTOMSETTINGS f.dwSize=sizeof(f)
f.dwMask=FCSM_ICONFILE
f.pszIconFile=@si
f.iIconIndex=iconindex
SHGetSetFolderCustomSettings &f @sf FCS_FORCEWRITE
