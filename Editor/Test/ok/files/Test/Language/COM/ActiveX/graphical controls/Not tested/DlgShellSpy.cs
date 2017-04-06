\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

typelib MBShellSpy {AD9E813C-FCE6-11D3-8ED3-00E07D815373} 1.0

if(!ShowDialog("DlgShellSpy" &DlgShellSpy 0)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54000000 0x0 0 87 96 48 "MBShellSpy.ShellSpy"
 END DIALOG
 DIALOG EDITOR: "" 0x2010700 "*" ""

ret
 messages
sel message
	case WM_INITDIALOG DT_Init(hDlg lParam)
	MBShellSpy.ShellSpy sh3._getcontrol(id(3 hDlg))
	 sh3.FolderToWatch=
	sh3._setevents("sh3___ShellSpy")
	sh3.Enabled=-1
	sh3.FolderEvents=-1
	 sh3.FolderPath
	
	ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
