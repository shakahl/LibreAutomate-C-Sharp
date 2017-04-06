 out
Ftp f.Connect("ftp.quickmacros.com" "quickmac" "*")
f.DirSet("public_html")

 f.DirNew("ėįš")
 f.DirSet("ėįš")
 f.DirGet(_s); out _s
 f.DirDel("ėįš")

 f.FileRen("ąčę.html" "žūų.html")
 f.FileDel("žūų.html")

 f.FilePut("$desktop$\ąčę.html" "ąčę.html")

 f.FileGet("index.html" "$desktop$\ąčę.html")
 f.FileGet("ąčę.html" "$desktop$\ąčę2.html")

 out f.Cmd("SIZE test.txt")
 ret
 
 f.Cmd("PASV")
 out f.Cmd("RETR index.html" _s)
 out _s

 f.Cmd("SIZE index.html" _s)
 out _s
 ret
 
 str ss.getfile("$desktop$\test.txt")
 f.Cmd("STOR ftptest.txt" ss 0 1)

 f.Disconnect
 f.Connect("ftp.quickmacros.com" "quickmac" "*")
 f.DirSet("public_html")
 
 ss="append this"
 f.Cmd("APPE ftptest.txt" ss 0 1)

str s
if(f.Cmd("LIST" &s)) out s

 f.Cmd("SITE chmod 647 test.txt")
