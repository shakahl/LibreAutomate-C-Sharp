function# row $cells nCells [firstCell] [flags] [LVITEM&lvi] ;;flags: 1 replace row text, 2 apply row properties when replacing.

 Adds new row, or replaces text of cells in a row.
 Same as RowAddSetSA. Different is only cells format.
 Returns row index if successful, -1 if failed.

 row - 0-based row index.
    If < 0, adds to the end.
    If > row count, adds empty rows before.
 cells - text of cells as multistring, like "cell1[0]cell2[0]cell3".
    Can be specified <help #IDP_QMGRID#details>grid row properties</help>. When replacing, applies them only if flag 2.
 nCells - number of strings in cells. When replacing, it can be 0 if you only want to change image etc using lvi.
 firstCell - 0-based index of first column. If firstCell and nCells don't span all columns, remaining cells will not be set or changed.
 lvi - can be used to set images, lparam and indent (but not state). LVITEM is documented in MSDN Library.

 When adding multiple rows using this function, don't call other DlgGrid functions in between, and don't send messages. It would make adding rows much slower.


ret RowAddSet(row 0 cells nCells firstCell flags lvi)
