 /
function# lParam1 lParam2 lParamSort

ARRAY(str)- t_arr
str& s1=t_arr[lParamSort lParam1]
str& s2=t_arr[lParamSort lParam2]

 test for null strings to avoid exception
if(!s1.len) ret s2.len
if(!s2.len) ret -1

ret q_stricmp(s1 s2)
