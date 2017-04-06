 /Dialog_Editor

 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 153 95 "QM MIDI triggers"
 4 ComboBox 0x54230243 0x4 2 16 148 261 ""
 6 Edit 0x54030080 0x204 2 50 148 14 ""
 8 Button 0x54032000 0x4 2 80 28 14 "Menu"
 9 Button 0x54032000 0x4 30 80 28 14 "Edit"
 10 Button 0x54032000 0x4 62 80 28 14 "Apply"
 2 Button 0x54030000 0x4 94 80 28 14 "Close"
 3 Button 0x54000001 0x4 122 80 28 14 "Exit"
 5 Static 0x54000000 0x4 3 4 144 10 "MIDI input device:"
 7 Static 0x54020000 0x4 4 38 144 10 "Menu (QM popup menu name):"
 END DIALOG
 DIALOG EDITOR: "" 0x2010200 "" ""


function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	rget(tls1 "midiinid" "" 0 -1)
	if(MT_Devices(id(4 hDlg) tls1)=0)
		mes "Number of MIDI input devices = 0" "Error" "x"
		DestroyWindow(hDlg)
		ret
	Tray-- tray.AddIcon("MIDI triggers.ico" "QM MIDI triggers" 3 hDlg)
	if(tls1>=0 and qmitem(_mt_menu 1)) MT_Start(tls1)
	else hid- hDlg; act hDlg
	
	case WM_CLOSE if(wParam) DestroyWindow(hDlg)
	case WM_DESTROY
	MT_Stop
	PostQuitMessage(hDlg)
	 
	case WM_COMMAND
	sel wParam
		case 8 mac _mt_menu; err ;;Menu
		case 9 mac+ _mt_menu; err ;;Edit
		 
		case 10 ;;Apply
		tls1=LB_SelectedItem(id(4 hDlg))
		if(tls1>=0) rset(tls1 "midiinid")
		str m.getwintext(id(6 hDlg))
		if(m.len and qmitem(m 1))
			rset m "MIDI triggers menu"; _mt_menu=m
			MT_Start(tls1)
		else mes "No such item"
		 
		case IDCANCEL min hDlg; hid hDlg; ret ;;Close
		case 3 DestroyWindow(hDlg) ;;Exit
	ret 1
	 message from tray icon:
	case (WM_USER+101)
	if(lParam=WM_LBUTTONUP) ifk(C) DestroyWindow(hDlg) else hid- hDlg; act hDlg
	else if(lParam=WM_RBUTTONUP) if(tls0) MT_Stop; else if(tls1>=0) MT_Start(tls1)
