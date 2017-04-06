 /
function $databaseFile $filePattern ARRAY(str)&results

 Finds files in database created by <tip>QuickFindFile_Index2</tip>.
 Error if fails.

 databaseFile - database file.
 filePattern - file pattern. Must match full path. Examples: "*.txt", "C:\*.txt", "C:\Folder\*", "*\file.txt".
 results - receives full paths of found files and folders.


str s f
int na(65000) nr nrTotal

results=0

__HFile h.Create(databaseFile OPEN_EXISTING GENERIC_READ FILE_SHARE_READ)
rep
	if(!ReadFile(h s.all(na) na &nr 0)) end _s.dllerror
	if(!nr) break
	s.fix(nr)
	s.fix(findcr(s 10)+1)
	
	foreach f s
		if(matchw(f filePattern 1)) results[]=f
	
	nrTotal+s.len
	SetFilePointer h nrTotal 0 0

err+ end _error
