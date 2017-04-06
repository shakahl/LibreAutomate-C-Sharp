/exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 5 6 7 8"
str c3Che e5 cb6 cb7 cb8
if(!ShowDialog("" &Dialog109 &controls 0 0 0 0 0 -1 -1)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Button 0x5C012003 0x0 34 16 48 12 "Check"
 4 Button 0x54032000 0x0 46 66 48 14 "Button"
 5 Edit 0x54030080 0x200 26 70 96 14 ""
 6 ComboBox 0x54230242 0x0 120 12 96 213 ""
 7 ComboBox 0x54230641 0x0 122 28 96 34 ""
 8 ComboBox 0x54230641 0x0 126 66 94 38 ""
 5 Edit 0x54030080 0x200 26 100 96 14 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x204 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	int c=id(3 hDlg)
	out but(c)
ret 1

 BEGIN PROJECT
 main_function  Dialog109
 exe_file  $my qm$\Dialog109.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {59A612DA-590F-4CE9-81D3-9F509D4CE3FE}
 END PROJECT
