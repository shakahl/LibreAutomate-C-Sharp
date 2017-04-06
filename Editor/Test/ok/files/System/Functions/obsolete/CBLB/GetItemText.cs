 /
function# hwndctrl item str&itemtext [ctrltype] ;;ctrltype: 0 combobox, 1 listbox.

ret sub_sys.CBLB_GetItemText(hwndctrl item itemtext ctrltype)
