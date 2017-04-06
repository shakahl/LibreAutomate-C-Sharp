function ARRAY(str)&a [$range] ;;range examples: "" (all), "sel" (selection), "A1:C3" (range), press F1 to see more.

 Stores whole or part of the worksheet into two-dimensional array.
 If range is used and not "", gets only the specified cells.
   range examples: "" (all), "sel" (selection in active sheet), "A1:C3" (range), "A:C" (columns A, B and C), "3:3" (row 3), "A1" (cell), "Named" (named range)


if(!ws) Init

int i j nr nc
Excel.Range r cell

GetRange(range r nr nc)

for i 0 a.len
	for j 0 a.len(1)
		cell=r.Item(i+1 j+1)
		cell.Value=a[j i]

err+ E
