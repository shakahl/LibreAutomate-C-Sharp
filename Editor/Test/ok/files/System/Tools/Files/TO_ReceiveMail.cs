 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "19 3 33 30 18 6 14 7 9"
__strt c19Del c3Dow c33Sho cb30acc c18Var e6fol c14No c7Del c9Get

cb30acc="&<Default>"

if(!ShowDialog("" &TO_ReceiveMail &controls _hwndqm)) ret

int i
str s sout sf
__strt vd varFiles varObjects

i=cb30acc.CbItem; if(i<1) cb30acc=""
sub_to.Mail_GetAccountVar s cb30acc c18Var 1
TO_FlagsFromCheckboxes sf c3Dow 1 c19Del 2 c33Sho 0x100 c7Del 0x1000000

if(e6fol.len) s+vd.VD("ARRAY(str) af[]" varFiles); sout.addline(F"for _i 0 {varFiles}.len[][9]out {varFiles}[_i]"); else varFiles=0
if(c9Get=1) s+vd.VD("ARRAY(MailBee.Message) am[]" varObjects); sout.addline(F"for _i 0 {varObjects}.len[][9]MailBee.Message& m={varObjects}[_i][][9]str subj from[][9]subj=m.Subject[][9]from=m.FromAddr[][9]out ''subject=%s, from=%s'' subj from"); else varObjects=0

s+F"{vd.VD(`int nMessages`)}=ReceiveMail({cb30acc} {sf} {e6fol.S} {varFiles} {varObjects})"
sub_to.Trim s " 0 0"

 sub_to.TestDialog s
InsertStatement s
if(sout.len) out "<><code> sample code, shows how to use results[]%s</code>" sout

 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 301 215 "Receive email messages (POP3)"
 19 Button 0x54012003 0x4 10 22 114 10 "Delete messages from server"
 3 Button 0x54012003 0x0 10 34 114 10 "Download only headers"
 33 Button 0x54012003 0x4 10 46 114 10 "Show progress dialog"
 29 Static 0x54020000 0x4 146 28 50 13 "Email account"
 30 ComboBox 0x54230243 0x4 200 26 92 213 "acc"
 18 Button 0x54012003 0x0 200 42 42 14 "Variable" "Insert account settings in the macro now instead of retrieving from registry at run time.[]Then the macro can be used on any computer. Don't need to create email account on other computers."
 31 Button 0x54032000 0x4 246 42 48 14 "Accounts..."
 23 QM_DlgInfo 0x54000000 0x20000 10 84 284 30 "Messages can be saved as eml files, or/and retrieved as message objects in memory (to get message parts, etc). To save files, please specify a folder. To get objects, check 'Get message objects'."
 4 Static 0x54000000 0x0 10 118 90 10 "Folder for message files"
 6 Edit 0x54030080 0x200 10 130 282 14 "fol"
 8 Button 0x54032000 0x0 10 146 46 14 "Browse..."
 5 Button 0x54032000 0x0 56 146 18 14 "SF" "Special folders"
 14 Button 0x54012003 0x0 88 148 36 13 "No SF" "Let the button give me normal path, not special folder name"
 7 Button 0x54012003 0x0 212 148 80 13 "Delete existing files"
 9 Button 0x54012003 0x0 10 166 94 12 "Get message objects"
 1 Button 0x54030000 0x4 4 196 48 14 "OK"
 2 Button 0x54030000 0x4 54 196 48 14 "Cancel"
 25 Button 0x54032000 0x4 104 196 18 14 "?"
 21 Static 0x54000010 0x20000 0 190 354 1 ""
 22 Button 0x54020007 0x0 4 8 294 54 "Downloading"
 20 Button 0x54020007 0x0 4 70 294 114 "Saving"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "[]$qm$\email_receive.ico" "6" 1)) ret wParam
sel message
	case WM_INITDIALOG
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 8 sub_to.FolderDialog hDlg 6 "" 4
	case 5 sub_to.File_SF hDlg 6
	case CBN_DROPDOWN<<16|30 sub_to.Mail_SetAccountsCb lParam
	case 31 MailSetup hDlg
	case 25 QmHelp "ReceiveMail"
ret 1

#opt nowarnings 1
