 does not work

 int w=id(2202 _hwndqm) ;;QM list of items (tree view)
 int w=id(1023 win("QM - My Macros" "#32770")) ;;list
int w=id(222 win("QM Help" "HH Parent")) ;;list 'Help Index Control'

RECT rc; GetClientRect w &rc
 SetScrollPos(w SB_VERT 30 1) ;;does not scroll, just sets scrollbar position. Did not test SetScrollInfo, probably the same.
out ScrollWindow(w 0 -rc.bottom 0 0)
 out ScrollWindowEx(w 0 -rc.bottom 0 0 0 0 0)

 __Hdc dc.Init(w)
 out ScrollDC(dc 0 -rc.bottom 0 0 0 0)
