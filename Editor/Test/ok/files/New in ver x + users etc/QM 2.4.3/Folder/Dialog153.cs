\Dialog_Editor

int maxChar=40 ;;change to 200 if need
str text
text=
 Hi there,
 Is it possible to grab a text, split these in like 200 characters with whole words and when i press a button part1 is set to an object. Another press gives part2 till the text is 'empty' ?
text.wrap(maxChar "" "" 1)

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 184 246 "Dialog"
 3 Button 0x54032000 0x0 8 8 48 14 "Button"
 4 ComboBox 0x54230641 0x0 8 28 168 213 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

str controls = "4"
str cb4
cb4=text
cb4-"&" ;;select the first
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	int hcb=id(4 hDlg)
	str s.getwintext(hcb)
	out s
	int i=CB_SelectedItem(hcb)
	CB_SelectItem(hcb i+1)
ret 1
