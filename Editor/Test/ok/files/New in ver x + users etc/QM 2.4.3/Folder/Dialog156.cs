\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 348 222 "Dialog1"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	PostMessage hDlg WM_APP 0 0
	case WM_APP
	sub.Dialog2 hDlg
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1


#sub Dialog2
function# [hwndOwner]

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x40000 0 0 224 136 "Dialog2"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc2 0 hwndOwner)) ret

ret 1


#sub DlgProc2
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	 EnableWindow GetParent(hDlg) 1
	PostMessage hDlg WM_APP 0 0
	case WM_APP
	sub.Dialog3 hDlg
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1


#sub Dialog3
function# [hwndOwner]

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 148 60 "Dialog3"
 1 Button 0x54030001 0x4 24 16 48 14 "OK"
 2 Button 0x54030000 0x4 76 16 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc3 0 hwndOwner)) ret

ret 1


#sub DlgProc3
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	EnableWindow GetParent(hDlg) 1
	 PostMessage hDlg WM_APP 0 0
	 case WM_APP
	 sub.Dialog4 hDlg
	
	case WM_APP
	out
	int w1 w2
	w1=GetAncestor(hDlg 3)
	w2=GetParent(hDlg)
	outw w1; outw w2; outw hDlg
	out ""
	outw GetLastActivePopup(w1)
	outw GetLastActivePopup(w2)
	outw GetLastActivePopup(hDlg)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1


#sub Dialog4
function# [hwndOwner]

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 96 14 "Dialog4"
 1 Button 0x54030001 0x4 0 0 48 14 "OK"
 2 Button 0x54030000 0x4 48 0 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc4 0 hwndOwner)) ret

ret 1


#sub DlgProc4
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	EnableWindow GetParent(hDlg) 1
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
