 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "40 36 12 3 4 13 6 7 10 5 1110 1111 1201 1342 1514 1626 1634 1701 1726 1729 1801 1826 1829 1902 1921 1913 1920 2002 2032"
__strt e40var e36 e12dat e3 cb4Wha c13Wit c6Mul cb7DV c10No lb5How c1110Blo c1111Slo c1201Cle e1342Var e1514Tit e1626 cb1634Con e1701Var e1726 si1729 e1801Var e1826 si1829 c1902No e1921Fil c1913App c1920Add e2002Fil c2032Add

if(!getopt(nargs)) rget wParam "Text def" "\Tools" 0 1
TO_FavSel wParam lb5How "Paste (use clipboard)[]Send keys[]Send to QM output[]Store into str variable[]Store into clipboard[]Text box[]Set text of control or window[]Set text of accessible object[]Set text of html element[]Write to file[]Log to file[]On-screen display"
TO_FavSel lParam cb4Wha "Text[]Variable or other expression[]Text with date/time[]Macro[]File[]Clipboard[]Selected text (copy)"
cb1634Con="&Set text[]Paste (must be Edit control)"
si1729="&$qm$\target.ico"
si1829=si1729
cb7DV="&Current date/time[]DateTime variable"

if(!ShowDialog("" &TO_Text &controls _hwndqm)) ret

 0 text, 1 var, 2 date, 3 macro, 4 file, 5 clipb, 6 selection
int what=val(cb4Wha)
 0 paste, 1 key, 2 out, 3 var, 4 clipb, 5 textb, 6 control, 7 acc, 8 htm, 9 file, 10 log, 11 osd
int how=val(lb5How)
int i j isF isML
str s sML vDT
__strt vd vdDT

if(what=2 and val(cb7DV)=1) vdDT.VD("-r DateTime dt[]" vDT); vdDT.formata("%s.FromComputerTime(); %s.AddParts(-1) ;;example - current time minus 1 day[]" vDT vDT); vDT-" "; vDT+")"; else vDT=")"

sel what
	case [0,2]
	isF=(!what and c13Wit=1)
	str& sr=iif(what e12dat e3)
	if c6Mul=1 or sr.len>1000 ;;multiline. Set sML="_s=[]text[]", set what/e40var as variable, and later prepend sML to s.
		isML=1+(what/2)
		if(!sr.len) sr="line1[]line2[]line3[]"
		sr.replacerx("^" " " 8); sr.replacerx("^ $" ";" 8)
		sML=F"_s=[]{`F[]`+(!isF*3)}{sr}[]"
		s="_s"; e40var=s
		if(what) s+".timeformat(_s"; s+vDT
		what=1; goto g1

sel what
	case 0 s=e3.SF(isF)
	case 2 s=e12dat.SE; if(!s.len and vDT.len>1) s="''''"
	case [3,4] s=e36.S("???")

sel what
	case 1
		if(!e40var.len) s="???"
		else sel(how) case [1,5,7,8,10,11] s=e40var.N; case else s=e40var
		sel(how) case [1,5,11] if(!e40var.VarExists("str") and !e40var.VarExists("lpstr")) s=F"_s.from({s})"
	case 2 s-"_s.timeformat("; s+vDT
	case 3 s-"_s.getmacro("; s+")"
	case 4 s-"_s.getfile("; s+")"
	case 5 s="_s.getclip()"
	case 6 s="_s.getsel()"
 g1
sel how
	case [3,4,6,9]
	if(!s.beg("_s.")) s-"_s="

