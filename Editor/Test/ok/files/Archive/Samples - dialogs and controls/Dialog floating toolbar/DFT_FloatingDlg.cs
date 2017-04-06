 /DFT_Main
 \Dialog_Editor

 This is a floating window for the toolbar control.

function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90CC0A48 0x80 0 0 217 130 "Toolbar"
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_CONTEXTMENU
	sel PopupMenu("Docked")
		case 1
		DFT_MakeFloating hDlg 0
	ret 1
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	DFT_MakeFloating hDlg 0
	case else
	 relay toolbar button click messages to the main window
	SendMessage GetWindow(hDlg GW_OWNER) WM_COMMAND wParam lParam
ret 1
