 /
function $parentFolder $filenameRX ARRAY(str)&a [flags] ;;flags: 0 files, 1 folders, 2 any, 4 include subfolders, 8 only subfolders, 0x80000 evaluate relative path

 Gets full paths of files that are in parentFolder and whose filenames match regular expression filenameRX.
 Stores them into a.
 Evaluation is case insensitive.
 This function requires QM 2.2.0 or above.

 If used flag 0x80000, evaluated is whole relative path followed by parentFolder and \. That is, it includes subfolder names (assuming that flags 4 or 8 used). If this flag is not used, evaluated is only filename.
 Note: If used flag 0x80000, \ characters in filenameRX must be \\, because whole filenameRX is regular expression.

 EXAMPLE
 ARRAY(str) a; int i
 EnumFilesRX "$system$" "^.*\.exe$" a
 for i 0 a.len
	 out a[i]


a=0

str s.expandpath(parentFolder)
s.dospath(s 1)
if(!s.end("\")) s+"\"
s+"*"

Dir d
foreach(d s FE_Dir flags&0xffff)
	str sPath=d.FileName(1)
	lpstr ss
	if(flags&0x80000) ss=sPath+s.len-1; else ss=d.FileName
	if(findrx(ss filenameRX 0 1)>=0) a[a.redim(-1)]=sPath
