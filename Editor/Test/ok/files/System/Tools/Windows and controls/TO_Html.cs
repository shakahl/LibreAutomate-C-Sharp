 \Dialog_Editor
function [_1] [_2] [fav]

str dd=
 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 334 192 "Html element"
 6 Static 0x54020000 0x4 4 6 46 14 "Htm variable"
 7 Edit 0x54030080 0x204 52 4 28 14 "Var" "HTML element variable. Optional.[]If you don't have it, use the Drag tool to create code."
 4 Static 0x54020003 0x4 88 2 16 16 "Drag" ".1 Drag and drop to capture object"
 5 Edit 0x54000884 0x0 118 2 212 20 ""
 3 ListBox 0x54230101 0x204 4 30 104 132 "Act"
 1102 ListBox 0x54230101 0x200 118 30 96 92 "Scr"
 1001 ListBox 0x54230109 0x200 118 30 96 50 "xy" "Select one or several"
 1201 Static 0x44020000 0x4 118 32 30 10 "Text"
 1202 Edit 0x44231044 0x204 118 42 214 50 "Tex"
 1801 ListBox 0x54230101 0x200 118 30 96 26 "Htm"
 1301 Static 0x44020000 0x4 118 32 40 10 "Attribute"
 1302 ComboBox 0x44230241 0x4 118 42 96 120 "Att"
 1303 ListBox 0x54230101 0x200 228 42 102 36 "Fla"
 1304 Static 0x54000000 0x0 228 32 40 10 "Flags"
 1401 ComboBox 0x44230243 0x4 118 30 72 213 "Sel"
 1402 Edit 0x44030080 0x204 196 30 134 14 "Ind"
 1403 Button 0x54012003 0x0 118 54 72 12 "Add to selection"
 1602 ListBox 0x54230101 0x200 118 30 96 50 "Mou"
 1603 Static 0x44000000 0x4 118 92 42 13 "Offset X, Y"
 1604 Edit 0x44030080 0x204 162 90 24 14 "xo"
 1605 Edit 0x44030080 0x204 190 90 24 14 "yo"
 1701 Button 0x54012003 0x0 118 30 66 12 "Get item text"
 1501 ListBox 0x54230109 0x200 118 30 96 50 "" "Select one or several"
 2501 QM_DlgInfo 0x54000000 0x20000 118 30 212 132 ""
 1 Button 0x54030001 0x4 4 174 48 14 "OK"
 2 Button 0x54010000 0x4 54 174 50 14 "Cancel"
 10 Button 0x54032000 0x4 106 174 16 14 "?"
 11 Static 0x54000010 0x20004 0 166 340 1 ""
 8 Static 0x54000010 0x20004 4 24 328 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "-1 -1 1 0 -1 2 8 -1 3 4 7 -1 -1 6 5" ""

str controls = "7 4 5 3 1102 1001 1202 1801 1302 1303 1401 1402 1403 1602 1604 1605 1701 1501"
__strt e7Var si4Dra e5 lb3Act lb1102Scr lb1001xy e1202Tex lb1801Htm cb1302Att lb1303Fla cb1401Sel e1402Ind c1403Add lb1602Mou e1604xo e1605yo c1701Get lb1501

TO_FavSel fav lb3Act "Click[]Set focus[]Scroll[]Get location[]Get text[]Set text[]Get HTML[]Get tag[]Get attribute[]Select combobox item[]Get combobox selected item[]Is checked[]Click asynchronously[]Mouse actions[]Get page properties[]More functions"
si4Dra="&$qm$\target.ico"
lb1102Scr="&Scroll into view[]down[]pageDown[]up[]pageUp[]right[]pageRight[]left[]pageLeft"
lb1001xy="X[]Y[]Width[]Height"
lb1801Htm="&Outer[]Inner"
cb1302Att="&href[]src[]type[]name[]value[]id[]alt[]title[]onclick[]classid"
lb1303Fla="&Interpolated[]Case sensitive[]As in HTML"
cb1401Sel="&Text[]Index"
lb1602Mou="Mouse move[]&Left click[]Right click[]Middle click[]Double click"
lb1501="URL[]Title[]HTML[]Text"

if(!ShowDialog(dd &sub.Dlg &controls _hwndqm)) ret

