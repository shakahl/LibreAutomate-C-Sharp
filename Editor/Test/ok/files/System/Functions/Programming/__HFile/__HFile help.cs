 Stores a file handle that can be used with Windows API file read/write functions.
 The Create function can be used instead of CreateFile API.
 Automatically closes the handle when destroying the variable.

 EXAMPLES
out
str sFile="$desktop$\test\test.txt"
str s="test string"

 Creates file and writes string.
__HFile f1.Create(sFile CREATE_ALWAYS GENERIC_WRITE) ;;create file and open for write; overwrite if exists
if(!WriteFile(f1 s s.len &_i 0)) end "" 16
if(!WriteFile(f1 "[]" 2 &_i 0)) end "" 16 ;;new line
f1.Close

 Opens file and appends string.
__HFile f2.Create(sFile OPEN_ALWAYS GENERIC_READ|GENERIC_WRITE) ;;open file for read/write; create new if does not exist
LARGE_INTEGER li0; if(!SetFilePointerEx(f2 li0 0 FILE_END)) end "" 16 ;;move file pointer to the end
if(!WriteFile(f2 s s.len &_i 0)) end "" 16
if(!WriteFile(f2 "[]" 2 &_i 0)) end "" 16 ;;new line
f2.Close

 Opens file. Repeatedly reads into a memory buffer until end of file.
__HFile f3.Create(sFile OPEN_EXISTING GENERIC_READ FILE_SHARE_READ) ;;open file for read; error if does not exist
str b.all(1024 2) ;;allocate 1 KB memory buffer
int nbRead EOF
rep
	if(!ReadFile(f3 b b.len &nbRead 0)) end "" 16
	if(nbRead<b.len) b.fix(nbRead); EOF=1 ;;end of file
	if(nbRead) out b
	if(EOF) break
f3.Close
