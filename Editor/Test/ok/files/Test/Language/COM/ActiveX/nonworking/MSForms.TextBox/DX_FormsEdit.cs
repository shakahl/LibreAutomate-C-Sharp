\Dialog_Editor
 WORKS BUT DOES NOT ALLOW TO EDIT TEXT. Using ATL even worse. In VS, cannot add to ATL and MFC dialogs. In VB works well.
function# hDlg message wParam lParam
if(hDlg) goto messages
typelib MSForms {0D452EE1-E08F-101A-852E-02608C4D0BB4} 2.0

out
if(!ShowDialog("DX_FormsEdit" &DX_FormsEdit)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 3 ActiveX 0x54030000 0x0 8 6 120 14 "MSForms.TextBox"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020008 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	MSForms.TextBox te3
	te3._getcontrol(id(3 hDlg))
	te3._setevents("te3_MdcTextEvents")
	VARIANT v="kk"
	te3.Value=v
	 out te3.Enabled
	 out te3.Locked
	out "-----------------------------------------"
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	te3._getcontrol(id(3 hDlg))
	out te3.Value
	
	case IDCANCEL
ret 1
