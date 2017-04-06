function# str&s $pat [flags]

if(_i) s=dir
else s=dir(pat flags); _i=1
ret s.len!=0
