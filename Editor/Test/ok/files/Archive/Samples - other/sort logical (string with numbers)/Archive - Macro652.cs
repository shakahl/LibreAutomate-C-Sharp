out
str s=
 ab1234/3:10
 ab1234/3:9
 bc3234/2:3
 ch8218/5:2
 ab1234/3:1
 bc3234/2:2
ARRAY(str) a=s
 a.shuffle

a.sort(0 sort_proc_str_logical 0)
out a
