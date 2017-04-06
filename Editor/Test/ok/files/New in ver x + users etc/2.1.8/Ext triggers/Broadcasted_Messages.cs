 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

UDTRIGGER& p=+lParam
 out wParam
sel wParam
	case 1 goto g1 ;;create and initialize property page
	
	case 2 goto g2 ;;collect property page data and format trigger string
	
	case 3 ;;validate trigger string
	ret 1
	
	case 4 ;;[re]create trigger tables
	 int i
	 for(i 0 p.niids)
		 out _s.getmacro(p.iids[i] 1)
	
	case 5 ;;provide icon
	 ret GetIcon("sound.ico")
	
	case 6 ;;help
	 mes "help" "" "i"
	
ret

 g1
str controls = "3 4 5"
str cb3 cb4 lb5
 =p.tdata
ret ShowDialog("" &Broadcasted_Messages &controls p.hwnd 1 WS_CHILD)

 g2
 p.tdata.getwintext(id(3 p.hwnd))



 BEGIN DIALOG
 0 "" 0x10000044 0x10000 0 0 203 368 "Form"
 3 ComboBox 0x54230243 0x4 0 14 202 213 ""
 4 ComboBox 0x54230243 0x4 4 332 96 213 ""
 5 ListBox 0x54230101 0x204 4 166 96 48 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010800 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG DT_Init(hDlg lParam); ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
