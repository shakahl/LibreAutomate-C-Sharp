\Dialog_Editor
typelib MediaPlayerLib {A6B0CE5F-A800-4AB6-BBD2-3141CB154B5E} 1.0
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("AX_GogoPlayer" &AX_GogoPlayer)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 4 4 216 110 "MediaPlayerLib.MediaPlayer"
 END DIALOG
 DIALOG EDITOR: "" 0x2020008 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	MediaPlayerLib.MediaPlayer mp._getcontrol(id(3 hDlg))
	mp.FileName="Q:\MP3\Happy Rhodes - Oh The Drears.mp3"
	mp.Start
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
