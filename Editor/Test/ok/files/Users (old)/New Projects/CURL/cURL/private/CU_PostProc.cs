 /
function[c]# !*ptr size nmemb CUPFDATA&cd
if(cd.offset>=cd.sin.len) ret
size*nmemb
if(cd.offset+size > cd.sin.len) size=cd.sin.len-cd.offset
memcpy(ptr cd.sin+cd.offset size)
cd.offset+size
ret size
