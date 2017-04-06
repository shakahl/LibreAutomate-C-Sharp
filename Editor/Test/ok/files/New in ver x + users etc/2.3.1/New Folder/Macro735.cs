out
Ftp f.Connect("ftp.quickmacros.com" "quickmac" "*")
 f.DirSet("public_html")
f.DirSet("public_html/test folder")
 f.DirSet("public_html/reg")
 f.DirSet("public_html/help")
ARRAY(STRINT) a
 f.DirAll("*" a 6)
 f.DirAll("public_html/*" a)
 f.DirAll("/public_html/*" a)
 f.DirAll("/public_html/*.txt" a)
 f.DirAll("public_html/help/*" a 0|4)
 f.DirAll("/public_html/forum/*" a 6)
 f.DirAll("h*/*" a 0)
f.DirAll("*" a 2|4)
int i
for i 0 a.len
	out a[i].s
	