function $range [int&firstColumn] [int&firstRow] [int&columnCount] [int&rowCount] ;;range examples: "<sel>", "<active>", "<used>"

 Gets index of first column/row and number of columns/rows in specified range.

 range - range string. <help>Excel range strings</help>.
 firstColumn, firstRow - variables that receive 1-based index of the first column and row of the range. Can be 0 if don't need.
 columnCount, rowCount - variables that receive the number of columns and rows. Can be 0 if don't need.

 REMARKS
 This function is useful to get location of special ranges, such as selection, active cell, used range.
 For selection, use range "<sel>". It gets location of selection in the active sheet, even if this variable represents other sheet.
 For the active cell, use "<active>". Like with "<sel>", it gets location in the active sheet.
 For the used range, use "<used>" or "". It is the part of worksheet containing non-empty cells (data, formatting). Does not include empty columns/rows at the beginning.

 See also: <ExcelSheet.NumRows>, <ExcelSheet._Range>.
 Added in: QM 2.3.3.
 Errors: Excel errors

 EXAMPLE
  /exe 1
 ExcelSheet es.Init
 int  c1 r1 nc nr
 es.GetRangeInfo("<sel>" c1 r1 nc nr)
 out "%i %i %i %i" c1 r1 nc nr


WS

Excel.Range r=__Range(range)

if(&firstColumn) firstColumn=r.Column
if(&firstRow) firstRow=r.Row
if(&columnCount) columnCount=r.Columns.Count
if(&rowCount) rowCount=r.Rows.Count

err+ E
