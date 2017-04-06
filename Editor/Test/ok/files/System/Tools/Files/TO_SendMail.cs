 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "8 10 11 16 7 17 23 22 12 14 33 19 30 24"
__strt e8to e10cc e11sub e16bcc e7tex e17fil c23Htm c22Fil e12att c14No c33Sho c19Pre cb30acc c24Var

cb30acc="&<Default>"

if(!ShowDialog("" &TO_SendMail &controls _hwndqm)) ret

int iacc
str s sf

if(c22Fil=1) e7tex=e17fil
iacc=cb30acc.CbItem; if(iacc<1) cb30acc=""
sub_to.Mail_GetAccountVar s cb30acc c24Var 0
TO_FlagsFromCheckboxes sf c23Htm 1 c22Fil 2 c33Sho 0x100 c19Pre 0x70000

s+F"SendMail {e8to.S} {e11sub.S} {e7tex.S} {sf} {e12att.S} {e10cc.S} {e16bcc.S} '''' '''' {cb30acc}"
sub_to.Trim s " 0 '''' '''' '''' '''' '''' ''''"

 sub_to.TestDialog s
InsertStatement s
ret

 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 345 186 "Send email message (use SMTP)"
 3 Static 0x54020000 0x4 4 6 12 10 "To"
 8 Edit 0x54030080 0x204 42 4 130 14 "to"
 4 Static 0x54020000 0x4 192 6 18 12 "Cc"
 10 Edit 0x54030080 0x204 212 4 130 14 "cc"
 6 Static 0x54020000 0x4 4 24 32 10 "Subject"
 11 Edit 0x54030080 0x204 42 22 130 14 "sub"
 9 Static 0x54020000 0x4 192 24 18 12 "Bcc"
 16 Edit 0x54030080 0x204 212 22 130 14 "bcc"
 20 Static 0x54020000 0x4 4 40 32 10 "Text"
 7 Edit 0x54231044 0x204 42 40 300 52 "tex" "Text or (variable)"
 17 Edit 0x44030080 0x204 42 78 300 14 "fil"
 23 Button 0x54012003 0x4 4 54 34 10 "Html"
 22 Button 0x54012003 0x4 4 66 34 10 "File"
 34 Button 0x44032000 0x4 24 78 16 13 "..."
 12 Edit 0x542310C4 0x204 42 98 300 28 "att"
 15 Button 0x54032000 0x4 4 98 36 14 "Attach..."
 14 Button 0x54012003 0x0 4 114 36 12 "No SF" "Let the button give me normal path, not special folder name"
 33 Button 0x54012003 0x4 4 138 46 18 "Show progress"
 19 Button 0x54012003 0x4 54 138 56 18 "Preview     (don't send)"
 29 Static 0x54020000 0x4 130 142 32 10 "Account"
 30 ComboBox 0x54230243 0x4 162 140 86 213 "acc"
 24 Button 0x54012003 0x0 250 140 42 14 "Variable" "Insert account settings in the macro now instead of retrieving from registry at run time.[]Then the macro can be used on any computer. Don't need to create email account on other computers."
 31 Button 0x54032000 0x4 294 140 48 14 "Accounts..."
 1 Button 0x54030000 0x4 4 168 48 14 "OK"
 2 Button 0x54030000 0x4 54 168 48 14 "Cancel"
 25 Button 0x54032000 0x4 104 168 18 14 "?"
 5 Button 0x54032000 0x0 124 168 34 14 "? More"
 32 Static 0x54000010 0x20004 0 160 362 1 ""
 18 Static 0x54000010 0x20004 4 130 338 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "[]$qm$\email.ico" "12 17" 1)) ret wParam
sel message
	case WM_INITDIALOG
	sub_to.SetUserFont hDlg "7"
	SendDlgItemMessage(hDlg 7 EM_LIMITTEXT 4000 0)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 22 TO_Show hDlg "-7 17 34" but(lParam)
	
	case 34
	s="Email messages (.eml), text files, html files[]*.txt;*.htm*;*.eml[]All files[]*.*"
	if(sub_to.FileDialog(hDlg 17 "mailtextdir" "" s "" sf))
		TO_Check hDlg "23" (sf.endi(".htm") or sf.endi(".html"))
		
	case 15
	if(sub_to.FileDialog(hDlg 0 "mailattachdir" "" "" "" sf)) TO_SetText sf hDlg 12 1
		
	case CBN_DROPDOWN<<16|30 sub_to.Mail_SetAccountsCb lParam
	case 31 MailSetup hDlg
	case 25 QmHelp "SendMail"
	case 5
	s=
 To create new message in default email program and don't send automatically, use <help>run</help> with "mailto:" protocol. Can be specified address, subject, CC, BCC and body, but cannot attach files. Example:
;
 <code>run "mailto:xxx@yyy.com?subject=Example&body=Hi"</code>
	QmHelp s 0 6
ret 1

#opt nowarnings 1
