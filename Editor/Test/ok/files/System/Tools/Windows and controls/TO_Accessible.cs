 \Dialog_Editor
function [_1] [_2] [fav]

str dd=
 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 333 206 "Accessible object"
 6 Static 0x54020000 0x4 4 6 46 12 "Acc variable"
 7 Edit 0x54030080 0x204 52 4 28 14 "Var" "Accessible object variable. Optional.[]If you don't have it, use the Drag tool to create code."
 4 Static 0x54020003 0x4 88 2 16 16 "Drag" ".1 Drag and drop to capture object"
 5 Edit 0x54000884 0x0 118 2 214 20 ""
 3 ListBox 0x54230101 0x204 4 30 104 148 "Act"
 1112 ListBox 0x54230109 0x204 118 30 96 50 "Sel"
 1201 ListBox 0x54230109 0x200 118 30 96 50 "xy" "Select one or several"
 1528 Static 0x54020000 0x4 118 30 36 10 "Value"
 1527 Edit 0x54231044 0x204 118 40 212 70 "Val"
 1901 QM_DlgInfo 0x54000000 0x20000 118 30 212 148 "This will create code that gets all direct child objects. You can then edit the code in macro. GetChildObjects can get all descendants, descendants at certain level(s), with certain role, etc."
 2101 ListBox 0x54230101 0x200 118 30 96 50 "Mou"
 2133 Static 0x54020000 0x4 120 90 42 12 "Offset X, Y"
 2131 Edit 0x54030080 0x204 164 88 24 14 "mx"
 2132 Edit 0x54030080 0x204 190 88 24 14 "my"
 2201 ComboBox 0x54230243 0x0 118 30 54 213 "CB"
 2202 Edit 0x54030080 0x200 176 30 154 14 "CB"
 2401 Static 0x54000000 0x0 118 32 54 12 "Attribute name"
 2402 Edit 0x54030080 0x200 174 30 90 14 "Att"
 2501 ListBox 0x54230109 0x200 118 30 96 48 "" "Select one or several"
 2601 ListBox 0x54230109 0x200 118 30 96 48 "" "Select one or several"
 2701 QM_DlgInfo 0x54000000 0x20000 118 30 212 148 ""
 1 Button 0x54030001 0x4 4 188 48 14 "OK"
 2 Button 0x54010000 0x4 54 188 50 14 "Cancel"
 10 Button 0x54032000 0x4 106 188 16 14 "?"
 8 Static 0x54000010 0x20004 4 24 328 1 ""
 11 Static 0x54000010 0x20004 0 182 342 1 ""
 END DIALOG

str controls = "7 4 5 3 1112 1201 1527 2101 2131 2132 2201 2202 2402 2501 2601"
__strt e7Var si4Dra e5 lb3Act lb1112Sel lb1201xy e1527Val lb2101Mou e2131mx e2132my cb2201CB e2202CB e2402Att lb2501 lb2601

TO_FavSel fav lb3Act "Do default action[]Select[]Get location[]Get name[]Get value[]Set value[]Get description[]Get role[]Get state[]Get children[]Get selected children[]Mouse actions[]Select COMBOBOX item[]Scroll web page[]Get HTML attribute[]Get HTML properties[]Get web page properties[]More functions"
si4Dra="&$qm$\target.ico"
lb1112Sel="&Focus[]&Select[]Extend selection[]Add to selection[]Unselect"
cb2201CB="&Text[]Index"
lb2101Mou="Mouse move to[]&Left click[]Right click[]Middle click[]Left double click"
lb1201xy="X[]Y[]Width[]Height"
lb2501="Tag[]Inner HTML[]Outer HTML"
lb2601="URL[]Title[]HTML[]Text"

if(!ShowDialog(dd &sub.Dlg &controls _hwndqm)) ret

str s sout vdResult
__strt vd vd2
ARRAY(str) z
int i f A(val(lb3Act))
if(A=17) ret ;;just info
lpstr arraySampleCode=" sample code, shows how to use the array[]out[]for _i 0 [1].len[][9]Acc& r=[1][_i][][9]str role name[][9]r.Role(role); name=r.Name[][9]out F''{role} {name}''"

if(e5.len) vd.ReplaceVar(e5 e7Var); e5.s+"[]"; else e7Var.VN("a")
s=F"{e7Var}.{sub.AccFunction(A)}"