sel how
	case 0 if(what=5) s="key Cv"; else if(what or isF or !sub.IsMenuType) s-"paste "
	
	case 1
	if(what) s-"("; s+")"
	s-"key "
	if(c1110Blo=1) s=F"BlockInput 1[]{s}[]BlockInput 0"
	if(c1111Slo=1) s=F"opt slowkeys 1; spe 100[]{s}[]opt slowkeys 0"
	
	case 2
	s-"out "
	if(c1201Cle=1) if(s="out ''''") s="out"; else s-"out[]"
	
	case 3
	if(e1342Var.len) vd.VD("-r str _s" e1342Var); if(isML=1) sML.replace(vd 0 2); s=""; else s.replace(vd 0 2)
	else if(isML=1) s=""
	if(!s.len) sML.rtrim("[]")
	
	case 4 s+"; _s.setclip()"
	
	case 5 s=F"ShowText({e1514Tit.S} {s})"
	
	case 6
	e1626.N("hwnd")
	if(val(cb1634Con)) s+F"; _s.setsel(0 {e1626})"
	else s+F"; _s.setwintext({e1626})"
	
	case 7
	if(e1726.len) vd.ReplaceVar(e1726 e1701Var); e1726.s+"[]"; else e1701Var.VN("a")
	s=F"{e1726}{e1701Var}.SetValue({s})"
	
	case 8
	if(e1826.len) vd.ReplaceVar(e1826 e1801Var); e1826.s+"[]"; else e1801Var.VN("e")
	s=F"{e1826}{e1801Var}.SetText({s})"
	
	case 9
	if(!e1921Fil.len) e1921Fil="???"
	s+F"; _s.setfile({e1921Fil.S}"
	if(c1913App=1) s+" -1"; if(c1920Add=1) s+" -1 1"
	s+")"
	
	case 10
	s=F"LogFile({s} {c2032Add} {e2002Fil.S})"
	sub_to.Trim s " 0 ''''"
	
	case 11
	s=_s.format("OnScreenDisplay %s" s)

if(what=1) sel(how) case [4,6,9] if(e40var.VarExists("str")) s.replacerx(F"_s={e40var}; _s" e40var)

s-sML
s-vdDT

 sub_to.TestDialog s how
