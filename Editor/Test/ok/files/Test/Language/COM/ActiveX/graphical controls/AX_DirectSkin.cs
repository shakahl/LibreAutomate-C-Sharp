/exe
 \Dialog_Editor
 Note: normal license costs $9000
typelib WBOCXLib {52BD8A52-B792-4C45-A4D9-245CC945AC34} 1.0
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("AX_DirectSkin" &AX_DirectSkin)) ret

 BEGIN DIALOG
 0 "" 0x90CC0A44 0x100 0 0 219 132 "Dialog"
 4 Button 0x54030000 0x4 120 116 48 14 "Skin"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 0 84 96 48 "WBOCXLib.Wbocx {55D94814-5664-4D04-9804-74DD038D4BA3} data:FF3B93B57DCD0595FDC248CD079B4CD6FEB4AEEC2770020B0DBA498E57D90EE6021BB8759574391802"
 END DIALOG
 DIALOG EDITOR: "" 0x2020009 "" ""
 3 ActiveX 0x54030000 0x0 0 0 22 20 "WBOCXLib.Wbocx {55D94814-5664-4D04-9804-74DD038D4BA3}"

ret
 messages
sel message
	case WM_INITDIALOG
	 WBOCXLib.Wbocx-- wb3._create
	WBOCXLib.Wbocx-- wb3
	wb3._getcontrol(id(3 hDlg))
	wb3.InitWB
	 out wb3.SupportMultiThread ;;0
	 wb3.AddExcludeHwnd(_hwndqm)
	 wb3.SkinAllThreadsOff
	wb3.SetRootPathStr("C:\Program Files\Stardock\DirectSkin\samples\skins\")
	 wb3.LoadUIS("dogmax\dogmax.uis")
	wb3.LoadUIS("ketix\ketix.uis")
	wb3.DoWindow(hDlg)
	wb3.ReloadUIS ;;Without this, skinning does not work, unless running in exe. The same in VB. But with this QM window is distorted. Tested only on Vista.
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	wb3._getcontrol(id(3 hDlg))
	wb3.ReloadUIS
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  AX_DirectSkin
 exe_file  $my qm$\AX_DirectSkin.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {7F9213F3-25D4-43ED-880B-035E9F8480ED}
 END PROJECT
