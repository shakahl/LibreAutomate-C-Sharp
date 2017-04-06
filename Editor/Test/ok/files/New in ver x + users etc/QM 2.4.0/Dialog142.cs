/exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog142" &Dialog142 0)) ret

 BEGIN DIALOG
 1 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040001 "" "" "" ""
 3 RichEdit20A 0x54233044 0x200 0 0 96 48 ""
 4 RichEdit20W 0x54233044 0x200 100 0 96 48 ""
 5 RichEdit 0x54233044 0x200 0 50 96 48 ""
 6 QM_Grid 0x56031041 0x200 100 50 96 48 "0[]A[]B"
 7 NoClass 0x56031041 0x200 100 50 96 48 "0[]A[]B"


ret
 messages
sel message
	case WM_INITDIALOG
	 outw CreateControl(0 "RichEdit20W" 0 0 0 0 100 100 hDlg 3)
	 outw CreateControl(0 "RichEdit20A" 0 0 0 0 100 100 hDlg 3)
	 outw CreateControl(0 "RichEdit" 0 0 0 0 100 100 hDlg 3)
	 outw CreateControl(0 "QM_Grid" 0 0 0 0 100 100 hDlg 3)
	 outw CreateControl(0 "NoClass" 0 0 0 0 100 100 hDlg 3)
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  Dialog142
 exe_file  $my qm$\Dialog142.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {95244900-0518-4378-9D6F-40F752E35FD3}
 END PROJECT
