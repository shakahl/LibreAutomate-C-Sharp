 /
function lParam idGrid

 Call on WM_NOTIFY. When user edits a cell, checks the row.
 Uses only GRID.LVN_QG_CHANGE.

NMHDR* nh=+lParam
GRID.QM_NMLVDATA* cd=+nh
if nh.idFrom=idGrid and nh.code=GRID.LVN_QG_CHANGE and cd.hctrl
	DlgGrid g.Init(nh.hwndFrom)
	g.RowCheck(cd.item 1)
