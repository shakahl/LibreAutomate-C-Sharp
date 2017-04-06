int w=win("POS Settlement File" "#32770")
Acc a.Find(w "STATICTEXT" "" "" 0x1000)
str s sn
s=a.Name
if(findrx(s "\d+" 0 0 sn)<0) end "no numbers"
int n=val(sn)
out n
