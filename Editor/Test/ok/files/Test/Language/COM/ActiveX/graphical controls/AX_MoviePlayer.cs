\Dialog_Editor
typelib MOVIEPLAYERLib {062F78EB-5E1D-43A7-9135-96E1392B3950} 1.0
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("AX_MoviePlayer" &AX_MoviePlayer)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 2 2 220 108 "MOVIEPLAYERLib.MoviePlayer {F4A32EAF-F30D-466D-BEC8-F4ED86CAF84E}"
 END DIALOG
 DIALOG EDITOR: "" 0x2020009 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	MOVIEPLAYERLib.MoviePlayer mo3
	mo3._getcontrol(id(3 hDlg))
	mo3.FileName="Q:\MP3\Happy Rhodes - Oh The Drears.mp3"
	mo3.Play
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