InsertStatement s

 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 391 199 "Text"
 35 Static 0x54020000 0x4 4 56 162 10 "File"
 37 Static 0x54020000 0x4 4 56 304 10 "Name of QM item (macro, menu, etc)"
 38 Static 0x54020000 0x4 4 56 158 10 "String or numeric variable or other expression"
 40 Edit 0x54030080 0x204 4 70 108 14 "var"
 36 Edit 0x54030080 0x204 4 70 384 14 "" "Tip: you can drag and drop."
 12 Edit 0x54231044 0x200 4 54 384 30 "dat"
 3 Edit 0x54231044 0x204 4 4 384 80 ""
 4 ComboBox 0x54230243 0x4 4 88 108 213 "What"
 13 Button 0x54012003 0x0 126 88 66 14 "With variables" "F string"
 14 Button 0x5C032000 0x0 194 88 42 14 "Variable..."
 16 Button 0x5C032000 0x0 236 88 18 14 "{"
 15 Button 0x54032000 0x4 254 88 18 14 "tab"
 6 Button 0x54012003 0x0 288 90 48 12 "Multiline" "Add text in multiline format, like[]str s=[] line1[] line2[] line3[][]If unchecked, adds text as quoted string with escape sequences, like[]str s=''line1[91]]line2[91]]line3''"
 7 ComboBox 0x54230243 0x0 126 88 96 213 "DV"
 8 Button 0x54032000 0x4 240 88 16 14 "?"
 18 Button 0x54032000 0x4 126 88 50 14 "Browse..."
 19 Button 0x54032000 0x4 178 88 16 14 "SF" "Special folders"
 10 Button 0x54012003 0x0 200 90 42 12 "No SF" "Let the button give me normal path, not special folder name"
 44 Button 0x54032000 0x4 346 88 42 14 "Paste"
 5 ListBox 0x54230101 0x204 4 112 108 54 "How"
 1110 Button 0x54012003 0x4 126 114 64 12 "Block input" "Adds:[]BlockInput 1"
 1111 Button 0x54012003 0x4 126 128 64 12 "Slow" "Adds:[]opt slowkeys 1"
 1201 Button 0x54012003 0x0 126 114 78 12 "Clear QM output"
 1341 Static 0x54020000 0x4 126 116 34 13 "Variable"
 1342 Edit 0x54030080 0x204 162 114 42 14 "Var" "Name of an existing or new str variable.[]Will be created if need.[]Default _s. It is a local str variable that can be used without declaring."
 1344 QM_DlgInfo 0x54000000 0x20000 126 146 262 20 "<>Variable help: <tip E_VARIABLES>quick help</tip>, <help #IDH_VARIABLES>variables</help>, <help #IDP_VARIABLES>declaration</help>, <help #IDP_SCOPE>scope and storage</help>"
 1522 Static 0x54020000 0x4 126 116 20 13 "Title"
 1514 Edit 0x54030080 0x204 148 114 240 14 "Title"
 1501 QM_DlgInfo 0x54000000 0x20000 126 140 262 26 "<>Shows a dialog with text that can be selected and copied.[]To show standard message box, instead use dialog <macro ''TO_Mes''>Message box</macro>."
 1627 Static 0x54020000 0x4 126 114 30 12 "Handle"
 1626 Edit 0x54030080 0x204 158 112 36 14 "" "Window/control handle variable.[]If you don't have it, use the 'Find...' button to create code."
 1625 Button 0x54032000 0x4 196 112 48 14 "Find..."
 1634 ComboBox 0x54230243 0x4 126 152 118 213 "Control" "'Set text' uses function str.setwintext. It replaces window or control text.[]'Paste' uses function str.setsel. It uses clipboard and WM_PASTE message. Most text editing controls should support it."
 1730 Static 0x54020000 0x4 126 118 48 12 "Acc variable"
 1701 Edit 0x54030080 0x200 176 116 36 14 "Var" "Accessible object variable. Optional.[]If you don't have it, use the Drag tool to create code."
 1726 Edit 0x54000884 0x4 126 144 262 22 ""
 1729 Static 0x54020003 0x4 220 114 16 16 "" ".1 Drag and drop to capture object"
 1831 Static 0x54020000 0x4 126 118 48 12 "Htm variable"
 1801 Edit 0x54030080 0x200 176 116 36 14 "Var" "HTML element variable. Optional.[]If you don't have it, use the Drag tool to create code."
 1826 Edit 0x54000884 0x4 126 144 262 22 ""
 1829 Static 0x54020003 0x4 220 114 16 16 "" ".1 Drag and drop to capture object"
 1901 Static 0x54000000 0x0 126 114 30 12 "File"
 1902 Button 0x54012003 0x0 244 112 40 12 "No SF" "Let the button give me normal path, not special folder name"
 1921 Edit 0x54030080 0x204 126 128 262 14 "Fil"
 1923 Button 0x54032000 0x4 170 112 50 14 "Browse..."
 1924 Button 0x54032000 0x4 222 112 16 14 "SF" "Special folders"
 1913 Button 0x54012003 0x4 126 146 44 13 "Append"
 1920 Button 0x5C012003 0x4 172 146 60 13 "Add newline"
 2001 Static 0x54000000 0x0 126 114 128 12 "File (optional, click ? for more info)"
 2002 Edit 0x54030080 0x200 126 128 262 14 "Fil"
 2032 Button 0x54012003 0x4 126 146 64 12 "Add date/time"
 2101 QM_DlgInfo 0x54000000 0x20000 126 114 262 52 "Shows temporary transparent text in screen center.[][]You can edit the code that this dialog creates. Set timeout, position, font, etc. Click ? to read more."
 1 Button 0x54030001 0x4 4 180 48 14 "OK"
 2 Button 0x54010000 0x4 54 180 50 14 "Cancel"
 17 Button 0x54032000 0x4 106 180 16 14 "?"
 46 Button 0x54032000 0x4 320 180 68 14 "Set default action"
 11 QM_DlgInfo 0x54000000 0x20000 4 2 384 48 ""
 45 Static 0x54000010 0x20004 4 106 384 1 ""
 28 Static 0x54000010 0x20004 0 172 476 2 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040108 "*" "" "" ""

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "5[]$qm$\text.ico" "" 3)) ret wParam
sel message
	case WM_INITDIALOG
	sub_to.SetUserFont hDlg "3"
	sub.Select hDlg
	sub.TimeSetText hDlg
	
	case WM_COMMAND goto messages2
	
	case WM_LBUTTONDOWN
	sel GetWinId(child(mouse))
		case 1729 EA_Main hDlg 1726 1
		case 1829 EH_Main hDlg 1826 1
ret
 messages2
