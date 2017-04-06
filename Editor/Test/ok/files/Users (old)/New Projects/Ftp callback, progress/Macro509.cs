Ftp f.Connect("ftp.quickmacros.com" "quickmac" "*")
f.DirSet("public_html")
if(!f.FilePutWithProgress("$desktop$\vistarestart.exe" "vistarestart.exe" 3)) mes- "failed"
