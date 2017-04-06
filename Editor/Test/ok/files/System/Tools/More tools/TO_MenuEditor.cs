\Dialog_Editor

type ___MENUEDITOR
	hmain hgrid htb
	mdMacro ~mdSub
	DlgGrid'g
	__ImageList'ilTB __ImageList'ilGrid
	ARRAY(str)undo undoPos !undoDisabled
	!save
___MENUEDITOR m

str dd=
 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x40000 0 0 350 300 "Menu Editor"
 3 QM_Grid 0x52039049 0x200 0 20 350 280 "0x184,0,0,0,0x0[]Text,40%,16,[]id,13%,,[]Hotkey,12%,16,[]Disabled,12%,2,[]Checked,12%,2,[]Radio check,16%,2,[]Owner draw,16%,2,[]Right justify,16%,2,[]Right order,16%,2,"
 4 ToolbarWindow32 0x54031945 0x0 0 0 352 20 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040201 "" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0 _hwndqm 1 0 0 0 0 0 "$qm$\menu2.ico")) ret
rep
	if(!IsWindow(m.hmain)) break
	MSG msg; if(GetMessage(&msg 0 0 0)<1) DestroyWindow(m.hmain); break
	sel msg.message
		case WM_KEYDOWN if(sub.OnKeyDownInMessageLoop) continue
	if(IsDialogMessage(m.hmain &msg)) continue
	TranslateMessage &msg
	DispatchMessage &msg
if(msg.message=WM_QUIT) PostQuitMessage msg.wParam


#sub OnKeyDownInMessageLoop v
function!
if(msg.hwnd!m.hgrid) ret
int vk(msg.wParam) mod(GetMod)
sel vk
	case VK_RETURN if(!mod) msg.wParam=VK_INSERT
	case VK_ESCAPE if(!mod) men 16 m.hgrid ;;select none
	case [VK_RIGHT,VK_LEFT] if(mod=2) sub.OnIndent(vk=VK_LEFT); ret 1
	case ['Z','Y'] if(mod=2) sub.Undo iif(vk='Z' 1 2)


#sub DlgProc v
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG m.hmain=hDlg; sub.Create
	case WM_CLOSE sub.Close
	case WM_HELP goto gHelp
	case WM_APP SetFocus m.hgrid
	case WM_USER+7 ret TO_FavRet(lParam 0 "$qm$\menu2.ico")
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case [IDOK,IDCANCEL] ret
	case 1001 sub.Save
	case 1002 sub.Close
	case 1003 sub.Undo 1
	case 1004 sub.Undo 2
	case 1101 sub.Preview
	case 1102 sub.EventCode
	case 1050 sub.OnIndent 1
	case 1051 sub.OnIndent 0
	case [1013,1014,1006,1007,1008,1011] men wParam-1000 m.hgrid ;;up, down, cut, copy, paste, insert
	case 1100
	 gHelp
	QmHelp "IDH_DIALOG_EDITOR#a13"
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 3 ret sub.OnNotify_Grid(nh)
	case 4
	sel nh.code
		case TBN_DROPDOWN
		NMTOOLBAR* nt=+nh
		sel nt.iItem
			case 1011 sub.InsertSpecItem


#sub Create v
m.ilTB.Load("$qm$\il_de.bmp")
m.htb=id(4 m.hmain 1)
lpstr btns=
 1011 42 "Insert    Enter" 8
 1006 40 "Cut    Ctrl+X"
 1007 34 "Copy    Ctrl+C"
 1008 41 "Paste    Ctrl+V"
 1050 36 "Move left    Ctrl+Left"
 1051 37 "Move right    Ctrl+Right"
 1013 38 "Move up    Ctrl+Up"
 1014 39 "Move down    Ctrl+Down"
 -
 1003 16 "Undo    Ctrl+Z" 0 4
 1004 17 "Redo    Ctrl+Y" 0 4
 -
 1102 32 "Event code[]Adds or finds code in the dialog procedure for a menu-item-click event."
 1101 18 "Menu preview[]Shows as a menu bar in a dialog.[]With Ctrl shows as a popup menu."
 -
 1001 15 Save 0x40
 1002 27 Close 0x40
 -
 1100 20 "Help    F1"
