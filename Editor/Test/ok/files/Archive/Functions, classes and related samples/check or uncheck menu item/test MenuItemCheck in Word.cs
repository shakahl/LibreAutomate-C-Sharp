int w1=win("Document1 - Microsoft Word" "OpusApp")
act w1
out
spe
key Av ;;show submenu. It is necessary because check mark often is added/removed only when showing the submenu.
spe -1
int wasChecked=MenuItemCheck(3 "Ruler" "MsoCommandBarPopup")
out wasChecked
