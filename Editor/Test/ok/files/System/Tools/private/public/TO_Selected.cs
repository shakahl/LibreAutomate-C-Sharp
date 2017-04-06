 /
function# hDlg [cid] [str&itemText] ;;if cid 0, hDlg must be control handle

 Returns selected index of listbox or combobox.

int h=iif(cid id(cid hDlg) hDlg)
sel WinTest(h "ListBox[]ComboBox")
	case 1 ret LB_SelectedItem(h itemText)
	case 2 ret CB_SelectedItem(h itemText)
