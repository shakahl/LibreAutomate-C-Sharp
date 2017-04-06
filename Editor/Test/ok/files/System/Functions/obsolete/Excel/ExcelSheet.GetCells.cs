function ARRAY(str)&a [$range] ;;range examples: "" (all), "sel" (selection), "A1:C3" (range), press F1 to see more.

 Gets whole or part of worksheet into array.

 a - variable for data.
   This function creates array of 2 dimensions.
 range - specifies part of worksheet from which to get data. Default: "" - all (used range).
   Examples: "sel" (selection in active sheet), "A1:C3" (range), "A:C" (columns A, B and C), "3:3" (row 3), "A1" (cell), "Named" (named range).

 See also: <FE_ExcelRow>.

 Errors: _error

 EXAMPLE
  display selected cells
 ARRAY(str) a
 ExcelSheet es.Init
 es.GetCells(a "sel")
 int r c
 for r 0 a.len
	 out "-----Row %i-----" r+1
	 for c 0 a.len(1)
		 out a[c r]


WS(1)

int i j nr nc
Excel.Range r cell

GetRange(range r nr nc)

a.create(nc nr)
for i 0 nr
	for j 0 nc
		cell=r.Item(i+1 j+1)
		a[j i]=cell.Value

err+ E
