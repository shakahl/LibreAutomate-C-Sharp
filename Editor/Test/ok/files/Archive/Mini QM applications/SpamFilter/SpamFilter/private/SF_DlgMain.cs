 \Dialog_Editor
function# hDlg message wParam lParam

 BEGIN DIALOG
 0 "" 0x10CA0A48 0x100 0 0 493 330 "QM SpamFilter"
 2 Button 0x54030001 0x4 442 314 48 14 "Close"
 4 Button 0x54032000 0x4 394 314 48 14 "Email app"
 15 Button 0x54032000 0x4 346 314 48 14 "Check now"
 11 Button 0x54010003 0x4 212 314 46 14 "Edit mode"
 16 Button 0x54032000 0x4 100 314 18 15 "?"
 12 Button 0x54032000 0x4 52 314 48 15 "Options ..."
 14 Button 0x54032000 0x4 4 314 48 15 "Filters ..."
 8 Static 0x54020000 0x4 2 4 28 13 "Mailbox"
 3 SysListView32 0x54000001 0x204 32 2 458 72 "goo"
 9 Static 0x54020000 0x4 2 80 28 12 "Deleted"
 6 SysListView32 0x54000001 0x204 32 78 458 146 "spa"
 18 Button 0x54039009 0x4 2 229 28 13 "Text"
 19 Button 0x54009009 0x4 2 242 28 13 "Errors"
 17 Edit 0x54231044 0x204 32 228 458 72 "tex"
 13 Button 0x54032000 0x4 2 112 18 14 "..."
 7 SysListView32 0x54000001 0x204 32 228 458 72 "err"
 5 Static 0x54000010 0x20004 4 306 486 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010500 "" ""

 messages
sel message
	case WM_INITDIALOG
	int+ __sfmain=hDlg
	tray.AddIcon("sf.ico[]sf checking.ico[]sf new.ico" "QM SpamFilter" 3 hDlg)
	SF_GetOptions
	if(o.flags&0x100) CheckDlgButton hDlg 11 1
	SF_Update 1
	SF_SetTimer o.period0
	SetTimer hDlg 5 1000 0
	
	case WM_TIMER
	sel wParam
		case 0
		if(o.flags&0x100 and !hid(hDlg) and timer) ret
		if(!checking) checking=1; else SF_SetTimer 60; ret
		if(o.flags&4 and !IntIsConnected) if(timer!=60) SF_SetTimer 60
		else SF_Check; if(timer!=o.period*60) SF_SetTimer o.period*60
		checking=0
		if(quit) PostQuitMessage 0
		
		case 5 if(o.flags&8 and WaitForSingleObject(ev 0)=0) SF_SetTimer 1
		
		case 10
		if(checking) KillTimer hDlg 10; ret
		if(!a.len) KillTimer hDlg 10; tray.Modify(1 "QM SpamFilter"); ret
		ictimer+1; if(ictimer>30) KillTimer hDlg 10; tray.Modify(3); ret
		tray.Modify(iif(ictimer&1 1 3))
	
	case WM_CLOSE if(wParam) DestroyWindow(hDlg)
	
	case WM_DESTROY
	quit=1
	if(p.p.Busy) p.p.Abort
	PostQuitMessage 0
	
	case WM_COMMAND goto messages2
	
	case WM_USER+101 ;;tray icon
	sel lParam
		case WM_LBUTTONUP
		sel(GetMod)
			case 0 act hDlg
			case 2 DestroyWindow hDlg
		case WM_RBUTTONUP SF_Menu
		
	case WM_USER ;;menu
	sel wParam
		case 1 SF_SetTimer
		case 2 run o.mailapp
	
	case WM_WINDOWPOSCHANGED
	WINDOWPOS* wp=+lParam
	if(wp.flags&SWP_SHOWWINDOW) SF_Update
	
	case WM_NOTIFY ret SF_wm_notify(hDlg wParam +lParam)
	
	case else
	if(message=___newtaskbar) tray.AddIcon("" "" 8)
	
ret
 messages2
sel wParam
	case [18,19] TO_Show hDlg "17 -7" wParam=18
	case 14 ;;Filters
#if !EXE
	if(!o.ff.len) ret
	_i=PopupMenu(o.ff); if(!_i) ret
	_s.getl(o.ff _i-1)
	mac+ _s
#else
	 should show a dialog
#endif
	
	case 11 ;;mode
	if(but(11 hDlg)) o.flags|0x100; else o.flags~0x100
	rset o.flags "flags" "\SpamFilter"
	
	case 12 SF_DlgOptions 0 0 hDlg
	case 13 run SF_DIR
	case 16 ShowText "SpamFilter Help" _s.getmacro("SpamFilter Help")
	case 15 SF_SetTimer
	case 4 min hDlg; hid hDlg; run o.mailapp
	case IDCANCEL min hDlg; hid hDlg; ret
err+
ret 1
