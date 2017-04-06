\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
if(!ShowDialog("Dialog96" &Dialog96 &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Edit 0x54231044 0x200 0 0 224 114 ""
 4 Button 0x54032000 0x0 4 118 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030301 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4 goto g1
ret 1

 g1
dll "qm.exe" TestGT h noInternal str*s1 str*s2 str*s3

 ----
 int w=win("Dialog" "#32770")
 int c=id(3 w)
 ----
 int w=win("" "QM_Editor")
 int c=id(2216 w)
 ----
 int w=win("Options" "#32770")
 int c=id(1103 w)
 ----
 int w=win("Dialog" "#32770")
 int c=id(4 w)
 ----
int c=id(4 hDlg)

str s1 s2 s3
TestGT c 0 &s1 &s2 &s3
out s1
out s2
out s3
