 /
function'str $filename [~folder] [$dateformat]

 Formats full path from 3 parts: folder, current date and filename.
 filename can be either full path or file name.
 If folder is not specified, extracts from filename or uses temp folder.
 dateformat is the same as with str.time.

 EXAMPLE
 str f="c:\d.txt"
 ren f FormatTempFileName(f "$temp$")


if(folder.len) folder.expandpath; if(!folder.end("\")) folder+"\"
else folder.getpath(filename); if(!folder.len) folder.expandpath("$temp$\")

if(!len(dateformat)) dateformat="%m%d%Y%H%M "
str TM.time(dateformat)

str s.from(folder TM _s.getfilename(filename 1))
ret s
