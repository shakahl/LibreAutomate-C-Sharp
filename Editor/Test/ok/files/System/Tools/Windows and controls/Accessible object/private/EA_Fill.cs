 /
function hDlg Acc&a

___EA- dA
str sc sw name role value descr cls Id wfName sError
int i h x y cx cy state checkCls checkId isNVD iVal(-1)
ARRAY(str) z

DlgGrid g.Init(hDlg 1014); g.RowsDeleteAll
dA.isFilled=1
 g1
dA.ai=a

opt err 1
 get window, class, id, wfName
h=child(a) ;;info: for some controls returns top-level window
if(!IsWindow(h)) sError="Invalid window."; goto gError ;;eg destroyed while building tree. In Chrome invalid after switching tab.
if h=dA.hwnd
	RecGetWindowName h &sw
else
	RecGetWindowName h &sc 0 &sw ;;finds the best way to identify the control
	if(!sw.len) sw.swap(sc) ;;not child window, eg combolbox. Probably will not find; user can select nearest child, and navigate.
	else if(sc.beg("id(")) Id.gett(sc+3 0 " ")
	else if(!findrx(sc "^child\(''.*?'' ''(.+?)'' \{window\}(?: \w+ ''(?:id=(\d+)|wfName=(\w+))'')?" 0 0 z)) cls=z[1]; Id=z[2]; wfName=z[3]
	else out "EA_Fill: incorrect rx. sc=%s" sc
	if(Id.len) checkId=1; else i=GetDlgCtrlID(h); if(i>=0 and i<0x10000) Id=i
if(cls.len) cls.escape; else cls.getwinclass(h)
 check class?
checkCls=cls.len and !(h=dA.hwnd and !child("" "" h 0x410)) ;;note: only if has visible children, to ignore eg QM_toolbar_owner
 get main acc props
a.Role(role)
name=a.Name
value=a.Value
descr=a.Description
a.Location(x y cx cy)
state=a.State
opt err 0

 main props
sw.setwintext(id(10 hDlg))
role.setwintext(id(1012 hDlg))
name.setwintext(id(1011 hDlg))
TO_Check hDlg "1019" name.len!0
isNVD=name.len or wfName.len
 browser
int isdoc browser; Acc aDoc
if(cls.len) isdoc=EA_IsVisibleDoc(a browser aDoc)
if isdoc
	if(browser=1) checkCls=0 ;;in most cases don't need class
	else cls=""; Id=""; checkId=0 ;;in FF versions classname is different; in most browsers/versions it is top-level window
	if(browser=2) TO_Check hDlg "1053" 1 ;;+invisible, because in FF some objects in path are invisible
TO_Check hDlg "52" isdoc ;;in web page
dA.isFirefoxDocAlwaysBusy=browser=2 and isdoc and aDoc.State&STATE_SYSTEM_BUSY; err
 window, class, id, wfName
if(cls.len) sub.AddGridProp g "class" cls checkCls
if(Id.len) sub.AddGridProp g "id" Id checkId
if(wfName.len) sub.AddGridProp g "wfName" wfName 1
 value, description
if(value.len)
	if(!isNVD) sel(role) case ["TEXT","COMBOBOX"] iVal=g.RowsCountGet; case else isNVD|2
	sub.AddGridProp g "value" value isNVD&2 8
if(descr.len) sub.AddGridProp g "descr" descr !isNVD 8; isNVD=1
 HTML attributes
int isAttr=sub.FillHtmlAttributes(a g isNVD)
if(!isNVD and !isAttr and iVal>=0) g.RowCheck(iVal 1) ;;check value of TEXT/COMBOBOX if other properties missing or not checked
 x, y
DpiScreenToClient dA.hwnd +&x
sub.AddGridProp g "xy" F"{x} {y}"
 state
if(role="TEXT" and !wfName.len and !checkId) sub.AddGridProp g "state" F"0x{state} 0x{STATE_SYSTEM_READONLY|STATE_SYSTEM_PROTECTED}" !isAttr 16 ;;detect editable/readonly/password
else sub.AddGridProp g "state" F"0x{state~STATE_SYSTEM_HOTTRACKED} 0" 0 16 ;;just give state without mask/check
 flags, navig, match, wait
