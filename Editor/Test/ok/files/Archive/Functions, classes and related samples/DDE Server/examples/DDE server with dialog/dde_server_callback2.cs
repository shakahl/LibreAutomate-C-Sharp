 /DDE_server_with_dialog
function! message $topic $item str&data idInst hConv hDlg reserved

 Callback function used with DDE_server_with_dialog.
 Parameters etc are documented in dde_server_callback.


sel message
	case XTYP_EXECUTE
	data.setwintext(id(6 hDlg))
	
	case XTYP_POKE
	data.setwintext(id(7 hDlg))
	
	case XTYP_REQUEST
	data.getwintext(id(8 hDlg))

ret 1
