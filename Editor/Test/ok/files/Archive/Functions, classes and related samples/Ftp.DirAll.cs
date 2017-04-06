 /
function $pattern ARRAY(STRINT)&a [flags] ;;flags: 0 files, 1 folders, 2 both, 4 include subfolders

 Gets names of files in an ftp folder and optionally in subfolders.

 pattern - filename with wildcard characters. Cannot contain spaces.
   Examples: "*" (all in current ftp directory), "" (the same), "*.htm", "abc/*" (relative to current ftp directory), "/abc/*" (full path).
 a - receives filenames and paths (relative or full, depending on pattern) in a[?].s. For folders, a[?].i will be 1, for files - 0.

 EXAMPLE
 out
 Ftp f.Connect("ftp.xxx.com" "********" "********")
 f.DirSet("public_html")
 ARRAY(STRINT) a
 f.DirAll("*" a 2)
 int i
 for i 0 a.len
	 out "%s%s" a[i].s iif(a[i].i " (folder)" "")


a=0
int f=flags&3
if(f=3) end ES_BADARG
if(flags&4 and f=0) f=2
if(empty(pattern)) pattern="*"
str sp.getpath(pattern)

lpstr s=Dir(pattern f)
rep
	if(!s) break
	sel(s) case [".",".."] goto g1
	int isf=m_fd.fd.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY!=0
	
	if((isf and f) or (!isf and f!=1))
		STRINT& r=a[]
		r.s.from(sp s)
		r.i=isf
	 g1
	s=Dir

if(flags&4=0) ret

int i j
for i 0 a.len
	&r=a[i]
	if(!r.i) continue
	str sp2.from(r.s "/" pattern+sp.len+(sp.len!0))
	ARRAY(STRINT) a2
	DirAll(sp2 a2 flags)
	for(j 0 a2.len)
		&r=a2[j]
		if((r.i and flags&3) or (!r.i and flags&3!=1)) a[]=r

if(flags&3=0) for(i a.len-1 -1 -1) if(a[i].i) a.remove(i)
