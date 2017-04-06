\Dialog_Editor
function# hDlg message wParam lParam
#if _winver<=0x600 ;;removed from Win7
if(hDlg) goto messages
typelib AgentObjects {F5BE8BC2-7DE6-11D0-91FE-00C04FD701A5} 2.0

 Actually this component does not have to be in a dialog. This is only for testing.
 At least one character should be installed. On XP and Vista it is merlin. More characters can be downloaded.

if(!ShowDialog("AX_MsAgent" &AX_MsAgent)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 88 112 22 20 "AgentObjects.Agent {D45FD31B-5C6E-11D1-9EC1-00C04FD7081F}"
 4 Button 0x54032000 0x0 4 6 84 14 "Set default character..."
 END DIALOG
 DIALOG EDITOR: "" 0x2020009 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	AgentObjects.Agent a
	a._getcontrol(id(3 hDlg))
	a._setevents("a__AgentEvents")
	a.Connected=-1 ;;connect to the MS Agent server
	a.Characters.Load("a") ;;load default character
	 a.Characters.Load("a" "merlin.acs") ;;load merlin
	a.Characters.Item("a").Show
	
	 AgentObjects.IAgentCtlCharacterEx c
	 foreach c a.Characters
		 out c.Name
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	a._getcontrol(id(3 hDlg))
	a.ShowDefaultCharacterProperties
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  AX_MsAgent
 exe_file  $my qm$\AX_MsAgent.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {077CB49F-C45A-4E40-B4D5-342EDEEBD8F5}
 END PROJECT
