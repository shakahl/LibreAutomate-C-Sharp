out
ExcelSheet es.Init

int r c
for r 0 4
	for c 0 3
		_s=es.Cell(c+1 r+1 0 _i)
		out "%s %i" _s _i

#ret
VT_DATE
VT_CY
VT_EMPTY
VT_R8
