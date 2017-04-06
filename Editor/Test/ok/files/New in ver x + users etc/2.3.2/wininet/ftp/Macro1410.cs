out
Ftp f.Connect2("ftp.quickmacros.com" "quickmac" "*" 0 0 1)
f.DirSet("public_html/test")

str sf
sf="$qm$\gdiplus_other.txt" ;;3166
 sf="$qm$\sqlite.txt" ;;17102
 sf="$qm$\winapi_other.txt" ;;279597

Q &q
if(!f.FilePut(sf "test.txt")) out f.lasterror
 str ss; if(sf.len) ss.getfile(sf)
 if(!f.FilePutFromStr(ss "test.txt")) out f.lasterror
Q &qq; outq
