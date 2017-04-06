\Dialog_Editor

 With snippet manager dialog.
 Unfinished (dialog proc not implemented).

function# [hDlg] [message] [wParam] [lParam]
if(hDlg) goto messages

str-- dbFile="$my qm$\snippets.db3"
str s=RtfSnippets(3 dbFile "") ;;get list of snippet names from database

s.replacerx("^((.+?)\..+[](\2\..+[])*)" ">$2[]$1<[]" 1|8) ;;add submenus

MenuPopup m
m.AddItems(s 1) ;;create menu
m.AddItems("-[]Manage...[]-[]Cancel" 10000)
int i=m.Show(0 0 1) ;;show menu and wait

sel i
	case [0,10003] ret ;;cancel
	
	case 10001 ;;manage
	str controls = "3 4"
	str lb3 rea4
	if(!ShowDialog("rtf_snippets_menu" &rtf_snippets_menu &controls)) ret
	
	case else ;;paste
	m.GetItemText(i s) ;;get selected snippet name
	RtfSnippets 1 dbFile s ;;get snippet from database, and paste

err+ mes _error.description

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "RTF Snippets"
 3 ListBox 0x54230109 0x200 2 16 96 48 ""
 4 RichEdit20A 0x54233044 0x200 2 84 220 48 ""
 5 Static 0x54000000 0x0 2 4 48 10 "Text"
 6 Static 0x54000000 0x0 2 72 48 10 "Preview"
 7 Button 0x54032000 0x0 112 4 48 14 "Add"
 8 Button 0x54032000 0x0 112 22 48 14 "Delete"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 g1
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 7 ;;add
	case 8 ;;delete
ret 1
