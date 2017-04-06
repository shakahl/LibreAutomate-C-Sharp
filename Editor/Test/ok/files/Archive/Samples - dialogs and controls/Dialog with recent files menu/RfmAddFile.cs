 /
function $_file $regkey $regname nmax

str s; int i
rget s regname regkey
ARRAY(str) a=s ;;list -> array
for(i a.len-1 -1 -1) if(a[i]~_file) a.remove(i) ;;remove duplicates
rep() if(a.len>=nmax) a.remove(0); else break ;;limit number of items
a[]=_file ;;add _file
s=a ;;array -> list
rset s regname regkey
