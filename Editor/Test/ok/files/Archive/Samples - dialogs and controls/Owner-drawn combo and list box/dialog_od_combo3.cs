\Dialog_Editor

 Shows how to draw icons in combo box using a bmp file created by the QM imagelist editor.

function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str cb3
if(!ShowDialog("dialog_od_combo3" &dialog_od_combo3 &controls)) ret
out cb3

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 204 164 "Dialog"
 1 Button 0x54030001 0x4 4 146 48 14 "OK"
 2 Button 0x54030000 0x4 56 146 48 14 "Cancel"
 3 ComboBox 0x54230253 0x0 4 6 96 215 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "" ""

ret
 messages

CB_DrawImages hDlg message wParam lParam 3 "$qm$\il_qm.bmp" 2|4
 note: il_qm.bmp is used by QM. It may be changed or deleted in the future. Create your own file with the imagelist editor. You can find the editor in the Icons dialog or in the floating toolbar.

sel message
	case WM_INITDIALOG
	int h=id(3 hDlg)
	CB_Add h "one" 0
	CB_Add h "two" 1
	CB_Add h "three" 15
	CB_Add h "four" 16
	CB_SelectItem h 0
	 third argument of LB_Add is icon index in the imagelist
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
