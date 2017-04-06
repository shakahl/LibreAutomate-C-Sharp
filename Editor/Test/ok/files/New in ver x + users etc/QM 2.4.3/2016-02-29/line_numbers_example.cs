\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 176 78 "line numbers+scroll example"
 3 Edit 0x54031844 0x200 27 4 18 37 ""
 4 Edit 0x54231044 0x200 48 4 122 37 ""
 1 Button 0x54030001 0x4 68 56 48 14 "OK"
 2 Button 0x54030000 0x4 120 56 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

str controls = "3 4"
str e3 e4
e4="line_1[]line_2[]line_3[]line_4[]five[]six[]seven"
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

 OutWinMsg message wParam lParam

sel message
	case WM_INITDIALOG
		sub.SetLineNumbers(hDlg) ;; Must pass 'hDlg', even when using 'v' parameter in subfunction 'hDlg' does not get recognized.
		
		SetTimer hDlg 1 50 &sub.Timer
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case EN_CHANGE<<16|4
		sub.SetLineNumbers(hDlg)
ret 1
 
#sub SetLineNumbers
function int'h_dlg
	int i
	str get_lns set_lns
	get_lns.getwintext(id(4 h_dlg))
	get_lns.trim
	ARRAY(str) ln_arr.create(numlines(get_lns))
	for i 0 ln_arr.len
		ln_arr[i]=i+1
	_s=ln_arr
	_s.setwintext(id(3 h_dlg))


#sub Timer
function hDlg _m _e _t
int pos=GetScrollPos(id(4 hDlg) SB_VERT)
SendDlgItemMessage hDlg 3 WM_VSCROLL MakeInt(SB_THUMBPOSITION pos) 0
