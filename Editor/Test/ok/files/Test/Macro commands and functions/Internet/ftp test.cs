 see FtpUpload

str ss
Ftp f.Connect("ftp.quickmacros.com" "quickmac" "password")
f.DirSet("public_html")

 f.Cmd("Is index.html" 0 0)

 ss=f.Dir("index.html")

ChDir "$desktop$"
f.Cmd("LIST" ss)
 f.Cmd("PASV")
 out "---"
out ss

 ss.getfile("$desktop$\led.txt")
 f.Cmd("STOR ftptest.txt" ss 1 1)
 f.Cmd("APPE ftptest.txt" ss 1 1)

 f.Cmd("SITE chmod 644 led.txt")
