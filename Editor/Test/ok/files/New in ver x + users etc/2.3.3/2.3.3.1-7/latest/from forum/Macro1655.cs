int w=win("" "QM_Editor")
int htv=id(2212 w) ;;Open items
 SetWinStyle htv TVS_CHECKBOXES 1

 TvCheckAll htv

int hi
int checked=1
TVITEMEXW t.hItem=hi
t.mask=TVIF_STATE
t.state=(checked!0+1)<<12
t.stateMask=3<<12
SendMessage(htv TVM_SETITEM 0 &t)
