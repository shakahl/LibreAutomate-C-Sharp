 \Dialog_Editor
function [hwndOwner]

 Shows 'QM Email Settings' dialog.


int h=win("QM Email Settings" "#32770" "qm")
if(h) act h; ret

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x40100 0 0 242 114 "QM Email Settings"
 5 Static 0x54020000 0x4 10 7 48 10 "Email accounts"
 3 ListBox 0x54230101 0x204 8 20 172 61 "Acc"
 8 Button 0x54012003 0x4 192 68 42 13 "Default"
 6 Button 0x54032000 0x4 192 20 44 14 "Properties"
 7 Button 0x54032000 0x4 192 36 44 14 "New"
 9 Button 0x54032000 0x4 192 52 44 14 "Delete..."
 4 Edit 0x54000844 0x20000 8 92 172 18 "Info: These email account settings are shared with Outlook Express (Windows XP)."
 2 Button 0x54030000 0x0 192 94 44 14 "Close"
 10 Static 0x54000010 0x20000 8 86 228 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040303 "" "" "" ""

ShowDialog(dd &sub.Dlg 0 iif(hwndOwner hwndOwner _hwndqm))


#sub Dlg
function# hDlg message wParam lParam

ARRAY(__REGEMAILACCOUNT)-- a
str s; int i
sel message
	case WM_INITDIALOG
	sub_sys.PortableWarning
	 g0
	SendDlgItemMessage hDlg 3 LB_RESETCONTENT 0 0
	MailGetAccounts a
	if(a.len)
		for(i 0 a.len) LB_Add(id(3 hDlg) a[i].name)
		SendDlgItemMessage(hDlg 3 LB_SETCURSEL 0 0)
		i=0; goto g1
	case WM_COMMAND goto messages2
ret
 messages2
i=TO_Selected(hDlg 3)
sel wParam
	case [6,LBN_DBLCLK<<16|3] if(i>=0) goto g2 ;;dblclick or Properties
	case 7 i=-1; goto g2 ;;New
	case 9 ;;Delete
	if(i<0) ret
	if(sub_sys.MsgBox(hDlg "Are you sure?[][]Note: These email account settings are shared with Outlook Express (Windows XP)." "" "YN!")!='Y') ret
	rset "" a[i].keyname RK_IAMA 0 -2; goto g0
	case 8 ;;Default
	if(i>=0) rset a[i].keyname "QM default mail account" RK_IAM
	case LBN_SELCHANGE<<16|3
	if(i<0) ret
	 g1
	if(rget(s "QM default mail account" RK_IAM)<2) rget(s "Default Mail Account" RK_IAM)
	TO_Check hDlg "8" s~a[i].keyname
ret 1

 g2
__REGEMAILACCOUNT-* ___t_em_acc; ___t_em_acc=iif(i>=0 &a[i] 0)
int- ___t_em_accn; ___t_em_accn=a.len
MailSetupAccount 0 hDlg 0 0
goto g0
