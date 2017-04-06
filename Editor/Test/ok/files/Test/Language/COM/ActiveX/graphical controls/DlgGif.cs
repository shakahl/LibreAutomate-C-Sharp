\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 typelib GIF89Lib {28D47522-CF84-11D1-834C-00A0249F0C28} 1.0 ;;if registered
typelib GIF89Lib "%com%\ui\gif89\GIF89.DLL" ;;if on desktop

if(!ShowDialog("DlgGif" &DlgGif 0)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54000000 0x4 2 4 58 53 "GIF89Lib.Gif89a"
 4 Button 0x54032000 0x0 0 58 48 14 "Play"
 5 Button 0x54032000 0x0 50 58 48 14 "Stop"
 END DIALOG
 DIALOG EDITOR: "" 0x2020002 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG
	GIF89Lib.Gif89a gi3
	gi3._getcontrol(id(3 hDlg))
	 gi3.Glass=TRUE
	 gi3.AutoStart=0
	gi3.FileName=_s.expandpath("%com%\ui\gif89\B_LOS.GIF")
	 hid- id(3 hDlg)
	
	ret 1
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	gi3._getcontrol(id(3 hDlg))
	gi3.Play
	 hid- id(3 hDlg)
	case 5
	 hid id(3 hDlg)
	gi3._getcontrol(id(3 hDlg))
	gi3.Stop
	case IDOK
ret 1
