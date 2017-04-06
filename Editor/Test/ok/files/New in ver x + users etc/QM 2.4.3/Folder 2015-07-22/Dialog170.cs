 /exe
 \Dialog_Editor

 ExeQmGridDll

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 QM_ComboBox 0x54030343 0x0 8 8 96 13 ""
 4 QM_ComboBox 0x54030242 0x0 8 28 96 13 ""
 6 ComboBox 0x54230842 0x0 112 8 98 0 ""
 15 RichEdit20A 0x54233044 0x200 8 52 96 14 ""
 16 RichEdit20W 0x54233044 0x200 8 62 96 14 ""
 17 RichEdit 0x54233044 0x200 8 72 96 14 ""
 18 RichEdit50W 0x54233044 0x200 8 82 96 14 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""
 5 QM_Grid 0x56031041 0x200 8 48 96 48 "0[]A[]B"

str controls = "3 4 6"
str qmcb3 qmcb4 cb6
qmcb3="0[]zero[]one[]two"
qmcb4=qmcb3

 str icons="resource:<>test.ico[]resource:<macro resources in menu>qm.ico[]resource:<macro resources - AddTrayIcon>output.ico[]resource:<Macro2194>blue.ico"
 str icons="resource:<dialog_resource_imagelist>il_de.bmp"
 str icons="image:il_qm"
str icons=":5 $qm$\il_qm.bmp"
 str icons=":1 empty.ico[]:2 copy.ico[]:3 paste.ico[]:4 cut.ico"
qmcb4=
F
 0,"{icons}"
 one,1
 two,2
 three,3

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
	case CBN_SELENDOK<<16|3
	_i=CB_SelectedItem(lParam)
	out _i
ret 1

 BEGIN PROJECT
 main_function  Dialog170
 exe_file  $my qm$\Dialog170.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  22
 guid  {4FC6C50D-629F-4DD2-BE52-CE1B385FB05C}
 END PROJECT
