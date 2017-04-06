act "POS Settlement File"
str s sn
s.getsel
 out s
if(findrx(s "\d+" 0 0 sn)<0) end "no numbers"
int n=val(sn)
out n
