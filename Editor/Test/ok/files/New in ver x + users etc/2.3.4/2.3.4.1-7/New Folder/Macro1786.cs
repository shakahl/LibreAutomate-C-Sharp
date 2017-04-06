str sheet="north"
DATE d1 d2
d1="01/01/2011"
d2="01/01/2011"

 ______________________

ExcelSheet es.Init(sheet)
ARRAY(str) a
es.CellsToArray(a "D:D")
int i
for i 0 a.len
	DATE _d=a[0 i]
	if _d>=d1 and _d<d2
		out i
