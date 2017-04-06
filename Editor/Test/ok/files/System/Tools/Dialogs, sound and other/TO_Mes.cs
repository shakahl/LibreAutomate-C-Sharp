 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "36 3 4 19 14 12 10 13 16 23"
__strt e36 e3 cb4Wha c19Wit e14Tit cb12But cb10Def cb13Ico cb16Foc cb23Res

cb4Wha="&Text[]Variable or other expression"
cb12But="&OK[]OK Cancel[]Yes No[]Yes No Cancel[]Abort Retry Ignore[]Retry Cancel[]Cancel TryAgain Continue"
cb10Def="&1[]2[]3"
cb13Ico="&No icon[]Question (?)[]Warning (!)[]Error (x)[]Information (i)[]QM[]No icon, silent[]Question, silent[]Warning, silent[]Error, silent[]Information, silent[]QM, silent[]Shield, silent"
cb16Foc="&[]Yes[]No"
cb23Res="&[]Exit on Cancel or No[]End macro on Cancel, No or single OK[]Use sel"

if(!ShowDialog("" &TO_Mes &controls _hwndqm)) ret

int i R(val(cb23Res))
str s sb si p1(" ") p2

sel val(cb4Wha)
	case 0 e3.SF(c19Wit=1)
	case 1 e3=e36.N

cb12But.SelS(sb "<> OC YN YNC ARI RI CTE"); if(sb.len && !R) R=3
cb13Ico.SelS(si "<> ? ! x i q s ?s !s xs is qs v")

sel(R) case [1,3] p1="("; p2=")"; if(!sb.len) sb="OC"

s=F"mes{p1}{e3} {e14Tit.S} ''{sb}{cb10Def.SelC(` 23`)}{si}{cb16Foc.SelC(` an`)}''{p2}"
sub_to.Trim s " '''' ''''"

sel R
	case 1 s-"if("; s+F"!='{sb[0]%%c}') ret"
	case 2 s.insert("-" 3)
	case 3 s-"sel "; for(i 0 sb.len) s+F"[][9]case '{sb[i]%%c}'"

 sub_to.TestDialog s
InsertStatement s

 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 264 195 "Message box"
 38 Static 0x54020000 0x4 4 28 172 13 "String or numeric variable or other expression"
 36 Edit 0x54030080 0x204 4 41 256 14 ""
 3 Edit 0x54231044 0x204 4 4 256 50 ""
 4 ComboBox 0x54230243 0x4 4 60 108 213 "What"
 19 Button 0x54012003 0x0 116 60 62 14 "With variables" "F string"
 6 Button 0x5C032000 0x4 182 60 42 14 "Variable..."
 7 Button 0x5C032000 0x4 224 60 18 14 "{"
 15 Button 0x54032000 0x4 242 60 18 14 "tab"
 22 Static 0x54020000 0x4 4 87 30 12 "Title"
 14 Edit 0x54030080 0x204 36 86 224 14 "Title"
 20 Static 0x54020000 0x4 4 104 30 13 "Buttons"
 12 ComboBox 0x54230243 0x4 36 104 90 213 "Buttons"
 9 Static 0x54020000 0x4 144 104 60 12 "Default button"
 10 ComboBox 0x54230243 0x4 206 104 54 213 "Def"
 21 Static 0x54020000 0x4 4 120 30 16 "Icon && sound"
 13 ComboBox 0x54230243 0x4 36 120 90 213 "Icon"
 11 Static 0x54020000 0x4 144 120 60 12 "Activate window"
 16 ComboBox 0x54230243 0x4 206 120 54 213 "Foc"
 24 Static 0x54020000 0x4 4 148 62 13 "Result processing"
 23 ComboBox 0x54230243 0x4 68 148 192 213 "Result"
 1 Button 0x54030000 0x4 4 177 48 14 "OK"
 2 Button 0x54010000 0x4 56 177 50 14 "Cancel"
 17 Button 0x54032001 0x4 110 177 16 14 "?"
 8 Static 0x54000010 0x20004 4 140 258 2 ""
 5 Static 0x54000010 0x20004 4 79 258 1 ""
 18 Static 0x54000010 0x20004 0 170 304 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040101 "*" "" "" ""

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "0[]$qm$\mes.ico" "" 1)) ret wParam
sel message
	case WM_INITDIALOG
	sub_to.SetUserFont hDlg "3"
	SendDlgItemMessage(hDlg 3 EM_LIMITTEXT 4000 0)
	goto g11
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case CBN_SELENDOK<<16|4
	i=TO_Selected(lParam)
	 g11
	TO_Show hDlg "3 6 7 15 19 -36 -38" i=0
	
	case 19 TO_Enable hDlg "6 7" but(lParam)
	case 6 sub_to.FstringVar hDlg 3
	case 7 EditReplaceSel hDlg 3 "{{" 2
	case 15 EditReplaceSel hDlg 3 "[9]" 2
	case 17 QmHelp "IDP_MES"
ret 1

#opt nowarnings 1
