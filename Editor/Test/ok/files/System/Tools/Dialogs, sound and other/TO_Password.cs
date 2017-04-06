 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4 10 9 6 8 17"
__strt e4Pas c10Enc c9Cas e6Des e8Tit cb17Err

cb17Err="&End macro[]Execute following tab-indented code"
c10Enc=1
c9Cas=1
if(!ShowDialog("" &TO_Password &controls _hwndqm)) ret

str s; int f

if(c10Enc=1) e4Pas.encrypt(16 e4Pas "inpp")
if(c9Cas=1) f|1

s=F"{e4Pas.S} {e6Des.S} {e8Tit.S} {f}"
sub_to.Trim s " '''' '''' 0"

sel val(cb17Err)
	case 0 s-"inpp "; s+" ;;show password input box. If Cancel or incorrect password, end macro."
	case 1 s-"if(!inpp("; s+")) ;;show password input box. If Cancel or incorrect password...[][9]"

 sub_to.TestDialog s
InsertStatement s

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 275 131 "Password input box"
 3 Static 0x54020000 0x4 4 22 40 13 "Password"
 4 Edit 0x54030080 0x204 46 20 106 14 "Pas"
 10 Button 0x54012003 0x0 154 20 46 12 "Encrypt"
 9 Button 0x54012003 0x0 204 20 70 12 "Case insensitive"
 5 Static 0x54020000 0x4 4 40 40 12 "Description"
 6 Edit 0x54231044 0x204 46 38 226 21 "Des"
 7 Static 0x54020000 0x4 4 64 40 12 "Title"
 8 Edit 0x54030080 0x204 46 62 226 14 "Tit"
 16 Static 0x54020000 0x4 4 88 94 12 "If password does not match"
 17 ComboBox 0x54230243 0x4 100 86 172 213 "Err"
 1 Button 0x54030001 0x4 4 112 48 14 "OK"
 2 Button 0x54030000 0x4 54 112 48 14 "Cancel"
 13 Button 0x54032000 0x4 104 112 18 14 "?"
 11 QM_DlgInfo 0x54000000 0x20000 0 0 276 14 "Asks for password to run the macro. The macro also should be encrypted."
 14 Static 0x54000010 0x20004 4 80 270 1 ""
 12 Static 0x54000010 0x20004 0 104 292 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "0[]$qm$\inp.ico" "" 1)) ret wParam
sel message
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 13 QmHelp "IDP_INPP"
ret 1
