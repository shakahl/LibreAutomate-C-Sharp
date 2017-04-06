\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 ListBox 0x54230161 0x200 0 0 108 114 ""
 4 ComboBox 0x54230262 0x0 118 14 102 215 ""
 5 ComboBox 0x54230263 0x0 118 54 102 215 ""
 6 Static 0x54000000 0x0 118 4 78 10 "Editable combo"
 7 Static 0x54000000 0x0 118 44 78 10 "Read-only combo"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

str controls = "3 4 5"
str lb3 cb4 cb5
 note: don't use dialog variable to add ownerdraw listbox/combobox items.

if(!ShowDialog(dd &sub.DlgProc &controls)) ret

 note: in dialog editor, select the listbox control and add styles LBS_OWNERDRAWVARIABLE and LBS_HASSTRINGS. Similar with combobox.


#sub DlgProc
function# hDlg message wParam lParam

 arrays for colors and fonts
ARRAY(int)-- t_act t_acb t_afont

 call this function before sel message
if(DT_LbCbOwnerDraw(hDlg message wParam lParam 0 1 t_act t_acb 0xffff 0x1 t_afont)) ret 1

sel message
	case WM_INITDIALOG
	 optionally set common font
	__Font-- t_f.Create("Comic Sans MS" 10 2)
	t_f.SetDialogFont(hDlg "3 4 5")
	
	 create arrays for colors
	t_act.create(4); t_act[0]=0xff; t_act[1]=0x8000; t_act[2]=0xff0000
	t_acb.create(4); t_acb[0]=0xffC0C0; t_acb[1]=0xC0FFff; t_acb[2]=0xC0ffC0; t_acb[3]=0xffffff
	
	 create array for fonts
	__Font-- t_f1.Create("Tahoma" 15 1) t_f2.Create("" 0 4 0 1 4)
	t_afont.create(4); t_afont[1]=t_f1; t_afont[2]=t_f2; t_afont[3]=t_f1 ;;and t_afont[0]=0 - default font
	
	 add items. Or can add/remove later. To remove all, send message LB_RESETCONTENT or CB_RESETCONTENT.
	ARRAY(str) a.create(4)
	a[0]="normal"
	a[1]="line[]line[]line"
	a[2]="wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap wrap"
	a[3]="last"
	int i h
	h=id(3 hDlg)
	for(i 0 a.len) LB_Add h a[i]
	h=id(4 hDlg)
	for(i 0 a.len) CB_Add h a[i]
	h=id(5 hDlg)
	for(i 0 a.len) CB_Add h a[i]
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
