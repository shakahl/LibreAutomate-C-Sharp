out
int w=win("Quick Macros - ok - [Macro1727]" "QM_Editor")
int c=child("Running items" "" w) ;;outline
outw c; if(c=0) ret
int p=GetParent(c)

outw GetParent(c)
outw GetWindowLong(c GWL_HWNDPARENT)


SetParent(c 0)
 SetWinStyle c WS_CHILD 2
_i=GetWinStyle(c); SetWinStyle c _i~WS_CHILD|WS_POPUP
mes 1
SetParent(c p)
 SetWinStyle c WS_CHILD 1
_i=GetWinStyle(c); SetWinStyle c _i|WS_CHILD~WS_POPUP
 outx GetWinStyle(c)

outw GetParent(c)
outw GetWindowLong(c GWL_HWNDPARENT)
