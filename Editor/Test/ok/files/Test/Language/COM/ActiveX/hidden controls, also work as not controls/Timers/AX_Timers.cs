\Dialog_Editor
 WORKS
function# hDlg message wParam lParam
if(hDlg) goto messages
 typelib A16_Timers "%com%\ui\Timers\A16_Tmrs.ocx"
typelib April16_Timers {31946E9C-3E50-4A83-9710-EB0019066670} 2.0

if(!ShowDialog("AX_Timers" &AX_Timers)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 0 0 42 32 "April16_Timers.STimer {81E3E8A2-6B68-45DE-AD21-2109A6921185}"
 END DIALOG
 DIALOG EDITOR: "" 0x2020009 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	April16_Timers.STimer t._getcontrol(id(3 hDlg))
	t._setevents("t___STimer")
	word p=1
	t.Interval=p
	word e=-1
	t.Enabled=e

	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
