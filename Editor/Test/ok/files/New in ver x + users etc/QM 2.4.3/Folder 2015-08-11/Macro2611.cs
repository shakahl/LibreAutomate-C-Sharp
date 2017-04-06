out
int w=win("Dependency Walker" "Afx:*" "" 0x4)
 SetParent(w GetDesktopWindow)
 SetWindowLong(w GWL_HWNDPARENT GetDesktopWindow)
 w=_hwndqm
outw GetWindow(w GW_OWNER)
outw GetAncestor(w 3)
