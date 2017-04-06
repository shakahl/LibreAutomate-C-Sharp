 /
function# hDlg cbId

sel CB_SelectedItem(id(cbId hDlg))
	case 0 ret id(2210 _hwndqm)
	case 1 _i=id(2211 _hwndqm); if(!hid(_i)) ret _i
	case 2 ret id(4 win("QM File Viewer" "#32770" "" 2)); err
