 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4 6 7 16 8 18 10 14"
__strt e4Use e6Pas c7Enc c16Pre c8Use c18In e10Win e14Wai
c7Enc=1; c16Pre=1; c8Use=1
if(!ShowDialog("" &TO_AutoPassword &controls _hwndqm)) ret

if(c7Enc=1) e6Pas.encrypt(16 e6Pas "AutoPassword")
int f=(val(c18In)*4)+(val(c8Use)*2)+val(c16Pre)
str s=F"AutoPassword {e4Use.S} {e6Pas.S} 0x{f} {e10Win.N} {e14Wai.N}"
sub_to.Trim s " 0x0 0 0"

 sub_to.TestDialog s
InsertStatement s

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 225 164 "Enter password"
 3 Static 0x54000000 0x0 4 34 56 13 "User (optional)"
 4 Edit 0x54030080 0x200 62 34 160 15 "Use"
 5 Static 0x54000000 0x0 4 50 56 13 "Password"
 6 Edit 0x54030080 0x200 62 50 118 15 "Pas"
 7 Button 0x54012003 0x0 182 50 40 13 "Encrypt"
 16 Button 0x54012003 0x0 4 68 132 13 "Press Enter key" "After filling user/password fields press Enter"
 8 Button 0x54012003 0x0 4 82 132 13 "Use keyboard to enter password" "Less secure but works everywhere.[]Must be checked if used with a web browsers other than Internet Explorer."
 18 Button 0x54012003 0x0 144 68 78 13 "In web page" "Check this if the password field is in web page in IE, Firefox or Chrome. Makes faster."
 9 Static 0x54000000 0x0 4 101 56 13 "Window handle"
 10 Edit 0x54030080 0x200 62 101 46 14 "Win" "Window/control handle variable.[]If you don't have it, use the 'Find...' button to create code."
 15 Button 0x54032000 0x0 110 100 48 14 "Find..."
 13 Static 0x54000000 0x0 4 117 56 13 "Wait max., s"
 14 Edit 0x54030080 0x200 62 117 46 14 "Wai" "If specified, waits for the password field.[]Useful if it is in a web page that may be still not fully loaded."
 12 QM_DlgInfo 0x54000000 0x20000 0 0 226 30 "Enters password into the password field of the active window. You can optionally use window handle to enter the password into that window or control."
 1 Button 0x54030001 0x4 4 146 48 14 "OK"
 2 Button 0x54030000 0x4 54 146 48 14 "Cancel"
 11 Button 0x54032000 0x0 104 146 18 14 "?"
 17 Static 0x54000010 0x20000 0 136 262 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040107 "*" "" "" ""

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "0[]$qm$\password.ico" "" 1)) ret wParam
sel message
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 15 TO_FindWindow 0 hDlg 10
	case 11 QmHelp "AutoPassword"
ret 1