sel wParam
	case 44 ;;Paste
		int h=id(3 hDlg); if(hid(h)) h=id(36 hDlg); if(hid(h)) h=id(40 hDlg)
		SetFocus h; SendMessage h WM_PASTE 0 0
	case 46 ;;Default action
		i=TO_Selected(hDlg 5)
		rset i "Text def" "\Tools"
		LB_GetItemText(id(5 hDlg) i s)
		sub_sys.MsgBox hDlg F"Action '{s}' will be selected whenever you open this dialog." "" "i"
	case 13 TO_Enable hDlg "14 16" but(lParam)
	case 14 sub_to.FstringVar hDlg 3
	case 15 EditReplaceSel hDlg 3 "[9]" 2
	case 16 EditReplaceSel hDlg 3 "{{" 2
	case 17
		_s=iif(TO_Selected(hDlg 1634) "CLIP" "WINDOW")
		QmHelp F"IDP_PASTE[]IDP_KEY[]IDP_OUT[]IDH_VARIABLES[]IDP_S_CLIP[]ShowText[]IDP_S_{_s}[]IDP_ACCESSIBLE[]IDP_HTM[]IDP_S_FILE[]LogFile[]OnScreenDisplay" TO_Selected(hDlg 5)
	case 18 sub_to.FileDialog hDlg 36 "outpdir" "" "Text files[]*.txt[]All Files[]*.*[]" "txt"
	case 19 sub_to.File_SF hDlg 36 "\"
	case 8 QmHelp "IDP_S_TIMEFORMAT"
	case 1625 TO_FindWindow 0 hDlg 1626
	case 1913 TO_Enable hDlg "1920" but(lParam)
	case 1923 sub_to.FileDialog hDlg 1921 "outpdir" "" "Text files[]*.txt[]All Files[]*.*[]" "txt"
	case 1924 sub_to.File_SF hDlg 1921 "\"
	
	case [CBN_SELENDOK<<16|4,LBN_SELCHANGE<<16|5] sub.Select hDlg
	case EN_CHANGE<<16|12
	_s.getwintext(lParam); if(findcr(_s '{')>findcr(_s '}')) _s+"}"
	OutStatusBar s.timeformat(_s)
ret 1

#opt nowarnings 1


#sub Select
function hDlg
int i=TO_Selected(hDlg 4)
int j=TO_Selected(hDlg 5)
TO_Show hDlg "3 6-8 10-16 18 19 35-38 40 44" 0
sel i
	case 0 TO_Show hDlg "3 6 13-16 44" 1
	case 1 TO_Show hDlg "40 38 44" 1
	case 2 TO_Show hDlg "6-8 11 12 44" 1
	case 3 TO_Show hDlg "36 37 44" 1
	case 4 TO_Show hDlg "10 18 19 35 36 44" 1
DT_Page hDlg j


#sub TimeSetText
function hDlg
lpstr stf=
 <>`{{}D}  - short date:  {D} 	`{{}DD} - long date:  {DD}
 `{{}T}  - time without seconds:  {T} 	`{{}TT} - time with seconds:  {TT}
 `{{}   - begin custom date/time format 	`}   - end custom date/time format
 `d    - day, 1-2 digits:  {d} 	`dd   - day, 2 digits:  {dd}
 `ddd  - day of week, short name:     {ddd} 	`dddd - day of week, full name:  {dddd}
 `M    - month, 1-2 digits:  {M} 	`MM   - month, 2 digits:  {MM}
 `MMM  - month, short name:  {MMM} 	`MMMM - month, full name:  {MMMM}
 `yy   - year, 2 digits:  {yy} 	`yyyy - year, 4-5 digits:  {yyyy}
 `h    - time, 12 hours, 1-2 digits:  {h} 	`hh   - time, 12 hours, 2 digits:  {hh}
 `H    - time, 24 hours, 1-2 digits:  {H} 	`HH   - time, 24 hours, 2 digits:  {HH}
 `m    - minutes, 1-2 digits:  {m} 	`mm   - minutes, 2 digits:  {mm}
 `s    - seconds, 1-2 digits:  {s} 	`ss   - seconds, 2 digits:  {ss}
 `t    - A/P or nothing:  {t} 	`tt   - AM/PM or nothing:  {tt}
 `gg   - period/era or nothing:  {gg} 	`y    - year, 1-2 digits:  {y}
 `{{}{{}}  - literal {{}
 
 Examples: Now is [{{}DD} {{}TT}], now is [{{}yyyy-MM-dd} {{}HH:mm:ss}].
 If empty, uses [{{}D} {{}T}]:  {D} {T}

_s.timeformat(stf)

str link=F"<fa ''{&sub.TimeLinkClick} /$1''>$1</fa>"
_s.replacerx("`(\S+)" link)
_s.replacerx("\[(.+?)\]" link)
_s.replacerx(":  ([^[][9]]+)" ":  <c 0x8000>$1</c>")

int h=id(11 hDlg)
SendMessage h SCI.SCI_SETTABWIDTH 90 0
_s.setwintext(h)
int- t_hDlg=hDlg


#sub TimeLinkClick
function[c] $s
int- t_hDlg

_s.getwintext(id(12 t_hDlg))
int i(findcr(_s '{')) j(findcr(_s '}'))
if(s[0]='{') if(i>j) s=_s.from("}" s)
else if(i<=j) s=_s.from("{" s)

EditReplaceSel t_hDlg 12 s 2


#sub IsMenuType
function!
QMITEM q; qmitem "" 0 q
sel(q.itype) case [2,3,4] ret 1
