\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

typelib Word {00020905-0000-0000-C000-000000000046} 8.0

str controls = "3"
str ax3SHD
if(!ShowDialog("dlg_doc_editor" &dlg_doc_editor &controls 0 0 0 0 0 0 0 "" "dlg_doc_editor")) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 474 335 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 474 334 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser we3
	we3._getcontrol(id(3 hDlg))
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
we3._getcontrol(id(3 hDlg))
sel wParam
	case 101 ;;open
	if(!OpenSaveDialog(0 _s "doc[]*.doc[]")) ret
	we3.Navigate(_s)
	
	case 102 ;;save
	Word.Document doc=we3.Document
	doc.Save
	
	case IDOK
	case IDCANCEL
ret 1

 BEGIN MENU
 >&File
	 &Open : 101 0 0 Co
	 &Save : 102 0 0 Cs
	 <
 >&Edit
	 Empty : 201 0 1
	 <
 END MENU
