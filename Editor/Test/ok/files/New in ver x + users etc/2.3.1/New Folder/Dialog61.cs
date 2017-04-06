 /Macro966
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

typelib Project2 {EB35919D-393D-4471-B747-C28F5DED9BCF} 2.0 ;;Simple control that I created with VB6

out
  if(!ShowDialog("" 0)) ret
 if(!ShowDialog("" &Dialog61)) ret
 ret

 int h=ShowDialog("" 0 0 0 1)
int h=ShowDialog("" &Dialog61 0 0 1)
opt waitmsg 1
wait 3
clo h
 0
out 1
 SendMessage h WM_CLOSE 1 0
 SendMessage h WM_CLOSE 1 0
 0.5

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000000 0x0 12 18 48 12 "Text"
 4 ActiveX 0x54030000 0x0 0 87 96 48 "Project2.UserControl1 {E46F9BB9-8BD9-4AAA-BD7D-1DFD779EC13F}"
 END DIALOG
 DIALOG EDITOR: "" 0x2020009 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_CLOSE
	out "close"
	case WM_DESTROY
	out "destr"
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	out "cancel"
	 mes 1
ret 1
