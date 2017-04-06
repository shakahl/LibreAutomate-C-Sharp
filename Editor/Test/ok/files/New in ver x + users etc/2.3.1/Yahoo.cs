\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages
lpstr YM=
 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 153 78 "Dialog"
 1 Button 0x54030001 0x4 12 46 48 14 "OK"
 2 Button 0x54030000 0x4 92 46 48 14 "Cancel"
 3 ComboBox 0x54230243 0x0 12 18 130 213 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030109 "" "" ""
str controls = "3"
str cb3
if(!ShowDialog(YM &Yahoo &controls)) ret
ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam

	/ Add YM Windows To Combo
	case CBN_DROPDOWN<<16|3
	ARRAY(int) a; int i; str sc sn Final
	Final=""
	win("" "" "YAHOOMESSENGER" 0 0 0 a)
	for(i 0 a.len)
		sn.getwintext(a[i])
		if(sn != "Yahoo! Messenger")
			if(Final != "")
				Final+"[]"
				Final+sn
			else
				Final+sn
	if(Final = "")
		Final = "(No Yahoo's)"
	ARRAY(str) b=Final
	b.sort(2)
	Final=b
	/Final.replacerx(" - Instant Message")
	TO_CBFill(lParam Final)

	case IDOK
	case IDCANCEL
ret 1
