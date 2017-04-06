function# row cellsFormat !*cells nCells [firstCell] [flags] [LVITEM&lvi]

GRID.QG_ROW r
r.flags=cellsFormat&3|(flags<<2)
r.nCells=nCells
r.cells=cells
r.firstCell=firstCell
r.lvi=&lvi

ret Send(GRID.LVM_QG_ADDROW row &r)
