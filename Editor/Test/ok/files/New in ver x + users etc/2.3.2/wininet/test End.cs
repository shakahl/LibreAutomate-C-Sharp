out
str s rh

#if 0

Http h
mac "internet_End_Thread" "" &h

h.Connect("www.quickmacros.com")
h.SetProgressDialog(1)
out h.Get("quickmac.exe" s 0 0 rh)
out h.lasterror

out s.len
out rh

#else

Ftp f
mac "internet_End_Thread" "" 0 &f

f.Connect("ftp.quickmacros.com" "quickmac" "*" 0 0 1)
f.DirSet("public_html")
f.SetProgressDialog(1)

out f.FileGetStr("quickmac.exe" s)
out f.lasterror
out s.len
