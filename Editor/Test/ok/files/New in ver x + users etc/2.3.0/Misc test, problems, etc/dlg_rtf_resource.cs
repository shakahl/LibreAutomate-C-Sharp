 /exe
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4 5"
str rea3 si4 si5
 rea3="&$desktop$\Document.rtf"
rea3="&:9 $desktop$\Document.rtf"
si4=":2 shell32.dll,4"
si5="&:2 shell32.dll,4"
if(!ShowDialog("dlg_rtf_resource" &dlg_rtf_resource &controls)) ret
out rea3

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 RichEdit20A 0x54233044 0x200 0 0 226 102 ""
 4 Static 0x54000003 0x0 2 106 16 16 ""
 5 Static 0x54000003 0x0 26 106 16 16 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "*" "" ""

ret
 messages
 lpstr f="$desktop$\Document.rtf"
 lpstr f=":10 $desktop$\Document.rtf"
lpstr f=":10 $desktop$\test.txt"
sel message
	case WM_INITDIALOG
	 out
	 out RichEditLoad(id(3 hDlg) f)
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	 out RichEditSave(id(3 hDlg) f)
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  dlg_rtf_resource
 exe_file  $my qm$\dlg_rtf_resource.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  22
 end_hotkey  0
 guid  {756AE563-FF0F-428F-8AE2-8ED8B28A3A69}
 END PROJECT
