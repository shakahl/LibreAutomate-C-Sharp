\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4 5"
str cb3 cb4 lb5
cb3="&yellow background[]blue text[]default colors[]black & green"
cb4="&yellow background[]blue text[]default colors[]black & green"
lb5="yellow background[]blue text[]default colors[]black & green"
if(!ShowDialog("dialog_combo_item_colors" &dialog_combo_item_colors &controls)) ret
out cb3
out cb4
out lb5

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 204 98 "Dialog"
 1 Button 0x54030001 0x4 4 80 48 14 "OK"
 2 Button 0x54030000 0x4 56 80 48 14 "Cancel"
 3 ComboBox 0x54230253 0x0 4 6 96 215 ""
 4 ComboBox 0x54230252 0x0 4 30 96 215 ""
 5 ListBox 0x54230151 0x200 106 6 96 64 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030109 "" "" ""

ret
 messages

CB_ItemColor hDlg message wParam lParam 3 &SampleCbItemColorProc
CB_ItemColor hDlg message wParam lParam 4 &SampleCbItemColorProc
CB_ItemColor hDlg message wParam lParam 5 &SampleCbItemColorProc

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
