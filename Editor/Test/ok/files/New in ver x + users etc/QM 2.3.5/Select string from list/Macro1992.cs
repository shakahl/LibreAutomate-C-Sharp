out

 create list of random strings for testing
str s sList sFind
rep 1000
	sFind.RandomString(10 10 "a-z")
	sList.addline(sFind)

ARRAY(str) a1=sList

 find
int i iFound1(-1) iFound2(-1) iFound3(-1) iFound4(-1)
Q &q
 foreach s sList
	 if(!StrCompare(s sFind 1)) iFound1=i; break
	 i+1
iFound2=SelStrInList(sList sFind 1)
Q &qq ;;193
 ARRAY(str) a1=sList
for i 0 a1.len
	if(!StrCompare(a1[i] sFind 1)) iFound2=i; break
Q &qqq ;;370
ICsv c._create
c.FromString(sList)
iFound3=c.Find(sFind 1)
Q &qqqq ;;300
outq
out "%i %i %i %i" iFound1 iFound2 iFound3 iFound4
