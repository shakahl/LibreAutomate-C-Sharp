\Dialog_Editor
typelib MP3PLib {707A5991-8950-11D2-9E58-00A024A8859A} 1.0
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("AX_Mp3Player" &AX_Mp3Player)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 6 4 144 96 "MP3PLib.Mp3P"
 END DIALOG
 DIALOG EDITOR: "" 0x2020008 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	MP3PLib.Mp3P mp3
	mp3._getcontrol(id(3 hDlg))
	 mp3.Init
	mp3.InputOpen("Q:\MP3\Happy Rhodes - Oh The Drears.mp3")
	mp3.Play
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