str s
__strt vd
ARRAY(str) z
int A(val(lb3Act)) i
if(A=15) ret ;;just info

if(e5.len) vd.ReplaceVar(e5 e7Var); e5.s+"[]"; else e7Var.VN("e")
s.from(e7Var "." sub.HtmFunction(A))

 0 click, 1 focus, 2 scroll, 3 loc, 4 text, 5 set text, 6 html, 7 tag, 8 attr,
 9 select, 10 get sel, 11 checked, 12 click async, 13 mouse, 14 page prop

sel A
	case 2 ;;scroll
	i=lb1102Scr.CbItem
	if(i>0) s+F"(''{lb1102Scr}'')"
	
	case 3 ;;location
	lb1001xy.LbSelectedItemsToNames(z "x y cx cy" "1111")
	vd.VD("-D int 0 0 0 0[]" z[0] z[1] z[2] z[3])
	s=F"{vd}{s}({z[0]} {z[1]} {z[2]} {z[3]})"; sub_to.Trim s " 0 0 0"
	
	case 4 s-F"{vd.VD(`str text`)}=" ;;get text
	case 5 s+F"({e1202Tex.S})" ;;set text
	case 6 s-F"{vd.VD(`str html`)}="; if(val(lb1801Htm)) s+"(1)" ;;get HTML
	case 7 s-F"{vd.VD(`str tag`)}=" ;;get tag
	case 8 cb1302Att.CbItem; vd=cb1302Att; vd.VN("attr"); s=F"{vd.VD(`-sD str attr`)}={s}({cb1302Att.S} {val(lb1303Fla)})"; sub_to.Trim s " 0"
	
	case 9 ;;select cb
	if(val(cb1401Sel)) e1402Ind.N; else e1402Ind.S
	s+F"({e1402Ind} {c1403Add})"; sub_to.Trim s " 0"
	
	case 10 s-F"{vd.VD(`int selItem`)}="; if(c1701Get=1) s=F"{vd.VD(`-i str selItemText` _s)}[]{s}({_s})" ;;get cb item
	
	case 11 s-F"{vd.VD(`int checked`)}=" ;;is checked
	
	case 13 ;;mouse
	s+F"({val(lb1602Mou)} {e1604xo.N} {e1605yo.N})"
	sub_to.Trim s "0 0 0"
	
	case 14 ;;page prop
	lb1501.LbSelectedItemsToNames(z "url title html text" "1000")
	vd.VD("-D str 0 0 0 0[]" z[0] z[1] z[2] z[3])
	s=F"{vd}{s}(0 {z[0]} {z[1]} {z[2]} {z[3]})"; sub_to.Trim s " 0 0 0"

s-e5

 sub_to.TestDialog s A
InsertStatement s


#sub Dlg
function# hDlg message wParam lParam

if(sub_to.ToolDlgCommon(&hDlg "3[]$qm$\web.ico[]TO_Html" "" 1)) ret wParam
sel message
	case WM_INITDIALOG goto gAction
	case WM_COMMAND goto messages2
	
	case WM_LBUTTONDOWN
	if(GetDlgCtrlID(child(mouse))=4) if(EH_Main(hDlg 5 1)) goto gAction
ret
 messages2
sel wParam
	case 10 QmHelp "IDP_HTM"
	
	case LBN_SELCHANGE<<16|3
	 gAction
	int i=TO_Selected(hDlg 3)
	DT_Page hDlg i "-1 -1 1 0 -1 2 8 -1 3 4 7 -1 -1 6 5"
	if(i=15) goto gMore
	QmHelp sub.HtmFunction(i 1)
ret 1

 gMore
_s=
 <>This dialog does not create code for:
 <help>Htm.AttributesAll</help>
 <help>Htm.FromAcc</help>
 <help>Htm.FromXY</help>
 <help>Htm.Hwnd</help>
 <help>Htm.NotFound</help>
;
 See also: <help>Htm help</help>
_s.setwintext(id(2501 hDlg))


#sub HtmFunction
function~ i [flags] ;;flags: 1 with "Htm."

_s.gett("Click SetFocus Scroll Location Text SetText HTML Tag Attribute CbSelect CbItem Checked ClickAsync Mouse DocProp" i)
if(flags&1) _s-"Htm."
ret _s
