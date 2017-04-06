\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
if(!ShowDialog("DropTargetExampleDialog" &DropTargetExampleDialog &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 41 "Dialog"
 1 Button 0x54030001 0x4 4 22 46 14 "OK"
 2 Button 0x54030000 0x4 54 22 46 14 "Cancel"
 3 Edit 0x54030080 0x200 4 4 216 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020006 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	QmRegisterDropTarget(id(3 hDlg) hDlg 16)
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_QM_DRAGDROP
	QMDRAGDROPINFO& di=+lParam
	 set edit box
	str s.getl(di.files 0) ;;get first file
	s.setwintext(id(3 hDlg))
	 to get all dropped files, you can use eg foreach. Example: foreach(s di.files) out s
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
