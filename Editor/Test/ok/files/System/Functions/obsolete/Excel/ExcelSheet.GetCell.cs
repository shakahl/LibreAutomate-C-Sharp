function# str&s column row

 Gets value of a cell.
 Returns 1 if successful, or 0 if column or row is too big (beyond the used range).

 column - 1-based column index (1 for A, and so on).
 row - 1-based row index.

 Errors: _error

 EXAMPLES
  get cell A3
 str s
 ExcelSheet es.Init
 es.GetCell(s 1 3)
 out s

  display values of cells in all rows in columns A and B
 ExcelSheet es.Init
 str s1 s2; int row
 for row 1 100000
	 if(!es.GetCell(s1 1 row)) break ;;break when there are no more used rows
	 es.GetCell(s2 2 row)
	 out "A%i=%s;  B%i=%s" row s1 row s2


WS(1)

Excel.Range r=ws.Cells.Item(row column)
s=r.Value
if(!s.len) if(row>NumRows(_i) or column>_i) ret
ret 1
err+ E
