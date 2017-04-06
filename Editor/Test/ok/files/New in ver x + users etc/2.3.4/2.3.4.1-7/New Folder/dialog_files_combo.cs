\Dialog_Editor

 Shows text files in My Documents folder in a combo box.
 When a file selected, shows its contents in a list box.

function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4"
str cb3 lb4
if(!ShowDialog("dialog_files_combo" &dialog_files_combo &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 ComboBox 0x54230243 0x0 2 4 96 213 ""
 4 ListBox 0x54230101 0x200 102 4 118 109 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030401 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
ARRAY(str)- aFiles
ARRAY(str) aLines
int i h
str s
sel wParam
	case CBN_DROPDOWN<<16|3
	 on combo dropdown, get txt files from My Documents folder and add filenames
	h=id(3 hDlg)
	GetFilesInFolder aFiles "$documents$" "*.txt"
	SendMessage h CB_RESETCONTENT 0 0
	for(i 0 aFiles.len) CB_Add h s.getfilename(aFiles[i]) i
	
	case CBN_SELENDOK<<16|3
	 on combo item selected, open the file
	h=id(3 hDlg)
	_i=CB_SelectedItem(h); if(_i<0) ret
	i=SendMessage(h CB_GETITEMDATA _i 0)
	s.getfile(aFiles[i])
	 add lines to the list box
	aLines=s
	h=id(4 hDlg)
	SendMessage h LB_RESETCONTENT 0 0
	for(i 0 aLines.len) LB_Add h aLines[i] i
	
	case LBN_SELCHANGE<<16|4
	 on list item selected, get its text and show in QM output
	h=id(4 hDlg)
	_i=LB_SelectedItem(h); if(_i<0) ret
	LB_GetItemText h _i s
	out s
	
	case IDOK
	case IDCANCEL
ret 1
