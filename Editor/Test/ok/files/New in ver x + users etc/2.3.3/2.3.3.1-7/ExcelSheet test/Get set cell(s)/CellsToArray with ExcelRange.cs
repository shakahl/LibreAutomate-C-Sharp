 out
ARRAY(str) a
ExcelSheet es.Init
 es.CellsToArray(a "sel" 2)
es.CellsToArray(a ExcelRange(4 1 5 4) 3)
int r c
for r 0 a.len
	out "-----Row %i-----" r+1
	for c 0 a.len(1)
		out a[c r]
