out
ExcelSheet es.Init

ARRAY(str) a
ARRAY(word) avt
es.CellsToArray(a "A1:C4" 0 avt)
 es.CellsToArray(a "A1:C4" 3)
int r c
for r 0 a.len
	for c 0 a.len(1)
		_s=a[c r]
		out "%s %i" _s iif(avt.len avt[c r] 0)

#ret
VT_DATE
VT_CY
VT_EMPTY
VT_R8
