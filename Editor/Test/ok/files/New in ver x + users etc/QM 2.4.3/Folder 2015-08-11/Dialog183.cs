 \Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 632 320 "Dialog"
 3 Button 0x54032000 0x0 8 56 48 14 "Button"
 4 Button 0x54012003 0x0 60 56 48 10 "Check"
 5 Edit 0x54030080 0x200 112 56 96 12 ""
 6 Static 0x54000000 0x0 8 72 48 12 "Text"
 8 Button 0x54032009 0x0 60 72 58 10 "Option first"
 9 Button 0x54002009 0x0 124 72 58 10 "Option next"
 10 Edit 0x54231044 0x200 120 88 96 48 ""
 11 Edit 0x54030020 0x200 8 104 96 12 ""
 12 Edit 0x54032000 0x200 8 120 32 12 ""
 13 Edit 0x54200844 0x20000 8 136 96 48 ""
 14 RichEdit20A 0x54233044 0x200 116 140 96 48 ""
 15 QM_Edit 0x54030080 0x200 8 188 96 13 ""
 16 ComboBox 0x54230243 0x0 8 204 96 213 ""
 17 ComboBox 0x54230242 0x0 8 220 96 213 ""
 18 ComboBox 0x54230641 0x0 8 236 96 48 ""
 19 ListBox 0x54230101 0x200 116 196 96 48 ""
 20 ListBox 0x54230109 0x200 116 244 96 48 ""
 21 QM_ComboBox 0x54030243 0x0 4 284 96 13 ""
 22 QM_ComboBox 0x54030242 0x0 8 300 96 13 ""
 23 Static 0x54000003 0x0 8 36 16 16 ""
 24 Static 0x5400000E 0x0 32 36 16 16 ""
 27 SysLink 0x54030000 0x0 52 28 96 13 "<a>Link1</a>, <a>link2</a>"
 28 SysDateTimePick32 0x56030000 0x0 220 4 96 13 ""
 29 ComboBoxEx32 0x54230042 0x0 220 20 96 214 ""
 30 msctls_hotkey32 0x54030000 0x200 220 36 96 13 ""
 31 msctls_progress32 0x54030000 0x0 220 52 96 13 ""
 32 msctls_statusbar32 0x54030000 0x0 0 306 464 14 ""
 33 msctls_trackbar32 0x54030000 0x0 220 68 96 16 ""
 34 msctls_updown32 0x54030000 0x0 220 88 11 12 ""
 35 RICHEDIT50W 0x54233044 0x200 220 104 96 48 ""
 36 ScrollBar 0x54030000 0x0 224 156 96 13 ""
 37 SysHeader32 0x54030000 0x0 224 172 96 13 ""
 38 SysIPAddress32 0x54030000 0x200 224 188 96 13 ""
 39 SysListView32 0x54030000 0x0 224 204 96 48 ""
 40 SysMonthCal32 0x54030000 0x0 324 4 102 98 ""
 42 SysTreeView32 0x54030000 0x0 224 256 96 48 ""
 43 ToolbarWindow32 0x54030001 0x0 324 164 464 18 ""
 44 ActiveX 0x54030000 0x0 324 184 96 48 "MSDataListLib.DataList {F0D2F219-CCB0-11D0-A316-00AA00688B10} data:F822915ABC6829D3BED0DC96B231561A79D32EF7C2AE5D6EADA54B8A236FBEAD53106463C252301304"
 45 ActiveX 0x54030000 0x0 324 240 96 48 "MSComCtl2.DTPicker {20DD1B9E-87C4-11D1-8BE3-0000F8754DA1} data:4ECA0AFEA5E22D8F0E05C917DEF12E7DF7715697FB63C56E3D6C70EE94C9B0CE8799AD6F3BA33EC204"
 46 ActiveX 0x54030000 0x0 432 8 96 48 "SHDocVw.WebBrowser {8856F961-340A-11D0-A96B-00C04FD705A2}"
 47 QM_Grid 0x56031041 0x200 432 64 96 48 "0x0,0,0,0x0,0x0[]A,,,[]B,,,[]C,,,"
 48 QM_DlgInfo 0x54000000 0x20000 432 112 96 48 ""
 7 Button 0x54020007 0x0 112 216 108 100 ""
 1 Button 0x54030001 0x0 8 88 48 14 "OK"
 2 Button 0x54030000 0x0 60 88 48 14 "Cancel"
 25 Static 0x54000010 0x20000 52 44 96 2 ""
 26 Static 0x54000011 0x20000 156 36 1 16 ""
 41 SysTabControl32 0x54030040 0x0 324 108 96 48 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

str controls = "4 5 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 35 46 47"
str c4Che e5 o8Opt o9Opt e10 e11 e12 e13 re14 qme15 cb16 cb17 cb18 lb19 lb20 qmcb21 qmcb22 si23 sb24 re35 ax46SHD qmg47x
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
	case IDOK
	case IDCANCEL
ret 1
