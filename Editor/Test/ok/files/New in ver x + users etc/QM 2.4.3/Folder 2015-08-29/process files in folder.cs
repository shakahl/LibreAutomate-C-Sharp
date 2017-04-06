str folder="D:\KATALOG_DE"
str files="*.pdf"

ARRAY(str) a; int i
GetFilesInFolder a folder files
for i 0 a.len
	sub.File a[i]


#sub File
function str&file_

sel mes(file_ "" "YNC")
	case 'N' ret
	case 'C' end

out file_
spe 100

 Move here your macro that processes a single file.
