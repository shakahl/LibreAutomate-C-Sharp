out

Ftp f.Connect2("ftp.quickmacros.com" "quickmac" "*" 0 0 1)
f.DirSet("public_html/test")
str ss

 out f.FileGet2("test.txt" "$temp$\test.txt")
 out f.FilePut2("$qm$\sqlite.txt" "test.txt")
out f.FilePut2("$qm$\winapi.txt" "test.txt")
