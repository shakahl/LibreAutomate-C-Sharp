 /
function# str&filePath $folder [$filenamePattern] [flags] [flags2] [DateTime&getTime] ;;flags: 0 files, 1 folders, 2 any, 4 +subfolders, 8 only subfolders, 32 skip symbolic-link subfolders, 64 skip hidden-system, 0x10000 regular expression, 0x20000 compare relative path.

 Gets full path of the newest (most recently modified) or oldest file in a folder.
 Returns the number of matching files in the folder.

 filePath - variable that receives file path.
 folder - folder where to look for files.
 filenamePattern - if used and not "", looks for files that match filenamePattern. Else looks for all files.
   Can be filename with <help #IDP_WILDCARD>wildcard characters</help> (*?), or regular expression (flag 0x10000).
   Can be relative path (flag 0x20000).
 flags - same as with <help>GetFilesInFolder</help>.
 flags2: 1 get the oldest file.
 getTime - optional variable that receives file time, UTF.

 REMARKS
 Uses <help>GetFilesInFolder</help>. Parameters folder, filenamePattern and flags are the same.

 EXAMPLES
 str sPath
 if GetMostRecentFileInFolder(sPath "$documents$")
	 out F"newest file:  {sPath}"
 
 DateTime t
 str sPath2
 if GetMostRecentFileInFolder(sPath2 "$documents$" "" 0 1 t)
	 out F"oldest file:  {sPath2}  ({t.ToStr(4)})"


opt noerrorshere 1
ARRAY(str) a
GetFilesInFolder a folder filenamePattern flags 2
if(!a.len) filePath.all; ret
a.sort(iif(flags2&1 8 9))
DateTime t=val(a[0] 1 _i) ;;use DateTime for times. Use long for size.
if(&getTime) getTime=t
filePath=a[0]+_i+1
ret a.len
