out
_s.expandpath("$my qm$\test\ok.db3")
RunConsole2 F"Q:\Downloads\Contig.exe ''{_s}''"
0.5

PF
__HFile f.Create(_s OPEN_EXISTING GENERIC_READ)
 __HFile f.Create(_s OPEN_EXISTING GENERIC_READ 0 FILE_FLAG_SEQUENTIAL_SCAN)
 __HFile f.Create(_s OPEN_EXISTING GENERIC_READ 0 FILE_FLAG_RANDOM_ACCESS)
int n=GetFileSize(f 0)
PN

rep 2
	 _s.all(n)
	 out ReadFile(f _s n &_i 0)
	
	 int N=4096
	 _s.all(N)
	 rep n/N
		 if(!ReadFile(f _s N &_i 0)) end "failed"
	
	int N=4096
	_s.all(N)
	int i
	for i 1 n/N
		 out i*N
		if(SetFilePointer(f -i*N 0 FILE_END)=-1) end "failed"
		 out SetFilePointer(f -i*N 0 FILE_END)
		if(!ReadFile(f _s N &_i 0)) end "failed"
		 out Crc32(_s N)

PN
PO
