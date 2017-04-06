 /
function hwndControl sizePlus

 Sets a control to make bigger on mouse over.
 Call this function in dialog procedure, under WM_INITDIALOG.

 hwndControl - control handle.
 sizePlus - how much bigger.


type CMCS_DATA wndProc sizePlus !big RECT'r
CMCS_DATA* p._new
p.wndProc=SubclassWindow(hwndControl &CMCS_WndProc)
p.sizePlus=sizePlus
SetProp hwndControl "CMCS_DATA" p
