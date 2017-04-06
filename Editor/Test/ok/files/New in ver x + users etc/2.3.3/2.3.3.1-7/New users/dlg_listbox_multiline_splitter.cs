\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4"
str lb3 cb4
 note: don't use dialog variable to add ownerdraw listbox/combobox items.

if(!ShowDialog("" &dlg_listbox_multiline_splitter &controls)) ret

 note: in dialog editor, select the listbox control and add styles LBS_OWNERDRAWVARIABLE and LBS_HASSTRINGS. Similar with combobox.

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 ListBox 0x54230161 0x200 0 0 108 114 ""
 4 ComboBox 0x54230263 0x0 118 0 102 215 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 8 QM_Splitter 0x54030000 0x0 108 0 8 112 ""
 5 QM_Splitter 0x54030000 0x0 0 112 108 10 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages

 call this function before sel message
if(DT_LbCbOwnerDraw(hDlg message wParam lParam 0 3)) ret 1

sel message
	case WM_INITDIALOG
	 optionally set font
	__Font-- t_f.Create("Comic Sans MS" 10 2)
	t_f.SetDialogFont(hDlg "3 4")
	
	 add items. Or can add/remove later. To remove all, send message LB_RESETCONTENT or CB_RESETCONTENT.
	ARRAY(str) a.create(3)
	a[0]="normal"
	a[1]="line[]line[]line"
	a[2]="wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap"
	int i h
	h=id(3 hDlg)
	for(i 0 a.len) LB_Add h a[i]
	h=id(4 hDlg)
	for(i 0 a.len) CB_Add h a[i]
	
	h=id(3 hDlg)
	SetClassLong h GCL_STYLE GetClassLong(h GCL_STYLE)|CS_HREDRAW|CS_VREDRAW
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
