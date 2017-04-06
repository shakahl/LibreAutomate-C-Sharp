 out
str p; GetEnvVar "PATH" p
out p; ret
p-"C:\Program Files\7-Zip\;"
SetEnvVar "PATH" p
out p
 ret

str s="7zfm.exe"
out _s.searchpath(s)
run s
