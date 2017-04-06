\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str- t_file="$my qm$\dialog_listbox_save.txt" ;;change this
ARRAY(str)- t_selected ;;the dialog will create this array on OK

if(!dir(t_file)) _s="one[]two[]three"; _s.setfile(t_file) ;;create file for testing

str controls = "4 3"
str e4 lb3
lb3.getfile(t_file)
if(!ShowDialog("dialog_listbox_add_remove_save" &dialog_listbox_add_remove_save &controls)) ret

lb3=t_selected ;;convert array to string, if need
out "SELECTED:"
out lb3

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 4 Edit 0x54030080 0x200 4 4 96 14 "" "Item to find or add"
 3 ListBox 0x54230109 0x200 4 22 98 90 ""
 7 Button 0x54032000 0x0 104 4 48 14 "Find" "Finds the specified item"
 5 Button 0x54032000 0x0 104 20 48 14 "Add" "Adds the specified item"
 6 Button 0x54032000 0x0 104 36 48 14 "Remove" "Removes selected items"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030604 "*" "" "" ""

ret
 messages
int hlb i n; str s ss
int- t_changed
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
hlb=id(3 hDlg)
sel wParam
	case 5 ;;Add
	s.getwintext(id(4 hDlg)); if(!s.len) ret
	i=LB_FindItem(hlb s 0 1); if(i>=0 and mes("Already exists. Add anyway?" "" "YN?")!='Y') ret
	LB_Add hlb s
	t_changed=1
	
	case 6 ;;Remove
	ARRAY(int) ai
	if(!LB_GetSelectedItems(hlb ai)) ret
	for(i n-1 -1 -1) SendMessage(hlb LB_DELETESTRING ai[i] 0)
	t_changed=1
	
	case 7 ;;Find
	LB_SelectItem(hlb -1 2) ;;deselect all
	s.getwintext(id(4 hDlg)); if(!s.len) ret
	i=LB_FindItem(hlb s 0 1); if(i<0) i=LB_FindItem(hlb s); if(i<0) ret ;;find exact or partial
	LB_SelectItem(hlb i 1)
	
	case IDOK
	LB_GetSelectedItems(hlb 0 t_selected)
	if t_changed
		for i 0 LB_GetCount(hlb)
			if(LB_GetItemText(hlb i s) and s.len) ss.addline(s)
		ss.setfile(t_file)
		t_changed=0
	
	case IDCANCEL
ret 1
