 /
function# ARRAY(str)&row ExcelSheet&es [$range] [flags] ;;range examples: "" (used range), "sel" (selection), "A1:C3" (range), "A:C" (columns A, B and C)

 Gets Excel worksheet cell values from each row. Use with foreach.

 row - variable that receives cell values of a row.
   The function creates 1-dim array.
 es - variable that represents a worksheet.
   At first call its Init function.
   Can be 0. Then gets from the active worksheet.
 range - range of cells. Default: "" (whole used range). <help>Excel range strings</help>.

 See also: <ExcelSheet.CellsToArray>, <ExcelSheet.Init>, <ExcelSheet help>
 Added in: QM 2.3.3.
 Errors: Excel errors.

 EXAMPLES
  /exe 1
 ExcelSheet es.Init

  for each row, display cells in columns A, B and C
 ARRAY(str) a
 foreach a es FE_ExcelSheet_Row "A:C"
	 out "%s, %s, %s" a[0] a[1] a[2]

  display all sheet (used range)
 ARRAY(str) a; int r c
 foreach a es FE_ExcelSheet_Row
	 r+1; out "---- row %i ----" r
	 for c 0 a.len
		 out a[c]


int r c nc
ARRAY(str) a

if !r
	ExcelSheet _es; if(!&es) &es=_es; es.Init
	es.CellsToArray(a range flags); nc=a.len(1)

if(r>=a.len) ret
if(row.len!nc) row.redim(nc)
for(c 0 nc) row[c].swap(a[c r])
r+1

ret 1
err+ end _error
