 /exe
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

goto g1
WindowText wt.Capture
 Sqlite sq
 __Tcc tcc
 SendMail "" "" ""
 ReceiveMail "" 0 ""
 Services.clsService serv._create
 Services.clsService serv._create(0 "$qm$\ARServicesMgr.DLL")

 #exe addfile "$qm$\mouse.ico" 5

 g1
if(!ShowDialog("dialog_make_exe_dll_detect" &dialog_make_exe_dll_detect 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 4 ActiveX 0x54030000 0x0 116 6 96 48 "SHDocVw.WebBrowser"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""
 3 QM_Grid 0x56031041 0x0 6 6 96 48 "0x0,0,0,0,0x0[]A,,,[]B,,,"

ret
 messages
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
 main_function  dialog_make_exe_dll_detect
 exe_file  $my qm$\dialog_make_exe_dll_detect.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 version  
 version_csv  
 flags  6
 end_hotkey  0
 guid  {2BDBC0A6-C78B-4369-9EEC-E78D9CC21861}
 END PROJECT
