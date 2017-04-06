 /
function# hwndctrl [ctrltype] [str&itemtext] ;;ctrltype: 0 combobox, 1 listbox.

ret sub_sys.CBLB_SelectedItem(hwndctrl ctrltype itemtext)
