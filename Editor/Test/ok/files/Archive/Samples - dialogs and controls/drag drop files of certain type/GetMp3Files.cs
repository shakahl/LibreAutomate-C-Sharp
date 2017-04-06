 /
function# ARRAY(str)&a ARRAY(str)&amp3

 Gets mp3 files from a.
 
 Returns the number of mp3 files.
 For folders, gets all mp3 files that are in the folders, including all subfolders.

 a - files of any type, including folders.
 amp3 - receives mp3 files that are in a.


amp3=0
str s; int i
for i 0 a.len
	s=a[i]
	if(dir(s 1)) ;;folder
		s+"\*"
		Dir d
		foreach(d s FE_Dir 4)
			str sPath=d.FileName(1)
			if(sPath.endi(".mp3")) amp3[]=sPath
	else ;;file
		if(s.endi(".mp3")) amp3[]=s
ret amp3.len
