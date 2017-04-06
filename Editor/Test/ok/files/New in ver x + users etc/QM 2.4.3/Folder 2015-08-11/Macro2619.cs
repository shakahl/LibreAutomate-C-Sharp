\Dialog_Editor

 ShowDropdownListSimple(",,1[]one[]two")
 out 1

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 QM_Edit 0x54030080 0x200 8 8 96 12 ""
 4 QM_ComboBox 0x54030243 0x0 120 8 96 13 ""
 5 QM_Grid 0x56031041 0x200 120 8 96 48 "0[]A[]B"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


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

 BEGIN PROJECT
 main_function  Macro2619
 exe_file  $my qm$\Macro2619.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {EEFC1DA9-2244-4D3F-A513-B14C4CEDA605}
 END PROJECT
