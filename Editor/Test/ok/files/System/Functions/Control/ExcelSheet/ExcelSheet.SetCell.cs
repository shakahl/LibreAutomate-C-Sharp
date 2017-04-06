function ~s `column [row] ;;column: index or string like "A" or "A1"

 Sets value of a cell.

 s - value or =formula. Can be string or number.
 column - 1-based column index.
   QM 2.3.3. Can be column name, like "A". 
   QM 2.3.3. Can be cell name, like "A1. Then row argument not used. Or can be "<active>".
 row - 1-based row index.
   QM 2.3.3. Optional. 

 Errors: Excel errors

 EXAMPLES
  /exe 1
 ExcelSheet es.Init
 es.SetCell("abc" 1 3) ;;set cell A3 to "abc"
 
  add 1 day to cell G1 that contains date
 DateTime d
 d.FromStr(es.Cell("G1"))
 d.AddParts(1) ;;add 1 day
 es.SetCell(d.ToStr "G1")


WS(1)

Excel.Range r=__RangeCR(column)
r.Value=s

err+ E
