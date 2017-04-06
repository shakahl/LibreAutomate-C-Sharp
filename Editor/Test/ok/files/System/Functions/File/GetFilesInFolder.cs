 /
function ARRAY(str)&a $parentFolder [$filenamePattern] [flags] [getProp] [$exceptPattern] ;;flags: 0 files, 1 folders, 2 any, 4 +subfolders, 8 only subfolders, 32 skip symbolic-link subfolders, 64 skip hidden-system, 0x10000 filenamePattern is regexp, 0x20000 filenamePattern is relative path, 0x40000 exceptPattern is regexp, 0x80000 exceptPattern is relative path.  getProp: 1 size, 2 time modified, 3 time created, 4 time accessed, 0x100 (flag) don't need folder size

 Gets full paths of files in a folder.

 a - variable for results.
 parentFolder - full path of the folder. Can end with \.
 filenamePattern - if used and not "", gets only files that match filenamePattern. Else gets all files.
   Can be full filename, filename with <help #IDP_WILDCARD>wildcard characters</help> (*?), or regular expression (flag 0x10000).
   Can be relative path (flag 0x20000).
   Case insensitive.
   QM 2.4.4: can be list, like "*.png[]*.gif[]*.jpg".
 flags:
   0, 1, 2 - file/folder type. If 0, gets only files. If 1 - only folders. If 2 - all.
   4 - also get files in subfolders.
   8 - get files only in subfolders, not in this folder.
   32 (QM 2.4.0) - don't get files in subfolders that have attribute FILE_ATTRIBUTE_REPARSE_POINT. It is used for symbolic links, junctions and volume mount points, that actually are not subfolders of that folder.
   64 (QM 2.4.0) - don't get hidden system files and folders (eg thumbnail cache files).
   0x10000 - filenamePattern is regular expression. Note: if need \ characters (if flag 0x20000), use \\.
   0x20000 - filenamePattern is path relative to parentFolder. Example: "subfolder\*\*.txt". Will be compared with whole relative paths of files, not just with filenames.
   0x40000 - like 0x10000 but is used with exceptPattern.
   0x80000 - like 0x20000 but is used with exceptPattern.
 getProp (QM 2.3.5) - prepend a file property to the path (in a), like "1000 c:\file.txt".
   File size is in bytes.
   File times are in FILETIME/DateTime format, UTC.
   To sort the array by a file property (or just by name, if getProp not used): a.sort(8)
   To get the number and path string: see example.
   File access time auto-updating on most computers is disabled, therefore it can be useful only if set explicitly.
 exceptPattern (QM 2.4.4) - if used and not "", skips files that match exceptPattern.
   Everything like with filenamePattern, but are used other flags (0x40000/0x80000).

 See also: <FE_Dir>.

 EXAMPLES
 ARRAY(str) a; int i
 GetFilesInFolder a "$desktop$"
 a.sort(8)
 for i 0 a.len
	 out a[i]
	 Dir d.dir(a[i])
	 out d.FileSize

 GetFilesInFolder a "$system$" "*.exe"
 GetFilesInFolder a "$system$" "^.*\.exe$" 0x10000

  get file time and path, sort by file time, and display all in readable format
 ARRAY(str) a; int i
 GetFilesInFolder a "$my qm$" "" 0 2
 a.sort(8)
 for i 0 a.len
	 DateTime t=val(a[i] 1 _i) ;;use DateTime for times. Use long for size.
	 t.UtcToLocal
	 str sPath=a[i]+_i+1
	 out F"{t.ToStr(4)}    {sPath}"


a=0

str s.expandpath(parentFolder)
s.dospath(s 1)
s+iif((s.end("\")||!s.len) "*" "\*")
ARRAY(str) afp aep
if(!empty(filenamePattern)) afp=filenamePattern
if(!empty(exceptPattern)) aep=exceptPattern
int getPropFlags=getProp&~255; getProp&255

Dir d; lpstr sPath ss; int i
foreach(d s FE_Dir flags&0xff)
	if(afp.len and !sub.IsMatch(iif(flags&0x20000 d.RelativePath d.FileName) afp flags&0x10000)) continue
	if(aep.len and sub.IsMatch(iif(flags&0x80000 d.RelativePath d.FileName) aep flags&0x40000)) continue
	
	sPath=d.FullPath
	if getProp
		long x
		sel getProp
			case 1 if(getPropFlags&0x100 and d.IsFolder) x=0; else x=d.FileSize
			case 2 x=d.TimeModifiedUTC
			case 3 x=d.TimeCreatedUTC
			case 4 x=d.TimeAccessedUTC
			case else end ERR_BADARG
		a[].from(x " " sPath)
	else a[]=sPath

err+ end _error ;;error in rx


#sub IsMatch
function! $ss ARRAY(str)&a isRX

int i
for i 0 a.len
	if(isRX) if(findrx(ss a[i] 0 1)>=0) ret 1
	else if(matchw(ss a[i] 1)) ret 1
