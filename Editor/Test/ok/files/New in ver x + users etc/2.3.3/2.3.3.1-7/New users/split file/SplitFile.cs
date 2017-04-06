 /
function $file_ $destFolder partSizeMB

 Splits file into smaller files.
 Error if fails.

 file_ - file.
 destFolder - folder for file parts. Creates if does not exist.
 partSizeMB - part size, MB.

 REMARKS
 Error if destFolder contains files with same names as of new files. You should delete old file part files or whole folder before calling this function.


str sf1.expandpath(file_) sf2 sff.expandpath(destFolder) sfn.getfilename(sf1 1)

mkdir sff

__HFile f1 f2
f1.Create(sf1 OPEN_EXISTING GENERIC_READ FILE_SHARE_READ)

int fi bs(1024*1024) br newFile(1) ps
str sb.all(bs)

rep
	if(!ReadFile(f1 sb bs &br 0)) end "failed"
	if(!br) ret
	
	if newFile
		newFile=0
		fi+1
		sf2.format("%s\%s_%i" sff sfn fi)
		f2.Create(sf2 CREATE_NEW GENERIC_WRITE)
		ps=partSizeMB
	
	if(!WriteFile(f2 sb br &_i 0)) end "failed"
	ps-1; if(!ps) newFile=1

err+ end _error
