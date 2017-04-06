 /
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	DT_Init(hDlg lParam)
	ret 1
	 ----
	case WM_COMMAND
	int msg(wParam >> 16) ctrlid(wParam & 0xFFFF)
	sel msg
		case BN_CLICKED
		sel ctrlid
			case IDOK
			DT_GetControls(hDlg)
			EndDialog hDlg 1
			 ----
			case IDCANCEL EndDialog hDlg 0
			case else
			 Button(s) requiring immediate return on being clicked: return Button id
			if GetWinStyle(id(ctrlid hDlg))=0x54032001 
				 DT_GetControls(hDlg);;if want to retrieve data from other controls on exit, omit if not needed since it adds to overhead in calling procedure (need to specify controls & strings then even if not needed)
				EndDialog hDlg ctrlid
	ret 1
	 ----
	case WM_DESTROY DT_DeleteData(hDlg)


 ShowDialog uses this function as dialog
 procedure when dlgproc omitted or is 0.