 /
function# $itemstring hwndctrl [ctrltype] [startfrom] [exact] ;;ctrltype: 0 combobox, 1 listbox.

ret sub_sys.CBLB_FindItem(hwndctrl itemstring ctrltype startfrom exact)
