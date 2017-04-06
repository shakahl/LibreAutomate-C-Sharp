 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4 5 13 14"
str e3Nam cb4Typ cb5Sco c13Fun c14Ref

cb4Typ="&str  (string)[]int  (integer number)[]double  (floating-point number)[]ARRAY(str)  (array of strings)"
cb5Sco="&local[]global[]thread (shared)[]thread (private)"

if(!ShowDialog("TO_Variables" &TO_Variables &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 169 114 "Create variable"
 7 Static 0x54020000 0x4 4 4 26 13 "Name"
 3 Edit 0x54030080 0x204 32 4 112 14 "Nam"
 10 Button 0x54032000 0x4 148 4 18 14 "?"
 8 Static 0x54020000 0x4 4 24 26 13 "Type"
 4 ComboBox 0x54230242 0x4 32 24 112 176 "Typ"
 15 Button 0x54032000 0x4 148 24 18 14 "?"
 9 Static 0x54020000 0x4 4 42 26 13 "Scope"
 5 ComboBox 0x54230243 0x4 32 42 112 158 "Sco"
 12 Button 0x54032000 0x4 148 42 18 14 "?"
 13 Button 0x54012003 0x4 4 60 110 13 "Function's argument"
 16 Button 0x54032000 0x0 148 60 18 14 "?"
 14 Button 0x54012003 0x4 4 74 110 12 "Reference to other variable"
 17 Button 0x54032000 0x0 148 74 18 14 "?"
 1 Button 0x54030001 0x4 4 96 48 14 "OK"
 2 Button 0x54030000 0x4 54 96 48 14 "Cancel"
 11 Button 0x54032000 0x4 104 96 18 14 "?"
 6 Static 0x54000010 0x20004 4 90 162 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010400 "*" ""

ret
 messages
sel message
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 10 QmHelp "IDP_IDENTIFIERS"
	case 15 ShowText "Variable types" "To view/insert other variable types, use menu Edit -> Members -> Types, libraries." hDlg 1
	case 12 QmHelp "IDP_SCOPE"
	case 16 QmHelp "IDP_FUNCTION"
	case 17 QmHelp "IDH_POINTERS"
	case 11 QmHelp "IDH_VARIABLES"
	case IDOK ret TO_Ok(hDlg)
ret 1
