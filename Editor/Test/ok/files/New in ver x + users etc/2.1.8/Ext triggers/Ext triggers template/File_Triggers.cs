 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

UDTRIGGER& p=+lParam
 out wParam
sel wParam
	case 1 ;;create and initialize property page
	ret
	str controls = "3 4"
	str e3 c4Che
	e3=p.tdata
	ret ShowDialog("File_Triggers" &File_Triggers &controls p.hwnd 1 WS_CHILD)
	
	case 2 ;;collect property page data and format trigger string
	p.tdata.getwintext(id(3 p.hwnd))
	
	case 3 ;;validate trigger string
	ret 1
	
	case 4 ;;[re]create trigger tables
	 int i
	 for(i 0 p.niids)
		 out _s.getmacro(p.iids[i] 1)
	
	case 5 ;;provide icon
	ret GetIcon("files_find.ico")
	
	case 6 ;;help
	mes "help" "" "i"
	

 BEGIN DIALOG
 0 "" 0x10000040 0x10000 0 0 175 101 "Form"
 3 Edit 0x54030080 0x204 8 8 96 14 ""
 4 Button 0x54012003 0x0 8 28 48 13 "Check"
 END DIALOG
 DIALOG EDITOR: "" 0x2010703 "*" ""

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
