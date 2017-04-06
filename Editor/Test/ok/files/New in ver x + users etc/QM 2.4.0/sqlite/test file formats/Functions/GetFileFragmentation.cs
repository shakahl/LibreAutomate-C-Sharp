 /
function# $_file

 Returns number of fragments in disk used to store _file.


opt noerrorshere 1
__HFile f.Create(_file OPEN_EXISTING FILE_READ_ATTRIBUTES FILE_SHARE_READ)

STARTING_VCN_INPUT_BUFFER ib
int nr n=4096
rep
	RETRIEVAL_POINTERS_BUFFER* rb=_s.all(n)
	if(DeviceIoControl(f FSCTL_GET_RETRIEVAL_POINTERS &ib sizeof(ib) rb n &nr 0)) ret rb.ExtentCount
	if(GetLastError!=ERROR_MORE_DATA) end _s.dllerror
	n*2

 note: for small files may be error "Reached the end of the file".
 note: on MS Virtual PC, XP, if the file is on host PC, error "incorrect function".
