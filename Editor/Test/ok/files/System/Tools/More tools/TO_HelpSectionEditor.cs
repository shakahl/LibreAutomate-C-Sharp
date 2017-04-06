 \Dialog_Editor


lpstr dd=
 BEGIN DIALOG
 0 "" 0x90CC0AC8 0x80 0 0 120 0 "Function Help Editor"
 END DIALOG
 DIALOG EDITOR: "" 0x2030306 "*" "" ""

lpstr md=
 BEGIN MENU
 >&Link
	 Show function &help : 101
	 Open &QM Help topic : 102
	 &Open URL or file : 103
	 &Google search : 104
	 &Run macro : 105
	 Open &macro : 106
	 M&essage box : 107
	 <
 >&Style
	 &Bold : 110
	 &Italic : 111
	 &Underline : 112
	 -
	 &Text color : 113
	 &Highlight color : 114
	 &Line color : 115
	 -
	 &Code : 120
	 >I&mage
		 &Resource : 121
		 &File : 122
		 <
	 &Ordinary text : 123
	 <
 >&Spec
	 Description : 151
	 Returns: : 152
	 Parameters : 153
	 Remarks : 154
	 See also: : 155
	 Added in: : 156
	 Errors: : 157
	 Version: : 158
	 Author: : 159
	 Example : 160
	 -
	 Multiple... : 150
	 <
 &Help : 200
 END MENU

if(!ShowDialog(dd &sub.Dlg 0 _hwndqm 0 0 0 0 0 0 0 md)) ret


#sub Dlg
function# hDlg message wParam lParam

if(sub_to.ToolDlgCommon(&hDlg "0[]$qm$\tip.ico")) ret wParam
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 500 0
	
	case WM_TIMER
	sel wParam
		case 1 ;;autoupdate Tips
		err-
		int-- t_item t_crc; int item crc
		QMITEM q; item=qmitem("" 0 q 8|1)
		crc=Crc32(q.text q.text.len)
		if(item!t_item or crc!t_crc)
			t_item=item; t_crc=crc
			QmHelp q.name
		err+
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 150 if(sub.SpecDlg(hDlg _s)) sub.Spec _s 1
	case 200 QmHelp "IDP_F1"
	case else
	if(wParam>=101 and wParam<=123) sub.Tag hDlg wParam-100
	else if(wParam>=151 and wParam<=160) if(MenuGetString(GetMenu(hDlg) wParam &_s)) sub.Spec _s 0
ret 1


#sub Tag
function! hDlg i

str tag.gett("help,help,link,google,macro,open,mes,_,_,b,i,u,c,z,Z,_,_,_,_,code,image,image,_" i-1)
str s attr
int code line isSimpleAttr

sel i
	case 2 ;;help topic
	err-
	int w=win("QM Help" "HH Parent")
	if(w) act w; else w=QmHelp("IDH_REFERENCE")
	if(mes("Open the topic, then click OK." "" "OCn")!='O') ret
	Acc ap=acc("" "PANE" w "Internet Explorer_Server" "mk:@MSITStore*" 0x1004)
	s=ap.Value
	findrx(s "\bID\w_\w+(?=\.html)" 0 1 attr); attr-"#"
	err+ mes _error.description; ret
	isSimpleAttr=1
	
	case 3 ;;link
	if(!inp(attr "URL or file path. Can be empty if it is the selected text.")) ret
	
	case 4 ;;google
	if(!inp(attr "Search query. Can be empty if it is the selected text.")) ret
	
	case 7 ;;mes
	if(!inp(attr "Text to show in message box." "" "[]")) ret
	
	case [13,14,15] ;;color
	if(!ColorDialog(0 attr)) ret
	line=i=15
	isSimpleAttr=1
	
	case 20 ;;code
	code=1
	
	case 21 ;;image resource
	men 33553 _hwndqm ;;Resources
	out "To insert image in function help, use the Resources dialog.[][9]Click a resource name link and select Insert in code -> Image in function help."
	ret
	
	case 22 ;;image file
	if(!OpenSaveDialog(0 attr ".ico, .png, .jpg, .gif[]*.ico;*.png;*.jpg;*.gif[]All Files[]*.*[]")) ret
	attr.expandpath(attr 2)

