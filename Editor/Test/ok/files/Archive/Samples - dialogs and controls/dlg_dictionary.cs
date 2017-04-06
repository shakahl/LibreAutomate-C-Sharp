\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 this code just creates a test file
str s=
 January Sausis
 February Vasaris
 March Kovas
s.setfile("$desktop$\test dictionary.txt")
 ___________________________________

str ss.getfile("$desktop$\test dictionary.txt")
IStringMap- m=CreateStringMap(1|2)
m.AddList(ss " ")
ss.all

str controls = "3 4 8"
str e3 e4 e8
e8="January[]February[]March"
if(!ShowDialog("dlg_dictionary" &dlg_dictionary &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dictionary"
 3 Edit 0x54030080 0x200 8 22 96 14 ""
 4 Edit 0x54030080 0x200 108 22 96 14 ""
 5 Static 0x54000000 0x0 8 6 48 12 "English"
 6 Static 0x54000000 0x0 108 6 48 12 "Lithuanian"
 7 Static 0x54000000 0x0 8 42 60 12 "Available words"
 8 Edit 0x54230844 0x20000 8 56 96 48 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020006 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case EN_CHANGE<<16|3
	str s1.getwintext(lParam) s2
	if(s1.len) m.Get2(s1 s2)
	s2.setwintext(id(4 hDlg))
	case IDOK
	case IDCANCEL
ret 1
