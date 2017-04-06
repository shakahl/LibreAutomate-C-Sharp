str icon.expandpath("$qm$\image.ico")
str sf.expandpath("$my qm$\sys")
mkdir sf
 if(!PathMakeSystemFolderW(@sf)) out _s.dllerror
SetAttr sf FILE_ATTRIBUTE_READONLY 1
str ini.from(sf "\desktop.ini")
iff ini
	SetAttr ini 0
	del- ini
str s=
F
 [.ShellClassInfo]
 IconFile={icon}
 IconIndex=0
s.unicode
s.setfile(ini)
SetAttr ini FILE_ATTRIBUTE_HIDDEN|FILE_ATTRIBUTE_SYSTEM
 IconResource={icon}
