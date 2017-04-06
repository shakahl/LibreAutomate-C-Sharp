 /
function $ext

 Removes a file type. Also removes all its verbs.
 Error if fails.

 ext - filename extension, eg "vig".

 REMARKS
 QM must be running as administrator.

 Added in: QM 2.3.2.


int h=HKEY_CLASSES_ROOT
str cls se=F".{ext}"

if(!sub_sys.FileType_GetClass(ext cls)) end F"{ERR_FAILED}, file type not registered"

rset "" se h 0 -2
rset "" cls h 0 -2

rset "" se "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts" 0 -2

SHChangeNotify SHCNE_ASSOCCHANGED 0 0 0
