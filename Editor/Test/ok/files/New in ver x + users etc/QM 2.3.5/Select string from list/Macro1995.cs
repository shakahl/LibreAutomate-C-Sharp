out

 create list of random strings for testing
str sList sFind
rep 1000
	sFind.RandomString(10 10 "a-z")
	sList.addline(sFind)
 out sList

 create map. Time 1100 us.
Q &q
IStringMap m=CreateStringMap(1|2)
int i=1; str s
foreach(s sList) m.IntAdd(s i); i+1
Q &qq

 find string and get its index. Time 2 us.
if(!m.IntGet(sFind i)) i=0
Q &qqq
outq
out i
