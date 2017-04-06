 change these
str sFile="$desktop$\new.txt" ;;large file
str destFolder="$desktop$\936543" ;;destination folder
int nLinesInFile=3000 ;;number of lines in small files
int enable=0 ;;change to 1 to create the files. By default, just displays file paths.

 ____________________________________

if(enable) mkdir destFolder
destFolder+"\new-%i.txt"

str s s1 s2 s3
s1.getfile(sFile)
int i j
foreach s s1
	s2.addline(s)
	i+1
	if(i=nLinesInFile)
		 g1
		j+1
		s3.format(destFolder j)
		out "%s (%i lines)" s3 i
		if(enable) s2.setfile(s3)
		s2=""
		i=0

if(s2.len) goto g1
