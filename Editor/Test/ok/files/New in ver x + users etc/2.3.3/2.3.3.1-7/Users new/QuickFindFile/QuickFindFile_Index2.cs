 /
function $databaseFile $folders

 Creates database for <tip>QuickFindFile_Find2</tip>.
 Error if fails.

 databaseFile - database file. Ex: "$my qm$\x.txt"
 folders - list of folders. Will get all file and folder paths from these folders. Ex: "C:[]E:\Folder"


str s f
__HFile h.Create(databaseFile CREATE_ALWAYS GENERIC_WRITE)

foreach f folders
	f+iif(f.end("\") "*" "\*")
	Dir d
	foreach(d f FE_Dir 0x6)
		s.addline(d.FileName(1))
		if s.len>10000
			 g1
			if(!WriteFile(h s s.len &_i 0)) end _s.dllerror
			s.fix(0 1)
	if(s.len) goto g1

err+ end _error
