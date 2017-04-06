\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str rea3
rea3="&$desktop$\document.rtf"
if(!ShowDialog("RichEdit" &RichEdit &controls)) ret


 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 RichEdit20A 0x54031044 0x204 2 4 92 72 ""
 4 Button 0x54032000 0x4 102 4 48 14 "Save"
 END DIALOG
 DIALOG EDITOR: "" 0x2010700 "" ""

ret
 messages
sel message
	case WM_INITDIALOG DT_Init(hDlg lParam); ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
int ctrlid=wParam&0xFFFF; message=wParam>>16
sel wParam
	case 4
	RichEditSave id(3 hDlg) "$desktop$\rtf.RTF"
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
