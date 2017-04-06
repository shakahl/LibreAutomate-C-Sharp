str folder.expandpath("$Documents$")
str files="*" ;;all
str backupFolder.expandpath("C:\Backup")

//get array of files modified in the last 24 hours. Look in subfolders too (flag 4).
str[] aFiles, aDates
DateTime t.FromComputerTime.AddParts(0, -24) ;;get time now minus 24 hours
for.file Dir d in F"{folder}\{files}", 4
	str path=d.FullPath
	//out path
	//str name=d.FileName
	DateTime tm=d.TimeModified
	if(tm<t) continue //modified earlier
	//out path
	aFiles[]=path
	aDates[].timeformat("-{yyyy.MM.dd}", tm)

//backup
if(!aFiles.len) out "0 files found"; ret
Files.CreateDirectory backupFolder
int i
for i, 0, aFiles.len
	str& r=aFiles[i]
	char* relPath=r.p+folder.len
	char* e=PathFindExtension(relPath)
	str file2.fromn(backupFolder, -1, relPath, e-relPath, aDates[i], -1, e, -1)
	out F"COPY from to:[]{r}[]{file2}"
	cop- r file2

//view results
run backupFolder