TO_TbInit m.htb btns m.ilTB 0 BTNS_AUTOSIZE 0 15 TBSTYLE_EX_MIXEDBUTTONS|TBSTYLE_EX_DRAWDDARROWS

m.hgrid=id(3 m.hmain 1)
m.g.Init(m.hmain 3)
sub.GridSend LVM_SETBKCOLOR 0 0xC0E0E0 ;;easier to see the menu. Not COLOR_APPWORKSPACE, because same as border, it is confusing.
def ___ME_INDENT 4
m.ilGrid=ImageList_Create(6 1 0 0 0) ;;indent ___ME_INDENT*6 pixels, and +6 pixels margin to select easier.
m.g.SetImagelist(m.ilGrid)
DT_SetAutoSizeControls m.hmain "3s"

sub.Open


#sub Open v
int i nMD mdMacro
str s
__Subs x

 get macro text, find all subs and menu definitions
mdMacro=qmitem
nMD=x.Init(mdMacro 0 1); if(!nMD) goto gDefaultMD

 get menu definition
if nMD=1
	for(i x.a.len-1 0 -1) if(x.a[i].dd.len) break
else ;;show list of subs containing DD, let user select
	x.List(s 0 mdMacro)
	i=ListDialog(s "Which menu to edit?" "Menu Editor" 2)-1
	if(i<0) PostMessage m.hmain WM_CLOSE 0 0; ret
__Sub& r=x.a[i]
s.get(x.sText r.dd.innerOffset r.dd.innerLen)

m.mdMacro=mdMacro
m.mdSub=r.name

sub.LoadMD s
ret

 gDefaultMD
s=
 >&File
 &Open :501 0 0 Co
 &Save :502 0 0 Cs
 >Submenu
 Item1 :551
 Item2 :552
 <
 -
 E&xit :2
 <
 >&Edit
 Cu&t :601
 &Copy :602
 &Paste :603
 <
 >&Help
 &About :901
 < 
sub.LoadMD s
sub.Tooltip "This is a new menu. Edit it as you need."


#sub LoadMD v
function $md

ARRAY(str) a=md
lpstr sText sId sType sState sHK
int i j nt isSub isSep level nextid=10000

for(i 0 a.len) a[i].ltrim(" [9],;"); a[i].rtrim
for i 0 a.len
	sText=a[i]; if(!sText) continue
	isSep=0
	sel sText[0]
		case '>' sText+1; if(i=a.len-1) else if(a[i+1].beg("<")) a[i+1].all; else isSub=1 ;;is sub only if not last item and not followed by <
		case '<' if(level) level-1; continue; else break
		case ['-','|'] isSep=1
	
	str gtxt gid ghk gdis gcheck grc god grj gro
	str* gp=&gtxt; for(j 0 9) gp[j].all
	if isSep
		gtxt.left(sText 1)
	else
		sType=0; sState=0; sHK=0
		sId=strstr(sText " :")
		if(sId) sId[0]=0; nt=tok(sId+2 &sId 4 " [9]''" 5); if(!nt) sId=0
		
		gtxt=sText; gtxt.escape
		
		j=val(sId); if(!j and !isSub) j=nextid; nextid+1
		if(j) gid=j
		
		ghk=sHK
		
		j=val(sState)
		if(j&MFS_DISABLED) gdis="Yes"
		if(j&MFS_CHECKED) gcheck="Yes"
		
		j=val(sType)
		if(j&MFT_RADIOCHECK) grc="Yes"
		if(j&MFT_OWNERDRAW) god="Yes"
		if(j&MFT_RIGHTJUSTIFY) grj="Yes"
		if(j&MFT_RIGHTORDER) gro="Yes"
	
	LVITEM lv.mask=LVIF_INDENT; lv.iIndent=level*___ME_INDENT
	m.g.RowAddSetSA(-1 &gtxt 9 0 0 lv)
	
	if(isSub) level+1; isSub=0

sub.Undo


#sub Save v
function!

sub.Normalize
str md
sub.FormatMenuDefinition md

#region  find destination macro/sub, and get text into txtAll/txtSub
 get m.mdMacro text into txtAll
