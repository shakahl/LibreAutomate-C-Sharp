function$ row nCells [firstCell] [flags] [int&nBytes] [LVITEM&lvi] ;;flags: 2 no empty rows, 4 only selected/checked rows

 Returns row text in multistring format. Also can get other properties.
 Returns 0 if failed.
 Fails if row does not exist, or if firstCell+nCells is more than grid column count.
 The return value is temporary.

 row - 0-based row index.
 nCells - number of cells to get. Can be 0 if you use only lvi; if successful, returns "".
 firstCell - 0-based index of first column.
 flags:
    2 - return "" if all the cells are empty.
    4 - return "" if the row is not selected or not checked (if the grid has check boxes style).
 nBytes - variable that receives number of bytes in the multistring.
    If didn't get the cells, nBytes will be (in order of evaluation):
    0 if failed.
    -1 if nCells is 0.
    -4 if didn't get the cells because of flag 4.
    -2 if didn't get the cells because of flag 2.
 lvi - can be used to get image, state, lparam and indent.
    lvi.mask can contain LVIF_IMAGE, LVIF_STATE, LVIF_PARAM, LVIF_INDENT. lvi.iSubItem must be 0.
    LVITEM is documented in MSDN Library.


GRID.QG_ROW r
r.nCells=nCells
r.firstCell=firstCell
r.flags=flags
r.lvi=&lvi

_i=Send(GRID.LVM_QG_GETROW row &r)
if(&nBytes) nBytes=_i
ret r.cells
