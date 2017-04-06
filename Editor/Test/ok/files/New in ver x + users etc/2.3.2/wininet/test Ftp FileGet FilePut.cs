out

Ftp f.Connect("ftp.quickmacros.com" "quickmac" "[*E734F9FC63ABA65014539038244DDD2602*]" 0 0 1)
f.DirSet("public_html/test")

str ss sf
 sf="$qm$\winapiv.txt"
sf="$qm$\qm.exe"

f.SetProgressDialog(1)
f.SetProgressCallback(&wininet_cb 7)

 goto get
 goto cmd

 ss="test data"
ss.getfile(sf)
f.FilePut(sf "test.txt")
f.FilePutStr(ss "test.txt")
ret

 get
str s
out f.FileGet("test.txt" "$temp$\test.txt")
out f.FileGetStr("test.txt" s)
 out f.FileGetStr("index.htm" s)
out s.len


 cmd
 str sc
 out f.Cmd("RETR test.txt" sc)
 out sc

 
