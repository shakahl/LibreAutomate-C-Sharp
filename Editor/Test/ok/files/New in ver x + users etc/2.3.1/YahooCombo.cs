\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages
str controls = "3"
str cb3
if(!ShowDialog("YahooCombo" &YahooCombo &controls)) ret
 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 152 78 "Yahoo! Combo"
 1 Button 0x54030001 0x4 14 48 48 14 "OK"
 2 Button 0x54030000 0x4 88 48 48 14 "Cancel"
 3 ComboBox 0x54230243 0x0 12 12 126 213 ""
 END DIALOG
 DIALOG EDITOR: "" 0x203000D "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case CBN_SELENDOK<<16|3
	_i=CB_SelectedItem(lParam)
	case CBN_DROPDOWN<<16|3
		ARRAY(int) a; int i; str sc sn Final
		Final=""
		win("" "" "NOTEPAD" 0 0 0 a)
		for(i 0 a.len)
			sn.getwintext(a[i])
			out sn
			if(sn != "Untitled - Notepad")
				if(Final != "")
					Final+"[]"
					Final+sn
				else
					Final+sn
		if(Final = "")
			Final = "(No Yahoo Window Is Open)"
		ARRAY(str) b=Final
		b.sort(2)
		Final=b
		TO_CBFill(lParam Final)
	case IDOK
	case IDCANCEL
ret 1
