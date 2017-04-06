function~ `column [row] [flags] [int&vt] ;;column: index or string like "A" or "A1".  flags: get 1 date as number, 2 formula, 3 text, 4 comment, 5 hyperlink

 Gets cell value. Also can get formula, comment or hyperlink.

 column - 1-based column index.
   Can be column name, like "A". 
   Can be cell name, like "A1. Then row argument not used and can be 0. Or can be "<active>".
 row - 1-based row index.
 flags:
   0-3 - same as with <help>ExcelSheet.CellsToArray</help>.
   4, 5 (QM 2.4.2) - get cell comment or hyperlink instead. Don't use with flags 0-3.
 vt - variable that receives value type, like with CellsToArray.

 REMARKS
 Tip: To get multiple cells, use <help>ExcelSheet.CellsToArray</help>, it is faster.

 Added in: QM 2.3.3.
 Errors: Excel errors

 EXAMPLES
  /exe 1
 ExcelSheet es.Init
 str s
 s=es.Cell(1 3) ;;get cell A3
 s=es.Cell("A" 3) ;;the same
 s=es.Cell("A3") ;;the same
 int i=val(es.Cell("A3")) ;;get cell A3 as integer number
 double d=val(es.Cell("A3") 2) ;;get cell A3 as double number


WS

Excel.Range r=__RangeCR(column)

VARIANT v
sel flags&7
	case 0 v=r.Value
	case 1 v=r.Value2
	case 2 v=r.Formula
	case 3 v=r.Text
	case 4 IDispatch c=r.Comment; if(c) v=c.Text
	case 5 Excel.Hyperlinks h=r.Hyperlinks; v=h.Item(1).Address; err
	case else end ERR_BADARG

if(&vt) vt=v.vt

ret v; err

err+ E

 notes:
 Error if str=VARIANT(VT_ERROR).
