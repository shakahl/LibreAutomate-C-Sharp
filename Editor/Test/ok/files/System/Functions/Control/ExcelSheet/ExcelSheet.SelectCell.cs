function `column [row] [flags] ;;column: index or string like "A" or "A1".  flags: 1 activate

 Selects a cell.

 column - 1-based column index.
   QM 2.3.3. Can be column name, like "A". 
   QM 2.3.3. Can be cell name, like "A1. Then row argument not used. Or can be "<active>".
 row - 1-based row index.
   QM 2.3.3. Optional. 
 flags (QM 2.3.3):
   1 - if the cell is in the current selection, activate the cell and don't change the current selection.

 Added in: QM 2.3.2.
 Errors: Excel errors

 REMARKS
 Tip: You can also use Excel function Goto. Read more in Excel Help.


WS(1)

Excel.Range r=__RangeCR(column)
if(flags&1) r.Activate
else r.Select

err+ E
