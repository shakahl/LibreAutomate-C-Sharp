 \Dialog_Editor
function# hDlg message wParam lParam

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 172 "Dialog"
 5 Static 0x54000000 0x0 4 4 40 13 "Folder"
 6 Edit 0x54030080 0x200 46 4 174 14 ""
 9 Button 0x54032000 0x0 202 20 18 14 "..."
 7 Static 0x54000000 0x0 4 20 40 13 "File pattern"
 8 Edit 0x54030080 0x200 46 20 112 14 ""
 3 ListBox 0x54230109 0x200 4 38 216 114 ""
 1 Button 0x54030001 0x4 60 156 48 14 "OK"
 2 Button 0x54030000 0x4 112 156 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "" ""

 messages
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 100 0
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_TIMER
	sel wParam
		case 1
		KillTimer hDlg 1
		int hlb=id(3 hDlg)
		SendMessage hlb LB_RESETCONTENT 0 0
		str sf.getwintext(id(6 hDlg)) sp.getwintext(id(8 hDlg))
		if(sf.len)
			sf.from(sf "\" iif(sp.len sp "*"))
			Dir d
			foreach(d sf FE_Dir)
				SendMessage hlb LB_ADDSTRING 0 d.FileName
ret
 messages2
sel wParam
	case [EN_CHANGE<<16|6,EN_CHANGE<<16|8]
	SetTimer hDlg 1 1000 0
	
	case 9 ;;...
	str sm=
  /dontrun /expandfolders 0x39 0
 Desktop ":: "
 C: "c:\"
	if(DynamicMenu(sm "" 1))
		qm.GetLastSelectedMenuItem 0 &sf 0
		sf.expandpath(sf 2)
		sf.setwintext(id(6 hDlg))
	
	case IDOK
	ARRAY(str)- t_af
	LB_GetTextOfSelectedItems id(3 hDlg) t_af
	case IDCANCEL
ret 1
