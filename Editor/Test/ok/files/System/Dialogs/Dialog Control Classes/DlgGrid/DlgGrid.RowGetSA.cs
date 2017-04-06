function# ARRAY(str)&a row nCells [firstCell] [flags] [LVITEM&lvi] ;;flags: 2 no empty rows, 4 only selected/checked rows

 Gets row into array. Also can get other properties.
 Returns number of bytes in the cells, including terminating null characters.
 Returns 0 if failed.
 Fails if row does not exist, or if firstCell+nCells is more than grid column count.
 Returns a negative value if succeeded but didn't get the cells (see below).

 a - receives cells. Can be 0 if you use only lvi.
 row - 0-based row index.
 nCells - number of cells to get. Can be 0 if you use only lvi; if successful, returns -1.
 firstCell - 0-based index of first column.
 flags:
    2 - don't get cells if all the cells are empty.
    4 - don't get cells if the row is not selected or not checked (if the grid has check boxes style).
 lvi - can be used to get image, state, lparam and indent.
    lvi.mask can contain LVIF_IMAGE, LVIF_STATE, LVIF_PARAM, LVIF_INDENT. lvi.iSubItem must be 0.
    LVITEM is documented in MSDN Library.

 If didn't get the cells, returns (in order of evaluation):
 0 if failed.
 -1 if nCells is 0.
 -4 if didn't get the cells because of flag 4.
 -2 if didn't get the cells because of flag 2.


if(&a) a=0
int i nb
lpstr s=RowGetMS(row nCells firstCell flags nb lvi)

if &a and nb>0
	a.create(nCells)
	for(i 0 nCells) a[i]=s; s+len(s)+1

ret nb
