 change these strings to match your main folder
str mainfolder="c:\main folder\"
str filepattern="c:\main folder\*.log"

 find all log files and write full paths to array
ARRAY(str) a
Dir d; str sPath
foreach(d filepattern FE_Dir 0xC)
	a[a.redim(-1)]=d.FileName(1)

 walk through the array
int i
for i 0 a.len
	str s ss sf
	sf.getfile(a[i]) ;;load file to variable
	 regular expressions
	str rx1="switch: (\d+)" ;;switch: and any number of digits
	str rx2="switch: (\d+/\d+)" ;;switch: and any number of digits with / somewhere in middle
	str rx3="dslam: (\d+-\d+-\d+)" ;;dslam: and any number of digits with two -
	 search and extract digits to s
	if(findrx(sf rx1 0 2 s 1)>=0)
	else if(findrx(sf rx2 0 2 s 1)>=0)
	else if(findrx(sf rx3 0 2 s 1)>=0)
	else continue ;;not found
	 found
	s.replacerx("[\/|<>?*:'']+") ;;remove bad characters
	s-mainfolder ;;make full path
	mkdir s ;;create new folder
	ss.getpath(a[i] "") ;;get folder containing the log file
	ren ss s ;;move the folder to the new folder
	
	