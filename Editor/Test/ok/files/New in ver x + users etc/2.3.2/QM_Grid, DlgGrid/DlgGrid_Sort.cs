 /
function# i1 i2 param

 out "%i %i %i" i1 i2 param

 if(i1<i2) ret 1
 if(i1>i2) ret -1

 i1/10; i2/10

 str s1 s2
 LvGetItemText(param i1 0 s1)
 LvGetItemText(param i2 0 s2)

DlgGrid g=param
lpstr s1 s2
s1=g.CellGet(i1 0)
s2=g.CellGet(i2 0)

 ret StrCompare(s1 s2)
ret StrCompareEx(s1 s2 4)
 ret StrCompare(s1 s2 1)
 ret lstrcmp(s1 s2)
 ret StrCmpLogicalW(@s1 @s2)
