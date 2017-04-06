 /
function# $_file [long&size] [DateTime&timeModified] [DateTime&timeCreated] [DateTime&timeAccessed]

 Gets standard file properties: attributes, size, times.
 Returns <google "file attribute constants ''FILE_ATTRIBUTE_READONLY''">attributes</google>. It is a combination of FILE_ATTRIBUTE_ flags. See example.
 Error if failed or if the file does not exist.

 _file - file path. Can be folder.
 size - variable that receives file size. Can be 0 if don't need.
   For folders, calls <help>GetFileOrFolderSize</help> to calculate folder content size.
 timeModified, timeCreated, timeAccessed - variables that receive file time, UTC. Can be 0 if don't need.
   You can call <help>DateTime</help> class functions to convert the UTC time to local time, string or other format. See example.
   Note: on most computers Windows doesn't auto-update the last access time.

 REMARKS
 This function calls Windows API function <google "GetFileAttributesEx">GetFileAttributesEx</google>.
 To create code for this function, use dialog "Get file info".
 When enumerating files with foreach FE_Dir, instead use <help>Dir</help> class functions. To create code, use dialog "Enumerate files".

 See also: <FileSetAttributes>.
 Added in: QM 2.4.1. Replaces GetAttr.

 EXAMPLES
 int attr=FileGetAttributes("$documents$\test.txt"); err out _error.description; ret
 if(attr&FILE_ATTRIBUTE_DIRECTORY) out "is folder"
 if(attr&FILE_ATTRIBUTE_READONLY) out "is read-only"
 
 long size; DateTime t
 FileGetAttributes("$qm$\qm.exe" size 0 t); err out _error.description; ret
 t.UtcToLocal; str st=t.ToStr(4)
 out "size=%I64i created=%s" size st


WIN32_FILE_ATTRIBUTE_DATA x
if(!GetFileAttributesExW(@_s.expandpath(_file) 0 &x)) end F"{ERR_FAILED} to get file attributes" 16

if &size
	if(x.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY) size=GetFileOrFolderSize(_file)
	else size=x.nFileSizeHigh<<32|x.nFileSizeLow
if(&timeModified) memcpy &timeModified &x.ftLastWriteTime 8
if(&timeCreated) memcpy &timeCreated &x.ftCreationTime 8
if(&timeAccessed) memcpy &timeAccessed &x.ftLastAccessTime 8

ret x.dwFileAttributes