TO_Check hDlg "1029 1026" 1 1 ;;Use*, also unchecks Regexp.
TO_Check hDlg "23 24" 0 ;;Navig, Match #
TO_SetText dA.o.GetStr(iif(isdoc "defWaitWeb" "defWait")) hDlg 27 6 ;;Wait
 min/max levels. Use "0 1000" because difficult to get/update.
sub.AddGridProp g "level" "0 1000"

 Firefox
sub.FillFF hDlg browser isdoc

ret
err+ sError=_error.line
 gError
dA.hwnd=0
dA.ai.a=0; dA.ai.elem=0
sError-"Error. "; sError.setwintext(id(10 hDlg))


#sub FillFF
function hDlg browser isdoc

___EA- dA

int is=1
FFNode f.FromAcc(dA.ai); err is=0
if(is and browser=2 and isdoc and hid(id(6 hDlg)) and dA.o.GetCheck("FF")) TO_Check hDlg "6" 1 1
 info: Chrome (browser 3) also supports ISimpleDOMNode if Firefox is installed, but does not get text/HTML etc, therefore don't check the checkbox
TO_Show hDlg "6" is
if !is
	if(but(6 hDlg)) TO_Check hDlg "6" 0 1
	ret
opt err 1

str sTag sText; int nodeType useHTML
f.Info(sTag sText nodeType)
 tag
sTag.setwintext(id(1161 hDlg))
 text or HTML
if(nodeType=FFDOM.NODETYPE_ELEMENT) _s=f.HTML; if(_s.len) _s.swap(sText); _s="HTML"; else _s="Text"
else _s="Text"
_s.setwintext(id(1101 hDlg))
sText.setwintext(id(1157 hDlg))
useHTML=sText.len!0; if(useHTML and _s[0]='H' and findc(sText '<')>=0) useHTML=2
 node type
_s.getl("[]element[]attribute[]text[]cdata_section[]entity_reference[]entity[]processing_instruction[]comment[]document[]document_type[]document_fragment[]notation" nodeType)
_s=F"Node type:[]{nodeType} - {_s}"; _s.setwintext(id(1162 hDlg))
 atributes
ARRAY(str) a; int i j useAttr; str idVal accName=dA.ai.Name
f.AttributesAll(a 4)
DlgGrid g.Init(hDlg 1155); g.RowsDeleteAll
for i 0 a.len
	if useHTML!1 ;;if HTML is empty or with tags
		j=SelStr(1 a[0 i] "name" "id")
		sel j
			case [1,2] if(!idVal.len) idVal=a[1 i]; else if(a[1 i]~idVal) j=0 ;;if name and id same, don't use both
			case else if(accName.len and a[1 i]=accName) j=1; accName=0 ;;check if same as acc Name
		if(j) useHTML=0; useAttr=1
	sub.AddGridProp g a[0 i] a[1 i] j!0
if(useHTML!1 and useAttr=0) for(i 0 a.len) sel(a[0 i] 1) case ["href","src","title"] g.RowCheck(i 1); useHTML=0
 flags
TO_Check hDlg "1101" useHTML

TO_Check hDlg "1102 1105" 1 1 ;;Use*, also unchecks Regexp.


 notes:
 FindFF() skips branches of other root acc.
 Root acc roles:
  web content: DOCUMENT. By default, FindFF() searches only here.
  bookmarks pane: GROUPING. Items don't have nodes. This dialog does not create code to search here.
  other: APPLICATION. Use FindFF() with flag 0x2000. However does not find some, eg tab buttons.


#sub AddGridProp
function DlgGrid&g $name $value [check] [rowType] ;;rowType: 8 multiline, |16 button

str s1(name) s2(value)
int i=g.RowAddSetSA(-1 &s1 2)
if(rowType) g.RowTypeSet(i rowType)
if(check) g.RowCheck(i 1)


#sub FillHtmlAttributes
function# Acc&ac DlgGrid&g int&isNVD

ARRAY(str) a; int i
ac.WebAttributesAll(a 6); err ret

str idVal; int j isAttr
for i 0 a.len
	if !isNVD ;;if no name/value/descr
		j=SelStr(1 a[0 i] "name" "id")
		sel(j) case [1,2] if(!idVal.len) idVal=a[1 i]; else if(a[1 i]~idVal) j=0 ;;if name and id same, don't use both
		if(j) isAttr=1
	a[0 i]-"a:"
	sub.AddGridProp g a[0 i] a[1 i] j!0 8

ret isAttr
err+
