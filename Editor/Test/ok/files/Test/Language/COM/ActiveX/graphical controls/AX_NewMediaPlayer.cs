\Dialog_Editor
/exe 1
function# hDlg message wParam lParam
if(hDlg) goto messages
typelib WMPLib {6BF52A50-394A-11D3-B153-00C04F79FAA6} 1.0

if(!ShowDialog("AX_NewMediaPlayer" &AX_NewMediaPlayer)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 205 184 "Dialog"
 1 Button 0x54030001 0x4 102 166 48 14 "OK"
 2 Button 0x54030000 0x4 154 166 48 14 "Cancel"
 3 ActiveX 0x54000000 0x0 4 6 198 126 "WMPLib.WindowsMediaPlayer"
 4 Button 0x54032000 0x0 4 140 48 14 "Play"
 5 Button 0x54032000 0x0 56 140 48 14 "Stop"
 END DIALOG
 DIALOG EDITOR: "" 0x2020008 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	WMPLib.WindowsMediaPlayer mp
	mp._getcontrol(id(3 hDlg))
	 mp.openPlayer("Q:\MP3\Happy Rhodes - Oh The Drears.mp3")
	 mp.launchURL("Q:\MP3\Happy Rhodes - Oh The Drears.mp3")
	 WMPLib.WMPCore c.
	 WMPLib.IWMPMedia m=mp.newMedia("Q:\MP3\Happy Rhodes - Oh The Drears.mp3")
	 mp.launchURL("Q:\MP3\Happy Rhodes - Oh The Drears.mp3")
	 out m.
	 mp.URL="Q:\MP3\Happy Rhodes - Oh The Drears.mp3"
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	mp._getcontrol(id(3 hDlg))
	mp.URL="Q:\MP3\Happy Rhodes - Oh The Drears.mp3"
	case 5
	mp._getcontrol(id(3 hDlg))
	mp.URL=""

	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  AX_NewMediaPlayer
 exe_file  $my qm$\AX_NewMediaPlayer.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {CC75CB94-08D5-4913-B891-70EA8448DDC4}
 END PROJECT
