str lnkFile.expandpath("$desktop$\notepad.lnk")
str targetFile.expandpath("$system$\notepad.exe")

 create shortcut
if(!CreateShortcut(lnkFile targetFile)) ret

 set 'run as admin'
BSTR blf=lnkFile
IShellLinkW sl._create(CLSID_ShellLink)
IPersistFile pf=+sl
pf.Load(blf STGM_READ|STGM_WRITE)
IShellLinkDataList dl=+sl
dl.GetFlags(&_i)
if(_i&SLDF_RUNAS_USER) ret
dl.SetFlags(_i|SLDF_RUNAS_USER)
pf.Save(blf 1)

  note: it sets 'run as admin' only for the shortcut, not for the exe, therefore the checkbox in shortcut Properties is not checked, and no shield overlay on icon, and if you double click exe or other shortcut to it, it will not run as admin
