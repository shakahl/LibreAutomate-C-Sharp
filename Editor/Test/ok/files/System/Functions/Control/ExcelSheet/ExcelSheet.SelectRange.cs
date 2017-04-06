function [$range] [flags] ;;range examples: "A1" (cell), "3:3" (row), "C:C" (column), "A1:C3" (range)

 Selects one or more cells, rows or columns.

 range - cells to select. <help>Excel range strings</help>.
 flags (QM 2.3.3):
   1 - if whole row or column specified, limit to the upper bound of the used range.

 Added in: QM 2.3.2.
 Errors: Excel errors


WS(1)

Excel.Range r
if(flags&1) GetRange(range r 0 0 1); else r=__Range(range)
r.Select

err+ E
