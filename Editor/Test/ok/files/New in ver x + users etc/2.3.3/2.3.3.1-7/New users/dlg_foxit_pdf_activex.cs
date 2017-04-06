\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

typelib FOXITREADEROCXLib {3D20F47F-E818-4A03-AD52-45B708ACCF23} 1.0

if(!ShowDialog("" &dlg_foxit_pdf_activex 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 390 280 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 392 280 "FOXITREADEROCXLib.FoxitReaderOCX {05563215-225C-45EB-BB34-AFA47217B1DE}"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""
 3 ActiveX 0x54030000 0x0 0 0 390 280 "SHDocVw.WebBrowser"

ret
 messages
sel message
	case WM_INITDIALOG
	FOXITREADEROCXLib.FoxitReaderOCX x._getcontrol(id(3 hDlg))
	x.OpenFile(_s.expandpath("$documents$\Knygos\J.Gray - Vyrai kilę iš Marso, moterys - iš Veneros.pdf"))
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
