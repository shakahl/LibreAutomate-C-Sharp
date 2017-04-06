\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 typelib ANIMATEDGIFLib {50E43D83-A74D-11D0-98CE-004005249458} 1.0
typelib ANIMATEDGIFLib "%com%\ui\MVSGif\MVSGif.ocx"

if(!ShowDialog("AniGifDlg" &AniGifDlg)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 4 Button 0x54032000 0x0 4 6 48 14 "Play"
 5 Button 0x54032000 0x0 4 24 48 14 "Play Once"
 6 Button 0x54032000 0x0 4 42 48 14 "Stop"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54000000 0x0 60 4 76 70 "ANIMATEDGIFLib.AnimatedGif"
 END DIALOG
 DIALOG EDITOR: "" 0x2020002 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	DT_Init(hDlg lParam) ;;*
	 ANIMATEDGIFLib.AnimatedGif g
	ANIMATEDGIFLib.AnimatedGif g
	g._getcontrol(id(3 hDlg))
	g.AnimatedGif=_s.expandpath("%com%\ui\MVSGif\B_LOS.GIF")

	ret 1 ;;*
	case WM_DESTROY DT_DeleteData(hDlg) ;;*
	case WM_COMMAND goto messages2
ret
 messages2
g._getcontrol(id(3 hDlg))
sel wParam
	case 4 g.Play
	case 5 g.PlayOnce
	case 6 g.Stop
	case IDOK
	DT_Ok hDlg ;;*
	case IDCANCEL DT_Cancel hDlg ;;*
ret 1

 * - not necessary in QM >= 2.1.9
