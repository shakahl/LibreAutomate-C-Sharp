 /
function $lnkFile $iconFile [iconIndex]

 Changes shortcut icon.
 Error if lnkFile or iconFile does not exist or lnkFile is read-only.

 EXAMPLE
 ChangeShortcutIcon "$desktop$\Macro1284.lnk" "shell32.dll" 9


str slnk sico
slnk.expandpath(lnkFile)
if(!dir(slnk) or !sico.searchpath(iconFile)) end "file not found"

IShellLinkW psl
IPersistFile ppf
psl._create(CLSID_ShellLink)
ppf=+psl
ppf.Load(@slnk STGM_WRITE)
psl.SetIconLocation(@sico iconIndex)
ppf.Save(0 1)

err+ end _error
