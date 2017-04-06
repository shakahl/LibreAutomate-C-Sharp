function# row str*cells nCells [firstCell] [flags] [LVITEM&lvi] ;;flags: 1 replace row text, 2 apply row options when replacing.

 Adds new row, or replaces text of cells in a row.
 Same as <help>DlgGrid.RowAddSetMS</help>. Different is only cells format.

 cells - text of cells. Address of first variable in array of str variables.
    If the array is ARRAY(str) a, cells can be &a[0].
    Or it can be address of first of several adjacent local str variables.


ret RowAddSet(row 2 cells nCells firstCell flags lvi)
