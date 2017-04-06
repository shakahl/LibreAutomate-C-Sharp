\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

typelib ARGradientControl "%com%\ui\\ARGradient.ocx"

if(!ShowDialog("DlgARGradient" &DlgARGradient 0)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54000000 0x0 4 6 96 48 "ARGradientControl.ARGradient"
 END DIALOG
 DIALOG EDITOR: "" 0x2010700 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG
	ARGradientControl.ARGradient ar3._getcontrol(id(3 hDlg))
	ar3._setevents("ar3___ARGradient")
	
	stdole.Font f=ar3.Font
	 ar3.Font. ;;why no list?
	f.Bold=TRUE
	f.Size=30
	f.Name="Comic Sans MS"
	
	 ar3.Color=stdole.FONTITALIC	
	
	BSTR cap="Hello"
	ar3.Caption=&cap
	word w=TRUE
	ar3.ShowCaption=&w
	
	word move=-1
	ar3.MoveForm=&move
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