sel A
	case 1 ;;select
	for(i 0 5) if(lb1112Sel[i]='1') f|1<<i
	s+F"({f})"
	
	case 2 ;;location
	lb1201xy.LbSelectedItemsToNames(z "x y cx cy" "1111")
	vd.VD("-D int 0 0 0 0[]" z[0] z[1] z[2] z[3])
	s=F"{vd}{s}({z[0]} {z[1]} {z[2]} {z[3]})"; sub_to.Trim s " 0 0 0"
	
	case 3 s-F"{vd.VD(`str name`)}=" ;;get name
	case 4 s-F"{vd.VD(`str value`)}=" ;;get value
	case 5 s+F"({e1527Val.S})" ;;set value
	case 6 s-F"{vd.VD(`str descr`)}=" ;;get description
	
	case 7 s=F"{vd.VD(`-i str roleStr` _s)}[]{vd2.VD(`int roleInt`)}={s}({_s})" ;;get role
	case 8 s=F"{vd.VD(`-i str stateText` _s)}[]{vd2.VD(`int stateFlags`)}={s}({_s})" ;;get state
	
	case 9 ;;children
	vdResult="ARRAY(Acc) ac"; sout=arraySampleCode
	
	case 10 vdResult="ARRAY(Acc) aSel"; sout=arraySampleCode ;;selection
	
	case 11 ;;mouse
	f=val(lb2101Mou)
	if(e2131mx.len or e2132my.len) s+F"({f} {e2131mx.N} {e2132my.N})"
	else s+F"({f})"
	
	case 12 ;;combo
	if(val(cb2201CB)) e2202CB.N; else e2202CB.S
	s+F"({e2202CB})"
	
	case 14 vd=e2402Att; vd.VN("attr"); s=F"{vd.VD(`-sD str attr`)}={s}({e2402Att.S})" ;;get web attribute
	
	case 15 ;;html prop
	lb2501.LbSelectedItemsToNames(z "tag innerHTML outerHTML" "100")
	vd.VD("-D str 0 0 0[]" z[0] z[1] z[2])
	s=F"{vd}{s}({z[0]} {z[1]} {z[2]})"; sub_to.Trim s " 0 0"
	
	case 16 ;;web page prop
	lb2601.LbSelectedItemsToNames(z "url title html text" "1000")
	vd.VD("-D str 0 0 0 0[]" z[0] z[1] z[2] z[3])
	s=F"{vd}{s}({z[0]} {z[1]} {z[2]} {z[3]})"; sub_to.Trim s " 0 0 0"

if vdResult.len
	str vr
	s-F"{vd.VD(vdResult vr)}[]"
	if(!s.replacerx("\)$" " ")) s+"("
	s+F"{vr})"
	sout.findreplace("[1]" vr)

s-e5

 sub_to.TestDialog s A
InsertStatement s
if(sout.len) out "<><code>%s</code>" sout


#sub Dlg
function# hDlg message wParam lParam

if(sub_to.ToolDlgCommon(&hDlg "3[]$qm$\acc.ico[]TO_Accessible" "" 1)) ret wParam
sel message
	case WM_INITDIALOG goto gAction
	case WM_COMMAND goto messages2
	
	case WM_LBUTTONDOWN
	if(GetDlgCtrlID(child(mouse))=4) if(EA_Main(hDlg 5 1)) goto gAction
ret
 messages2
sel wParam
	case 10 QmHelp "IDP_ACCESSIBLE"
	
	case LBN_SELCHANGE<<16|3
	 gAction
	int i=TO_Selected(hDlg 3)
	DT_Page hDlg i
	if(i=17) goto gMore
	QmHelp sub.AccFunction(i 1)
ret 1

 gMore
_s=
 <>This dialog does not create code for:
 <help>Acc.ChildCount</help>
 <help>Acc.CompareProp</help>
 <help>Acc.FromEvent</help>
 <help>Acc.FromFFNode</help>
 <help>Acc.FromFocus</help>
 <help>Acc.FromHtm</help>
 <help>Acc.FromMouse</help>
 <help>Acc.FromWindow</help>
 <help>Acc.FromXY</help>
 <help>Acc.FromXYWindow</help>
 <help>Acc.Hwnd</help>
 <help>Acc.Navigate</help>
 <help>Acc.NotFound</help>
 <help>Acc.ObjectFocused</help>
 <help>Acc.ObjectFromPoint</help>
 <help>Acc.WebAttributesAll</help>
;
 See also: <help>Acc help</help>
_s.setwintext(id(2701 hDlg))


#sub AccFunction
function~ i [flags] ;;flags: 1 with "Acc."

_s.gett("DoDefaultAction Select Location Name Value SetValue Description Role State GetChildObjects ObjectSelected Mouse CbSelect WebScrollTo WebAttribute WebProp WebPageProp" i)
if(flags&1) _s-"Acc."
ret _s