rep
	QMITEM q
	if(m.mdMacro) qmitem m.mdMacro 0 q 1|8; err m.mdMacro=0 ;;error if macro deleted
	if(m.mdMacro) break
	if(mes("Save the menu in current macro?" "Menu Editor" "OC?")!='O') ret
	m.mdMacro=qmitem
	md+"[][]  menu bar example:[] if(!ShowDialog(dd &sub.DlgProc &controls 0 0 0 0 0 0 0 0 md)) ret[]  popup menu example:[] int i=ShowMenu(md); out i"
	int isNew=1
str txtAll.swap(q.text) txtSub sList

 find our sub and get its text into txtSub
__Subs x.Init(0 txtAll 1)
int iSub=x.FindSub(m.mdSub)
if isNew or iSub<0
	if(x.a.len=1) iSub=0
	else
		x.List(sList 1 m.mdMacro)
		iSub=ListDialog(sList "Where to save the menu?" "Menu Editor" 2 m.hmain)-1
		if(iSub<0) ret
	m.mdSub=x.a[iSub].name
__Sub& r=x.a[iSub]
txtSub.get(txtAll r.codeOffset r.codeLen)

#region-  update MD in txtSub
int mdOffset
if(r.dd.len) mdOffset=r.dd.offset-r.codeOffset
else ;;no MD in the sub. Insert at the end.
	md-"[]str md=[]"; md+"[]"
	mdOffset=findrx(txtSub "(?m)^str dd=[]")
	if(mdOffset<0) mdOffset=findrx(txtSub "(?m)^[\t,]*(?=[^ ;/\\\t,]).*?\bShowDialog\b")
	if(mdOffset<0) mdOffset=txtSub.len; if(txtSub.end("[][]")) mdOffset-2
	else md+"[]"
txtSub.replace(md mdOffset r.dd.len)

#region-  replace macro text, return 1
txtAll.replace(txtSub r.codeOffset r.codeLen) ;;replace old sub text in txtAll with the new sub text
txtAll.setmacro(m.mdMacro)
err ;;error if read-only
	sel mes(F"{_error.description}[][]Failed to update menu in current macro. Possibly it is read-only. Create new macro for it?" "Error" "OCx")
		case 'O' m.mdMacro=newitem("" txtAll q.name "" "" 4)
		case else ret

m.save=0
ret 1
err+ mes _error.description "Error" "x"
#endregion


#sub FormatMenuDefinition v
function str&so

so=" BEGIN MENU[]"
str s
int i ind pind plen idm n=m.g.RowsCountGet
for i 0 n
	ARRAY(str) a; LVITEM lvi.mask=LVIF_INDENT
	m.g.RowGetSA(a i 9 0 0 lvi)
	
	ind=lvi.iIndent/___ME_INDENT
	if(ind>pind) so.insert(">" plen+pind)
	else for(pind pind ind -1) so.formata("%.*m<[]" pind 9)
	pind=ind; plen=so.len
	
	str& st=a[0]
	sel st 2
		case ["-*","|*"] s.format("%.*m%.1s" ind 9 st)
		case else
		int state=iif(a[3]~"Yes" MFS_DISABLED 0)|iif(a[4]~"Yes" MFS_CHECKED 0)
		int typ=iif(a[5]~"Yes" MFT_RADIOCHECK 0)|iif(a[6]~"Yes" MFT_OWNERDRAW 0)|iif(a[7]~"Yes" MFT_RIGHTJUSTIFY 0)|iif(a[8]~"Yes" MFT_RIGHTORDER 0)
		st.escape(1); st.findreplace(" :" "[91]32]:")
		idm=sub.ValidateGridEditField(1 a[1]); if(idm<0) idm=0
		if(sub.ValidateGridEditField(2 a[2])<0) a[2].all
		s.format("%.*m%s :%i 0x%X 0x%X %s" ind 9 st idm typ state a[2])
		sub_to.Trim s " :0 0x0 0x0 "
	so.addline(s)
for(pind pind 0 -1) so.formata("%.*m<[]" pind 9)
so.findreplace("[]" "[] ")
so+"END MENU"


