out

str filetypes=
 +*
 +Directory
 +Drive
 +InternetShortcut
 lnkfile
 Folder
 AllFileSystemObjects
 +Directory\Background

 *
 Directory
 Drive
 InternetShortcut
 lnkfile
 Folder
 AllFileSystemObjects
 Directory\Background

 *
 Directory
 +Drive
 InternetShortcut
 lnkfile
 Folder
 +AllFileSystemObjects
 +Directory\Background


str s
foreach s filetypes
	if s[0]='+'
		rset "{C00E2DB5-3AF8-45a6-98CB-73FCDE00AC5C}" "" F"{s+1}\shellex\ContextMenuHandlers\QM" HKEY_CLASSES_ROOT
	else
		rset "" "QM" F"{s}\shellex\ContextMenuHandlers" HKEY_CLASSES_ROOT -2

SHChangeNotify SHCNE_ASSOCCHANGED 0 0 0
