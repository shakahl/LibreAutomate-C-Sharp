 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4 6 8 10 15 14"
__strt e4Var e6Des e8Tit e10Def cb15Tex cb14Res

cb15Tex="&Simple[]Multiline[]Password"
cb14Res="&[]Skip code (if)[]Exit[]End macro"

if(!ShowDialog("" &TO_Input &controls _hwndqm)) ret

int isDefVar
str s stt
__strt vd

if(!e4Var.VarExists) vd.VD("str s ;;string variable. If need numeric, replace str with int or double.[]" e4Var)

e10Def.S("" isDefVar)
if(cb15Tex.SelS(stt "<> [91]] *")) if(isDefVar) e10Def=F"F''{stt}{{{e10Def}}''"; else e10Def=F"''{stt}{e10Def+1}"

s=F"inp({e4Var} {e6Des.S} {e8Tit.S} {e10Def})"
sub_to.Trim s " '''' '''' ''''"

sel val(cb14Res)
	case 0 s-cb14Res.VD("int ok=")
	case 1 s=F"if {s}[][9]"
	case 2 s=F"if(!{s}) ret"
	case 3 s.replace("- " 3 1); s.fix(s.len-1)

s-vd

 sub_to.TestDialog s
InsertStatement s

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 273 132 "Input box"
 3 Static 0x54020000 0x4 4 6 42 12 "Variable"
 4 Edit 0x54030080 0x204 48 4 48 14 "Var" "String or numeric variable that receives user input text or number.[]Will be created if need."
 5 Static 0x54020000 0x4 4 24 42 12 "Description"
 6 Edit 0x54231044 0x204 48 22 220 20 "Des"
 7 Static 0x54020000 0x4 4 46 42 12 "Title"
 8 Edit 0x54030080 0x204 48 46 220 14 "Tit"
 9 Static 0x54020000 0x4 4 66 42 12 "Default"
 10 Edit 0x54231044 0x204 48 64 220 14 "Def" "Text or number that the text field initially will contain"
 11 Static 0x54000000 0x0 4 86 42 12 "Text field is"
 15 ComboBox 0x54230243 0x0 48 84 76 213 "Tex"
 16 Static 0x54000000 0x0 152 86 38 12 "On Cancel"
 14 ComboBox 0x54230243 0x0 192 84 76 213 "Res"
 1 Button 0x54030001 0x4 4 114 48 14 "OK"
 2 Button 0x54030000 0x4 54 114 48 14 "Cancel"
 13 Button 0x54032000 0x4 104 114 18 14 "?"
 12 Static 0x54000010 0x20004 0 106 300 1 ""
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
	case 13 QmHelp "IDP_INP"
ret 1
