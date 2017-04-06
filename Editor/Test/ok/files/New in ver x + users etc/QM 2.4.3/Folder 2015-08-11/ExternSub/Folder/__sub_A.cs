 \Dialog_Editor
#sub Moko
function [m] ;;fff
 Annot.

int u2
out 1000
sub.Doko 8
sub.Private
sub_A.Doko


#sub Doko
function [m] ;;ddd
 Yyyyyyyy.

 deb
out "Doko %i" m
if(m) sub.Doko
if(0) end "aaa"; act "bbb";; ShowDialog


#sub Unused

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

if(!ShowDialog(dd 0 0)) ret


#sub Timer
function a b c d
out 1


#sub Set
out "Set 3"
sub.Timer(0 0 0 0)
ret 33


#sub __Hidden
function@
out "HID"


#sub Private p
out "PRIV"


#sub Dialog2
function# [hwndOwner]

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 ActiveX 0x54030000 0x0 8 8 96 48 "SHDocVw.WebBrowser {8856F961-340A-11D0-A96B-00C04FD705A2}"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

str controls = "3"
str ax3SHD
if(!ShowDialog(dd &sub.DlgProc2 &controls hwndOwner)) ret

ret 1


#sub DlgProc2
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser we3
	we3._getcontrol(id(3 hDlg))
	we3._setevents("sub.we3")
	we3.Navigate("http://www.quickmacros.com")
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1


#sub we3_DocumentComplete
function IDispatch'pDisp `&URL ;;SHDocVw.IWebBrowser2'
out URL


#sub OnErr
out "errrrrorrrr"
