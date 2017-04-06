out
Dir d
 if d.dir("$Desktop$\D2" 2) ;;if exists
 if d.dir("$Desktop$\test_sl.txt" 2) ;;if exists
if d.dir("$Desktop$\D" 2) ;;if exists
	str fn=d.FileName
	str fp=d.FileName(1)
	int a=d.FileAttributes
	out fn
	out fp
	out "0x%X" a
	if(a&FILE_ATTRIBUTE_REPARSE_POINT) out "REPARSE_POINT"
	

