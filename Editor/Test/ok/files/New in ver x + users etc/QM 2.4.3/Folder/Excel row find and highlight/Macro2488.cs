 gets all cells in the same row as the selected cell
ExcelSheet x.Init
int row; x.GetRangeInfo("<sel>" 0 row)
ARRAY(str) a; x.CellsToArray(a ExcelRow(row))
int i
for i 0 a.len(1)
	out a[i 0]
