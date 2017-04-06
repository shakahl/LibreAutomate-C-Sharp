\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "5"
str rea5
rea5="right click toolbar..."
if(!ShowDialog("DFT_Main" &DFT_Main &controls)) ret

 BEGIN DIALOG
 0 "" 0x90CF0A48 0x0 0 0 217 129 "Dialog"
 5 RichEdit20A 0x54233044 0x200 0 22 220 88 ""
 3 ToolbarWindow32 0x54010000 0x0 0 0 217 17 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	DFT_TbInit hDlg 3 1001 "Cut[]Copy[]Paste" "cut.ico[]copy.ico[]paste.ico"
	DFT_ResizeControls hDlg
	
	case WM_CONTEXTMENU
	sel GetWinId(wParam)
		case 3
		sel PopupMenu("Floating")
			case 1
			DFT_MakeFloating hDlg 1
		ret 1
		
		case 5
		sel PopupMenu("Sample[]Menu")
			case 1
			case 2
	
	case WM_DESTROY
	ImageList_Destroy SendMessage(id(3 hDlg) TB_GETIMAGELIST 0 0)
	
	case WM_SIZE
	DFT_ResizeControls hDlg
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 1001
	out "cut"
	SendMessage(id(5 hDlg) WM_CUT 0 0)
	case 1002
	out "copy"
	SendMessage(id(5 hDlg) WM_COPY 0 0)
	case 1003
	out "paste"
	SendMessage(id(5 hDlg) WM_PASTE 0 0)
	
	case IDOK
	case IDCANCEL
ret 1
