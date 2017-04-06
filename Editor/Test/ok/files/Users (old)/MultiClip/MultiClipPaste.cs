ARRAY(str)+ g_mca
int i=g_mca.len-1
if(i<0) ret
str& s=g_mca[i]
s.setsel
g_mca.redim(i)
