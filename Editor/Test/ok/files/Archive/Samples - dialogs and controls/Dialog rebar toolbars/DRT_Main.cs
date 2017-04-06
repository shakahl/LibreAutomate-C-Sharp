\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "50"
str rea50
if(!ShowDialog("DRT_Main" &DRT_Main &controls)) ret

 BEGIN DIALOG
 0 "" 0x92CF0A48 0x0 0 0 217 152 "Dialog"
 50 RichEdit20A 0x54233044 0x200 2 60 220 88 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030002 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 create 2 toolbars. You can add more DRT_TbInit to create more toolbars.
	DRT_TbInit hDlg 4 1001 "Cut[]Copy[]Paste" "cut.ico[]copy.ico[]paste.ico" 0
	DRT_TbInit hDlg 5 2001 "Text[]Key[]Mouse" "text.ico[]keyboard.ico[]mouse.ico" 1
	 create rebar and add all toolbars
	DRT_RebarInit hDlg
	
	DRT_ResizeControls hDlg
	
	case WM_SIZE
	DRT_ResizeControls hDlg
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case 1001
	out "cut"
	 ...
	
	case 2001
	out "text"
	 ...
	
	case IDOK
	case IDCANCEL
ret 1
 messages3
NMHDR* n=+lParam
sel n.code
	case RBN_LAYOUTCHANGED
	DRT_RebarOnRearrange hDlg
