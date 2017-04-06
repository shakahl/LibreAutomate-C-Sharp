int w1=win("Untitled - Notepad" "Notepad")
act w1
out
spe
key Ao ;;show submenu. It is necessary because check mark often is added/removed only when showing the submenu.
spe -1
int wasChecked=MenuItemCheck(3 "Word Wrap")
out wasChecked
