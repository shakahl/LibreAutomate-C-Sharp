 finds and highlights text in cells in the same row as the selected cell
str textToFind="e"
ExcelSheet x.Init
int row; x.GetRangeInfo("<sel>" 0 row)
ARRAY(Excel.Range) a
if x.Find(textToFind a 2|4 ExcelRow(row))
	int i
	for i 0 a.len
		Excel.Range& r=a[i]
		out F"col={r.Column} row={r.Row}"
		 color
		EXCELFORMAT f.cellColor=0xe0ff
		x.Format(ExcelRange(r.Column r.Row) f)
	x.Activate(4)