#sub Close v
SetFocus m.hgrid
if m.save
	MES k.hwndowner=m.hmain; k.style="YNC?"
	sel mes("Save changes?" "" k)
		case 'Y' if(!sub.Save) ret
		case 'C' ret
DT_Cancel m.hmain


#sub OnNotify_Grid v
function# NMHDR*nh

GRID.QM_NMLVDATA* cd=+nh
NMLVDISPINFO* di=+nh
sel nh.code
	case LVN_BEGINLABELEDIT
	if(di.item.iSubItem=1) SetWinStyle child("" "Edit" m.g) ES_NUMBER 1
	
	case LVN_ENDLABELEDIT
	if(sub.ValidateGridEditField(di.item.iSubItem di.item.pszText)<0) ret DT_Ret(m.hmain 1)
	
	case GRID.LVN_QG_BUTTONCLICK
	sel cd.subitem
		case 0 sub.OnGridButton_Text cd.hctrl
		case 2 sub.OnGridButton_Hotkey cd.hctrl
	
	case GRID.LVN_QG_CHANGE
	if(!cd.hctrl) sub.Undo


#sub SelectChildren v
 Selects descendants of the last selected item.

ARRAY(int) a
m.g.RowsSelectedGet(a)
if(!a.len) ret
int i ind
i=a[a.len-1]
ind=sub.IndentGet(i)
for(i i+1 m.g.RowsCountGet) if(sub.IndentGet(i)>ind) m.g.RowSelect(i 1); else break


#sub OnIndent v
function !back
SetFocus m.hgrid
sub.SelectChildren
ARRAY(int) a
m.g.RowsSelectedGet(a)
int i
for(i 0 a.len) if(!sub.IndentSet(a[i] iif(back -1 1) 1)) break
if(i) sub.Undo


#sub IndentGet v
function# item

LVITEM lv.iItem=item; lv.mask=LVIF_INDENT
if(!sub.GridSend(LVM_GETITEM 0 &lv)) ret
ret lv.iIndent/___ME_INDENT


#sub IndentSet v
function! item indent [flags] ;;flags: 1 inc/dec
 Returns 1 if changed, 0 if not (same or invalid).

if(item<1) ret
LVITEM lv.iItem=item; lv.mask=LVIF_INDENT
if(!sub.GridSend(LVM_GETITEM 0 &lv)) ret
int j i(lv.iIndent/___ME_INDENT) i0=(i)
if(flags&1) i+indent; else i=indent

if(i<0) i=0
else if(i>1) j=sub.IndentGet(item-1); if(i>j+1) i=j+1 ;; must be max 1 more than prev item
if(i=i0) ret

lv.iIndent=i*___ME_INDENT
sub.GridSend(LVM_SETITEM 0 &lv)
ret 1


#sub Normalize v

SetFocus m.hgrid

 remove empty rows
int i nb changed
for i m.g.RowsCountGet-1 -1 -1
	if(m.g.RowGetMS(i 9 0 2 nb) and nb=-2) changed+m.g.RowDelete(i)

 correct indent
int ind pind(-1)
for i 0 m.g.RowsCountGet
	ind=sub.IndentGet(i)
	if(ind>pind+1) ind=pind+1; changed+sub.IndentSet(i ind)
	if(ind=pind+1 and i) sel(m.g.CellGet(i-1 0) 2) case ["-*","|*"] ind-1; changed+sub.IndentSet(i ind)
	pind=ind
if(changed) sub.Undo


#sub InsertSpecItem v
 Called on Insert button drop-down arrow.

int i=ShowMenu("1Separator[]2Vertical separator" m.hmain)
sel i
	case [1,2]
	m.undoDisabled=1
	sub.GridSend WM_KEYDOWN VK_INSERT
	SendMessage GetFocus WM_CHAR iif(i&1 '-' '|') 0
	m.undoDisabled=0
	SetFocus m.hgrid


#sub OnGridButton_Text v
function hedit

int i=ShowMenu("1Underline next character[9]&&[]2&&[9]&&&&[]3Tab (right-align text after)[]-[]4Undo[]-[]5End cell editing[9]Esc[]6Select none[9]Esc Esc" m.hmain)
if(!i) ret
sel i
	case 1 EditReplaceSel hedit 0 "&"
	case 2 EditReplaceSel hedit 0 "&&"
	case 3 EditReplaceSel hedit 0 "[9] "
	case 4 SendMessage hedit WM_UNDO 0 0
	case 5 PostMessage m.hmain WM_APP 0 0 ;;now SetFocus does not work
	case 6 men 16 m.hgrid