if attr.len
	if(isSimpleAttr) attr-" "
	else attr.escape(1); attr=F" ''{attr}''"

int h=GetQmCodeEditor

if(line) SendMessage h SCI.SCI_HOME 0 0; SendMessage h SCI.SCI_LINEENDEXTEND 0 0
int j=sub_to.SciGetSelText(h s)

int LS LE
LS=j=SendMessage(h SCI.SCI_POSITIONFROMLINE -1 0)
LE=s.end("[]")

if(LS and s.beg(" ")) s.get(s 1)
if(LS and code) s-"[] "
if(LE) if(code) s+" "; else s.fix(s.len-2)
s=F"<{tag}{attr}>{s}</{tag}>"
if(LS) s-" "
if(LE) s+"[]"

 out s; ret

act h; err
SendMessage h SCI.SCI_REPLACESEL 0 s
ret 1


#sub Spec
function $csv !multi

str s st sf saa ex sn sv
ICsv c._create
int i n iIns

st.getmacro
iIns=findrx(st "^function\b.*([]|\z)" 0 8 sf)
c.FromString(csv)

for i 0 c.RowCount
	sn=c.Cell(i 0); sv=iif(multi c.Cell(i 1) "")
	if(sn.beg("<")) sn.gett(sn 1 ">" 2)
	
	sel sn 2
		case "Description"
		if(!sv.len) sv="Short_description_what_this_function_does."
		s+F" {sv}[]"
		
		case "Parameters"
		if(iIns<0) continue
		n=__LocalVarGetAll(&saa &sf); if(!n) continue
		s+"[]"
		lpstr sa=saa
		rep n
			sel(sa)
				case "flags" s+" flags:[]   1 -[]   2 -[]"
				case else s+F" {sa} -[]"
			sa+len(sa)+1
		s+"[]"
		
		case "See also:"
		if(!sv.len and !multi) sv="Function1, Function2."
		sv.replacerx("\w+" "<$0>")
		goto g1
		
		case "Added in:"
		if(!sv.len) sv=F"QM {_qmver_str}."
		goto g1
		
		case "*:"
		 g1
		if(sv.len) sv-" "
		s+F" {sn}{sv}[]"
		
		case else ;;Remarks, Example, user-defined
		sn.ucase
		if(sv.len) sv.replacerx("^(\t*)" "$1 " 8)
		sv=F"[] {sn}[]{sv}[][]"
		if(sn="EXAMPLE") ex=sv; else s+sv

s+ex

int h=GetQmCodeEditor
act h; err

if !multi
	InsertStatement s.trim("[]")
	ret

s.findreplace("[][][]" "[][]")
s.ltrim("[]"); s-"[]"
if(!s.end("[][]")) s+"[]"

 out; out s; ret

if(iIns<0) iIns=findrx(st "^([ ;]?[/\\].*(\n|\z))*" 0 0 sf)
if(iIns<0) iIns=0; else iIns+sf.len
SendMessage h SCI.SCI_GOTOPOS iIns 0
SendMessage h SCI.SCI_REPLACESEL 0 s


#sub SpecDlg
function# hDlg str&csv

lpstr dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 240 134 "Function Help Editor - Special Lines"
 3 QM_Grid 0x56035041 0x200 0 0 240 112 "0x27,0,0,4,0x0[],29%,,[],70%,,"
 1 Button 0x54030001 0x0 4 116 48 14 "OK"
 2 Button 0x54030000 0x0 56 116 48 14 "Cancel"
 END DIALOG

str controls = "3"
str qmg3x
qmg3x=
F
 <////2>Description
 <////2>Returns:
 <7////2>Parameters, <gets from code>
 Remarks
 See also:,"Function1, Function2."
 Added in:,QM {_qmver_str}.
 Errors:
 Version:
 Author:
 <////2>Example
 
if(!ShowDialog(dd 0 &controls hDlg)) ret
csv=qmg3x
ret 1
