 SetCurDir "C:\windows"
str s="q:\my qm\test\test.txt"
 str s="c:\windows"
 str s.expandpath("$system$\notepad.exe")
 str s.expandpath("$desktop$\Untitled.pdf")
 str s="I:\Start.exe"
 str s="Q:\Downloads\Contig.exe"
 str s="Q:\XP.vhd"
 str s.expandpath("$documents$\My QM\main.cs")
 str s="C:"
 str s.expandpath("$my qm$")
int n=1
int what=0

PF
sel what
	case 0
	rep(n) if(GetFileAttributesW(@s)=-1) out "failed"
	 rep(n) GetFileAttributes(s)
	
	case 1
	WIN32_FILE_ATTRIBUTE_DATA a
	rep(n) if(!GetFileAttributesExW(@s 0 &a)) out "failed"
	 rep(n) GetFileAttributesEx(s 0 &a)
	
	case 2
	WIN32_FIND_DATAW f
	rep(n) _i=FindFirstFileW(@s &f); if(_i=-1) out "failed"; else FindClose(_i)
	
	case 3
	 rep(n) dir(s)
	rep(n) PathFileExists(s)
PN;PO

sel(what) case 1 out a.nFileSizeLow; case 2 out f.nFileSizeLow

 out _s.getstruct(a 1)
