 /
function# ARRAY(str)&rowcells $range [`sheet] [flags] [$book] ;;Used with foreach, to get cell values in Excel.

 This function is used with foreach, to get Excel worksheet cell values in each row.

 rowcells - will be populated with cell values in single row
 range - range of cells. Default: "" (whole sheet).
   range examples: "" (all), "sel" (selection), "A1:C3" (range), "A:C" (columns A, B and C), "Named" (named range)
 sheet, flags, book - same as Init.

 REMARKS
 Excel must not be in cell edit mode.

 See also: <ExcelSheet.Init>

 EXAMPLES
  for each row, display cells in columns A, B and C
 ARRAY(str) a
 foreach a "A:C" FE_ExcelRow
	 out "%s %s %s" a[0] a[1] a[2]

  the same, but only for rows 1 to 10
 ARRAY(str) a
 foreach a "A1:C10" FE_ExcelRow
	 out "%s %s %s" a[0] a[1] a[2]


int i j nr nc
ExcelSheet e
Excel.Range r cell

if(!i)
	e.Init(sheet flags book)
	e.GetRange(range r nr nc)

if(i>=nr) ret
i+1

rowcells.redim(nc)
for j 0 nc
	cell=r.Item(i j+1)
	rowcells[j]=cell.Value

ret 1
err+ end _error
