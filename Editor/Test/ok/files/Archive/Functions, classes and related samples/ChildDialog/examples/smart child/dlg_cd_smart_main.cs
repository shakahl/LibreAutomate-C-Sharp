\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 ---- child dialog ----

type DCSCONTROLS842 ~controls ~o3Opt ~o4Opt ~o5Opt ~e6 ~e9 ~si10 ~c12Che
DCSCONTROLS842 d.controls="3 4 5 6 9 10 12"

 init some controls
d.o3Opt=1
d.si10="&shell32.dll,23"
for(_i 0 100) d.e6.formata("%i[]" _i)

 init ChildDialog variable
#compile __ChildDialog
ChildDialog- cd
#exe addtextof "dlg_cd_smart_child"
cd.Init(3 "dlg_cd_smart_child" &dlg_cd_smart_child &d 0 0xe0ffff)

 ---- main dialog ----

str controls = "4"
str c4Che

if(!ShowDialog("dlg_cd_smart_main" &dlg_cd_smart_main &controls)) ret

out "c4Che=%s, d.e9=%s, d.c12Che=%s, d.o3Opt=%s, d.o4Opt=%s, d.o5Opt=%s" c4Che d.e9 d.c12Che d.o3Opt d.o4Opt d.o5Opt

 BEGIN DIALOG
 0 "" 0x90C80A48 0x10100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x44000000 0x0 4 4 216 106 ""
 4 Button 0x54012003 0x0 40 116 48 13 "Check"
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "*" "" ""

ret
 messages
cd.Message(hDlg message wParam lParam)
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
