\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 for testing OutWinMsg


str controls = "3 4 6 7 8"
str e3 c4Che cb6 lb7 rea8
if(!ShowDialog("dlg_control_messages" &dlg_control_messages &controls 0 0 0 0 0 -1)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 334 260 "Dialog"
 2 Button 0x54030000 0x4 136 242 48 14 "Cancel"
 3 Edit 0x54030080 0x200 4 4 40 13 ""
 4 Button 0x54012003 0x0 96 4 48 13 "Check"
 5 Static 0x54000100 0x0 148 4 28 13 "Text"
 6 ComboBox 0x54230243 0x0 180 4 48 213 ""
 7 ListBox 0x54230101 0x200 230 4 48 26 ""
 8 RichEdit20W 0x54233044 0x200 46 4 48 26 ""
 9 ComboBoxEx32 0x54030000 0x200 0 44 86 54 ""
 11 msctls_hotkey32 0x54030000 0x200 90 44 48 14 ""
 12 msctls_progress32 0x54030000 0x0 142 44 48 13 ""
 13 msctls_statusbar32 0x54030000 0x0 2 186 334 14 ""
 14 msctls_trackbar32 0x54030000 0x0 194 44 46 14 ""
 15 msctls_updown32 0x54030000 0x0 246 44 11 16 ""
 16 ReBarWindow32 0x54030001 0x0 0 1 334 0 ""
 17 ScrollBar 0x54030000 0x0 284 6 48 10 ""
 18 SysAnimate32 0x54030000 0x0 262 44 36 18 ""
 19 SysDateTimePick32 0x56030000 0x0 90 62 52 14 "2009.03.03"
 20 SysHeader32 0x54030000 0x0 148 62 32 12 ""
 21 SysIPAddress32 0x54030000 0x200 190 62 56 14 "0.0.0.0"
 22 SysListView32 0x54030000 0x0 2 116 60 48 ""
 23 SysMonthCal32 0x54030000 0x0 68 116 96 48 ""
 24 SysPager 0x54030000 0x0 252 66 28 26 ""
 25 SysTabControl32 0x54030040 0x0 170 116 46 46 ""
 26 SysTreeView32 0x54030000 0x0 222 116 56 46 ""
 27 ToolbarWindow32 0x54030001 0x0 2 166 334 17 ""
 28 Button 0x54032000 0x0 4 204 48 14 "Button"
 10 Static 0x54000010 0x20000 0 38 379 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "*" "" ""

ret
 messages
OutWinMsg message wParam lParam
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 28
	 out
	 ARRAY(int) a; int i j h
	 child "" "" hDlg 0 0 0 a
	 for i 0 a.len
		 h=a[i]
		 j=SendMessage(h WM_GETOBJECT 0 OBJID_QUERYCLASSNAMEIDX)
		 str s.getwinclass(h)
		 out "%s  %i" s j
	
	 SendMessage hDlg 0 0 0
	 SendMessage hDlg RegisterWindowMessage("wm_qm_abc") 0 0
	 SendMessage hDlg WM_USER+100 0 0
	 SendMessage hDlg WM_APP+100 0 0
	 SendMessage hDlg 1111111111 0 0
	case IDOK
	case IDCANCEL
ret 1
