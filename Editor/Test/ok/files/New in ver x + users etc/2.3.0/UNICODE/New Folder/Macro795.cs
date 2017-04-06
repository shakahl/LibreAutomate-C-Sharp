Ftp f.Connect("ftp.quickmacros.com" "quickmac" "*")
f.DirSet("public_html")

 out f.Dir("index.html")
 out f.Dir("/public_html/index.html")

lpstr s=f.Dir("*")
rep
	if(!s) break
	out s
	s=f.Dir
