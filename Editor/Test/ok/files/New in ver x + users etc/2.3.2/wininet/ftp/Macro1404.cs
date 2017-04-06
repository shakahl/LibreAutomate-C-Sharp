 Ftp f.Connect("ftp.quickmacros.com" "quickmac" "*")
 f.DirSet("public_html/test")
 str ss="Hello World"
 int i=f.Cmd("STOR WLPO/TestZone/test.txt" ss 1 1)
 out i

out

Ftp f.Connect2("ftp.quickmacros.com" "quickmac" "*" 0 0 1)
f.DirSet("public_html/test")
 out f.FileDel("test.txt")
str ss
 goto g1
ss.getfile("$qm$\gdiplus_other.txt") ;;3166
 ss.getfile("$qm$\sqlite.txt") ;;17102
 ss.getfile("$qm$\winapi_other.txt") ;;279597
 ss.all
 out f.Dir("test.txt")
out f.Cmd("STOR test.txt" ss 0 1)
 out f.Cmd("STOR test.txt" ss 0 1)
 out f.Cmd("NOOP")
 out f.Dir("test.txt")
ret


 g1
 out f.Dir("test.txt")
 out f.Cmd("RETR test.txt" ss)
out f.Cmd("LIST" ss)
 out f.FileGet("test.txt" "$temp$\test.txt")
 out f.FilePut("$temp$\test.txt" "test.txt")
 _s="data"; out f.FilePutFromStr(_s "test.txt")
 out f.Dir("test.txt")
out ss.len