#sub OnGridButton_Hotkey v
function hedit

str dd=
 BEGIN DIALOG
 5 "" 0x90C80AC8 0x0 0 0 144 52 "Menu Item Hotkey"
 3 msctls_hotkey32 0x54030000 0x200 8 8 128 12 ""
 1 Button 0x54030001 0x4 8 32 48 14 "OK"
 2 Button 0x54030000 0x4 60 32 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040200 "*" "" "" ""

str controls = "3"
str hk3
hk3.getwintext(hedit)
if(!ShowDialog(dd 0 &controls m.hmain)) ret
hk3.setwintext(hedit)


#sub ValidateGridEditField v
function# isub $txt
 Returns -1 if txt invalid.
 Returns val(txt) if isub=1.
 Else returns 0.

str tt; int i
sel isub
	case 1 ;;id
	i=val(txt)
	if(i>=0 and i<=0xffff) ret i
	tt=F"Invalid id {txt}. Must be 0 to 65535."
	
	case 2 ;;hotkey
	if(empty(txt) or TO_HotkeyFromQmKeys(txt i i)) ret
	tt=F"Invalid hotkey {txt}."
	
	case else ret

sub.Tooltip tt
ret -1


#sub Preview v

sub.Normalize
str md
sub.FormatMenuDefinition md

if GetMod&2
	ShowMenu(md)
	ret

str dd=
 BEGIN DIALOG
 0 "" 0x90CC0AC8 0x0 0 0 300 68 "Menu Preview"
 END DIALOG
 DIALOG EDITOR: "" 0x2040200 "" "" "" ""

if(!ShowDialog(dd 0 0 m.hmain 0 0 0 0 0 0 "" md)) ret


#sub Undo v
function [action] ;;action: 0 changed, 1 undo, 2 redo

if(action) SetFocus m.hgrid ;;must be before, because on set focus this func may be called recursively

str md
sel action
	case 0 ;;changed
	if(m.undoDisabled) ret
	m.g.ToCsv(md "," 0x200000)
	if m.undo.len
		m.save=1
		m.undoPos+1
		if(m.undoPos<m.undo.len) m.undo.redim(m.undoPos) ;;after Undo
		else if(m.undo.len>100) m.undo.remove(0); m.undoPos-1 ;;limit to 100 undo actions
	m.undo[].swap(md)
	
	case 1 ;;undo
	if(m.undoPos<1) ret
	m.undoPos-1
	
	case 2 ;;redo
	if(m.undoPos>=m.undo.len-1) ret
	m.undoPos+1

if(action) m.g.FromCsv(m.undo[m.undoPos] ",")

SendMessage m.htb TB_ENABLEBUTTON 1003 m.undoPos>0
SendMessage m.htb TB_ENABLEBUTTON 1004 m.undoPos<m.undo.len-1


#sub EventCode v
int i ii
ARRAY(int) ai
ARRAY(STRINT) a
m.g.RowsSelectedGet(ai)
for i 0 ai.len
	ii=val(m.g.CellGet(ai[i] 1))
	if(ii<1 or ii>0xffff) continue
	STRINT& r=a[]
	r.i=ii
	r.s=m.g.CellGet(ai[i] 0); r.s.gett(r.s 0 "[9][]"); if(r.s.len) r.s-" ;;"
if(!a.len) sub.Tooltip "Select one or more items. At least one of them must have id."; ret

__Subs x
if(!x.EventsInit(m.mdMacro m.mdSub 1)) ret
for i a.len-1 -1 -1
	&r=a[i]
	x.EventsInsert(m.hmain m.mdMacro F"{r.i}" r.s 0)


#sub Tooltip v
function $s
sub_sys.TooltipOsd s 9 "Menu Editor tooltip" 0 0 0 m.htb


#sub GridSend v
function# message [wParam] [lParam]
ret SendMessageW(m.hgrid message wParam lParam)


