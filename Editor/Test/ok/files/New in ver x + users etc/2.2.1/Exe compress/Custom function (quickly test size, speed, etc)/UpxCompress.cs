 /
function $path [$cl]

str s.expandpath(path)
ChDir _s.getpath(s "")
 out CurDir
s.getfilename(s 1)
 s+" --force"
 s+" -1"
 s+" --best"
 s+" --lzma" ;;190 KB, fast enough
 s+" --lzma --brute" ;;189 KB
s+" --compress-exports=0"
 s+" --brute" ;;189 KB (default 201), but takes very much time. Uses lzha.
if(len(cl)) s+" "; s+cl
 out s
 s+" --help"
int t1=perf
RunConsole("$qm$\upx300.exe" s)
int t2=perf
out t2-t1
ret 1

 Tried various upx versions:
 1.xx are smaller (1.24 is 92 KB). Works only if --force used.
 2.xx are < 200 KB. Works always.
 3.00 is 255 KB. Works always. Also has lzha compression (makes 12 KB smaller), but it is not useful because much slower.

 It is best to use default options. Only add --compress-exports=0, because without it does not work on wine (I did not test).
