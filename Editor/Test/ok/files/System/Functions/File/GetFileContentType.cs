 /
function# $_file str&ct [flags] ;;flags: 1 don't examine file data

 Gets MIME content type (eg "image/gif") of file by examining file extension and optionally data.
 Returns: 1 success, 0 failed.

 _file - file. Can be nonexisting.
 ct - receives content type.
 
 EXAMPLE
 str s
 if(!GetFileContentType("$desktop$\test.txt" s)) s="text/plain"
 out s


word* w
int r=FindMimeFromData(0 @_file 0 0 0 0 &w 0)
if(r)
	if(flags&1) ret
	str s.getfile(_file 0 256); err ret
	r=FindMimeFromData(0 0 s s.len 0 0 &w 0)
	if(r) ret
ct.ansi(w)
CoTaskMemFree w
ret 1
