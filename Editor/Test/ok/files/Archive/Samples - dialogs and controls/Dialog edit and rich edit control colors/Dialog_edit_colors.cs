\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
e3="red text"
if(!ShowDialog("Dialog_edit_colors" &Dialog_edit_colors &controls)) ret

 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 145 99 "Form"
 1 Button 0x54030001 0x4 4 82 48 14 "OK"
 2 Button 0x54030000 0x4 56 82 48 14 "Cancel"
 3 Edit 0x54030080 0x204 8 38 128 16 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010601 "*" ""

ret
 messages
int textcolor=0xff
int backcolor=0x00ff00
int-- hbrush

sel message
	case WM_INITDIALOG
	hbrush=CreateSolidBrush(backcolor)
	
	case WM_DESTROY
	DeleteObject hbrush
	
	case WM_COMMAND goto messages2
	
	case WM_CTLCOLOREDIT
	if(lParam=id(3 hDlg))
		SetTextColor wParam textcolor ;;text
		SetBkMode wParam TRANSPARENT
		ret hbrush ;;background
	 Similarly, you can use other WM_CTLCOLORx messages for other control classes and dialog itself.
	
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
