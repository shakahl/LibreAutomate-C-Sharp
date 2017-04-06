 __sub_to contains shared subs used in \System\Tools.
 __sub_sys contains shared subs used anywhere in \System.

#sub ToolDlgCommon
function! *_hDlg [$fav] [$drop] [flags] ;;flags: 1 detect read-only macro, 2 always use drop

 Processes messages to support: Favorites; Simple drag and drop; detecting read-only macro.
 Called before sel messages.
 If returns 1, dlgproc must return wParam.

 _hDlg - &hDlg
 fav - TO_FavRet parameters: "idAction[]icon[]func" (all optional). Can be 0 or "" if don't need.
 drop - if not empty or flags&2, registers drop target and on WM_QM_DRAGDROP calls TO_DropFiles with sid=drop.


__DIALOGPARAM* a=+_hDlg
sel a.message
	case WM_USER+7
	if !empty(fav)
		str sId sIcon sFunc
		str* sa=&sId; for(_i 0 3) if(sa[_i].getl(fav -_i)<0) break
		int iid=iif(sFunc.len qmitem(sFunc 1) getopt(itemid 1))
		a.wParam=TO_FavRet(a.lParam val(sId) sIcon +iid)
		ret 1
	
	case WM_INITDIALOG
	if(!empty(drop) or flags&2) QmRegisterDropTarget a.hDlg 0 16|32
	DT_MouseWheelRedirect
	
	case WM_QM_DRAGDROP
	if(!empty(drop) or flags&2) TO_DropFiles a.hDlg +a.lParam drop
	
	case WM_COMMAND
	if(flags&1 and a.wParam=IDOK and !sub.CanInsertStatement(a.hDlg)) a.wParam=0; ret 1


#sub CanInsertStatement
function# hDlg

 Call from tool dialogs on IDOK. Returns 0 if the macro where the code statement would be inserted is read-only.

str s.getmacro("" 8)
if(val(s)) sub_sys.MsgBox hDlg "This macro is read-only." "" "!"; ret
ret 1


#sub Trim
function# str&s $st
 Removes unused arguments from QM code statement string.

 s - statement, eg "Func x y 0 0" or "Func(x y 0 0)".
 st - default arguments.
   For example, if st is " 0 0", replaces "Func(x y 0 0)" with "Func(x y)"; replaces "Func(x y z 0)" with "Func(x y z)".

int i j p
if(s.end(")")) s.fix(s.len-1); p=1
for i 0 100000000
	if(s.end(st)) s.fix(s.len-len(st)); break
	st+1; j=findc(st 32); if(j<0) break
	st+j
if(p) s+")"
ret i


#sub SetUserFont
function hDlg $controls

 Sets user-defined font for one or more controls.

__Font-- f
if !f
	if(rget(_s "toolfont" "\Tools")) f.CreateFromString(_s)
	if(!f) ret

f.SetDialogFont(hDlg controls)


#sub FileDialog
function# hDlg idEdit $regdir [$defdir] [$filter] [$defext] [str&s] [idLnkParam]

if(empty(defdir)) defdir="$documents$"
if(empty(filter)) filter="All Files[]*.*[]"
if(!&s) &s=_s
int f=sub.File_GetPathFlags(hDlg)
str ss

rget ss regdir "\Tools" 0 defdir
 g1
if(!OpenSaveDialog(0 s.all filter defext ss 0 1)) ret
str se.expandpath(ss 2)
rset se regdir "\Tools"

SHORTCUTINFO si
if(s.endi(".lnk") and GetShortcutInfoEx(s &si) and FileExists(si.target 1)) ss=si.target; goto g1 ;;folder shortcut

if(f&1=0) sub.File_LinkTarget s 0 hDlg idLnkParam
if(f&2=0) s.expandpath(s 2)
if(idEdit) TO_SetText s hDlg idEdit
ret 1


#sub FolderDialog
function# hDlg idEdit [$defdir] [flags] [str&s] ;;flags: 1 include files, 2 include non-file objects (Control Panel, etc), 4 new style

if(!&s) &s=_s

if(!BrowseForFolder(s defdir flags)) ret

sub.File_UnexpandPathIfNeed hDlg s
if(idEdit) TO_SetText s hDlg idEdit
ret 1


#sub File_SF
function# hDlg idEdit [$append]

str s
if(!SpecFoldersMenu(hDlg &s)) ret
s+append

if(hDlg)
	if(sub.File_GetPathFlags(hDlg)&2) s.expandpath
	TO_SetText s hDlg idEdit 8
else s.setsel

ret 1


#sub File_FileMenu
function# hDlg idEdit _menu [str&s] [idLnkParam] [isFolder] ;;_menu: 0 predefined, 2 IE Favorites

if(!&s) &s=_s

sel _menu
	case 0 s=iif(_winnt>=6 "TO_FileRunMenu" "TO_FileRunMenuXP"); if(isFolder) s+"F"
	case 2 s="TO_FileWebFavorites"

if(!mac(s)) ret
GetLastSelectedMenuItem(0 &s)

int f=sub.File_GetPathFlags(hDlg)
if(f&1=0) sub.File_LinkTarget s 0 hDlg idLnkParam
if(f&2=0) s.expandpath(s 2)
if(idEdit) TO_SetText s hDlg idEdit
ret 1


#sub File_GetPathFlags
function# hDlg
 Returns flags:
   1 - hDlg has "Shortcut" button and it is checked.
   2 - hDlg has "No SF" button and it is checked.

int h f

if(!hDlg) ret
h=child("Shortcut" "Button" hDlg 1)
if(h and but(h)) f|1

h=child("No SF" "Button" hDlg 1)
if(h and but(h)) f|2

ret f


#sub File_UnexpandPathIfNeed
function hDlg str&s

if sub.File_GetPathFlags(hDlg)&2=0
	s.expandpath(s 2)
	if(s.begi("$windows$\SysWOW64\")) s.replace("$system$" 0 18)


#sub File_LinkTarget
function# str&s [SHORTCUTINFO&si] [hDlg] [idLnkParam]

SHORTCUTINFO si2
if(!&si) &si=si2
if(s.endi(".lnk") and GetShortcutInfoEx(s &si))
	s=si.target
	if(idLnkParam) TO_SetText si.param hDlg idLnkParam
	ret 1


#sub VirtualKeysMenu
function# hEdit part [replace] [str&sGet]

str s.getmacro("VirtualKeys")
int j i=find(s " VK_NUMLOCK")+1
if(part) s.get(s i); else j=find(s " VK_BACK")+1; s.get(s j i-j)
ARRAY(str) a
findrx(s "\bVK_\w+(?= +0x)" 0 4 a)
s=""
for(i 0 a.len)
	s.formata("%c%s[]" toupper(a[0 i][3]) (a[0 i].lcase+4))
	if(i+1%21=0) s+"|[]"
i=ShowMenu(s.rtrim("|[]") GetAncestor(hEdit 2) 0 2); if(!i) ret
_s.getl(s i-1)
s.from("(VK_" _s.ucase ")")

if &sGet
	sGet=s
else
	EditReplaceSel hEdit 0 s iif(replace 7 2)

ret 1


#sub MouseXY
function hDlg hwnd idx idy idclient [POINT&point] [flags] ;;flags: 1 support fractional coord (idx/idy are combobox)

 Fills Edit or ComboBox controls idx and idy with mouse coordinates relative to hwnd (0 if screen).
 idclient - id of 'Client area' checkbox.
 point - if used, uses its coord instead of current mouse coord. Must be screen coord.

POINT p; RECT r
int client

if(&point) p=point; else xm p

if(!hwnd or GetDlgCtrlID(GetFocus)=idclient) client=IsDlgButtonChecked(hDlg idclient)
else client=!__RecIsPointInNonclient(hwnd p.x p.y); CheckDlgButton hDlg idclient client

if hwnd
	if(client) DpiScreenToClient hwnd &p; DpiGetWindowRect hwnd &r 4
	else DpiGetWindowRect hwnd &r; p.x-r.left; p.y-r.top
else
	MonitorFromIndex -3 client &r
	if(client) p.x-r.left; p.y-r.top
OffsetRect &r -r.left -r.top

str s sf
rep 2
	s=p.x
	int h=id(idx hDlg)
	if flags&1
		sf=F"{0.0+p.x/r.right%%.3f}"
		int is=TO_Selected(h); if(is<0) is=0
		TO_CBFill h F"{s}[]{sf}" 0 is
		if(is) s=sf
	s.setwintext(h)
	idx=idy; p.x=p.y; r.right=r.bottom


#sub DragTool_Menu
function# hDlg $items [flags] [$helpText] ;;flags: 1 add "102 Capture with Shift key[]", 2 add tip "use Shift to zorder control", 4 add tip "Ctrl+Shift+Alt+W"

 Shows context menu of a Drag tool.
 Replaces "{+}" with common items: "100 Don't minimize windows[]101 Make this dialog always-on-top".
 Processes common items and returns selected item.
 Supports item states in items. See <help>MenuPopup.AddItems</help>.
 helpText - if not 0, adds "[]-[]103 Tips" item that shows message box with common tips + helpText.

MenuPopup m
int- t_dragToolFlags
int isTopmost=GetWinStyle(hDlg 1)&WS_EX_TOPMOST

str s sr="100 Don't minimize windows[]101 Make this dialog always-on-top"
if(flags&1) sr-"102 Capture with Shift key[]"
s=items; s.findreplace("{+}" sr 4)
if(helpText or flags&6) s+"[]-[]103 Tips"
m.AddItems(s 0 4)

if(t_dragToolFlags&1) m.CheckItems("100")
if(isTopmost) m.CheckItems("101")

 step 2
int R=m.Show(hDlg)
sel R
	case 100 t_dragToolFlags^1 ;;for __MinimizeDialog.Minimize
	case 101 Zorder hDlg iif(isTopmost HWND_NOTOPMOST HWND_TOPMOST) SWP_NOACTIVATE
	 case 102 t_dragToolFlags|1 ;;also check "no minimize"
	case 103
	s="While drag-capturing, you can right click a window to send it behind other windows. Don't release the left button."
	if(flags&2) s+"[][]Use Shift to send a control behind other controls."
	if(flags&4) s+"[][]You can open this dialog anywhere with Ctrl+Shift+Alt+W."
	if(helpText) s+"[][]"; s+helpText
	mes s "" "i"
ret R


#sub DragTool_Loop
function! __Drag&d

 Calls d.Next, sets red cross cursor, on rbuttonup zorders windows. Returns d.Next.

if(!d.Next) ret
d.cursor=4
if(d.m.message=WM_RBUTTONUP) SetWindowPos iif(d.mk&MK_SHIFT child(mouse) win(mouse)) HWND_BOTTOM 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE; 0.1
ret 1


#sub Test
function# hDlg ~statement [param] [flags] [idTimeControl] [$testMacro] ;;flags: 1 add "err+ sub_sys.MsgBox ...; end" line

 Takes caller's or testMacro's text after '#ret' line, replaces another '#ret' line with statement, and runs the text in new thread async.
 Returns thread handle.
 Passes hDlg, statement and param as arguments.
 Adds "#err sub_to.Test_CompileError" line after each line of statement.
 If Ctrl pressed, displays statement in output.
 Also stores statement execution time (ms) into local double variable _t. If idTimeControl, displays in the control.
 Sets spe 100.


ifk(C) out statement
if(idTimeControl) _s.setwintext(id(idTimeControl hDlg)); err

str s=statement
if(!s.end("[]")) s+"[]"
s.findreplace("[]" "[]#err sub_to.Test_CompileError[]")

if(flags&1) s+"err+ sub_sys.MsgBox hDlg _error.description ''Error'' ''!''; end[]"

s-"spe 100[]long __t=perf[]"
s+"double _t=perf-__t/1000.0; _t=Round(_t iif(_t<1 2 _t<10))[]"
if(idTimeControl) s.formata("SetDlgItemText hDlg %i F''Time: {_t} ms''[]" idTimeControl)

str m; if(empty(testMacro)) m.getmacro(getopt(itemid 1)); else m.getmacro(testMacro)
m.get(m find(m "[]#ret[]")+8)
m.findreplace("#ret" s 4)

 out; out m

ret RunTextAsFunction(m hDlg statement param)


#sub Test_CompileError
MsgBoxAsync "Used variables or incorrect expressions." "Cannot test" "x"
ret -1


#sub Test_ActWin8
function hwnd

 On Windows 8/10, if the window is cloaked, activates it.
 Call before testing acc etc, because the window may be suspended and objects unavailable.

if(!IsWindowCloaked(hwnd)) ret

sel WinTest(hwnd "ImmersiveLauncher")
	case 1 ;;Win8 Start screen
	 key W ;;may show eg ClassicShell's Start menu
	PostMessage _hwndqm WM_SYSCOMMAND SC_TASKLIST 0
	 shoulddo: do the same for Win10 Start screen. But difficult to recognize it. Not necessary.
	
	case else
	act hwnd
0.25

err+


#sub Mail_SetAccountsCb
function hcb

SendMessage hcb CB_RESETCONTENT 0 0
ARRAY(__REGEMAILACCOUNT) a; int i
MailGetAccounts(a)
CB_Add hcb "<Default>"
for(i 0 a.len) CB_Add hcb a[i].name


#sub Mail_GetAccountVar
function ~&s __strt&sa ~sv pop

if(sv=1)
	___EMAILACCOUNT ac.Get(sa 3)
	s.getstruct(ac 1)
	s.replacerx(iif(pop "(?s).+?[](p.+)" ".+[](?s)(.+?[])p.+") "$1")
	s-"_s=[]"
	s.replacerx("[](?!$)" "[] ")
	sa="_s"
else sa.S

err+ out F"<>Error: {_error.description}"


#sub GridCsvSetVar
function ICsv&c $rows $defCSV int&defUsed

 Creates c from rows and defCSV, to pass to a QM grid control.

 rows - 2-col CSV (names and values). Values usually empty. Supports QM grid row type. Example: "aaa,[]<8>bbb".
 defCSV - 2-col CSV that sets values. Separator =. Can be empty or have any number of rows in any order.
 defUsed - receives mask of rows found in defCSV.

defUsed=0
c._create; c.FromString(rows)

if(empty(defCSV)) ret
ICsv d._create; d.Separator="="; d.FromString(defCSV)
int i j
for i 0 c.RowCount
	lpstr s(c.Cell(i 0)) st
	if(s[0]='<') st=s; s+findc(s '>')+1; else st=0
	j=d.Find(s 1); if(j<0) continue
	str s1 s2(d.Cell(j 1))
	if(st) s1.format("%.*s////2>%s" s-st-1 st s); else s1.format("<////2>%s" s)
	c.ReplaceRowSA(i 2 &s1)
	defUsed|1<<i

err+


#sub SciGetSelText
function# h str&s ;;Returns sel start. If h 0, calls GetQmCodeEditor.

if(!h) h=GetQmCodeEditor
TEXTRANGE t
t.chrg.cpMin=SendMessage(h SCI.SCI_GETSELECTIONSTART 0 0)
t.chrg.cpMax=SendMessage(h SCI.SCI_GETSELECTIONEND 0 0)
t.lpstrText=s.all(t.chrg.cpMax-t.chrg.cpMin)
s.fix(SendMessage(h SCI.SCI_GETTEXTRANGE 0 &t))
ret t.chrg.cpMin


#sub RegKeyExists
function! $key_ [hive]

RegKey rk
ret rk.Open(key_ hive KEY_READ)


#sub IsRectInRect
function RECT&rOuter RECT&rInner

ret PtInRect(&rOuter rInner.left rInner.top) and PtInRect(&rOuter rInner.right-1 rInner.bottom-1)


#sub FstringVar
function hDlg idEdit

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 165 95 "Add variable"
 3 Static 0x54000000 0x0 4 44 110 10 "Variable or other expression"
 4 Edit 0x54030080 0x200 4 56 158 14 ""
 5 QM_DlgInfo 0x54000000 0x20000 0 0 166 38 "<>To insert a variable in text, use this dialog or type {variable} in the main dialog.[][]More info: <help #IDP_FSTRING>F string</help>. See also: <help>str.format</help>, <help>out</help>."
 1 Button 0x54030001 0x4 4 76 48 14 "OK"
 2 Button 0x54030000 0x4 54 76 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

str controls = "4"
__strt e4
if(!ShowDialog(dd 0 &controls hDlg)) ret
e4.trim(" }{")
EditReplaceSel hDlg idEdit F"{{{e4}}" 2


#sub TestDialog
function str&s [action] [flags] ;;flags: 1 don't clear output

 Outputs code formatted by caller dialog and runs the dialog again with same action.

if(flags&1=0) out
out F"<><code>{s}</code>"
TO_Fav _s.getmacro(getopt(itemid 1) 1) action


#sub SetTextNoNotify
function hEdit $txt [flags] ;;flags: 1 undoable

 Sets Edit control text. Parent will not receive EN_CHANGE.

int p=GetParent(hEdit)
int x=SubclassWindow(p &DefWindowProcW)
if(flags&1) EditReplaceSel hEdit 0 txt 1
else SendMessageW hEdit WM_SETTEXT 0 @txt
err+
SubclassWindow(p x)


#sub LoadCursor
function# [cursor] ;;cursor: 1 move, 2 copy, 3 no, 4 red cross, 5 blue cross, 6 link

 Loads one of often used cursors that cannot be loaded with LoadCursor(0 ...).
 Don't need to destroy.

int hmr
#if EXE
#exe addfile "" 211 RT_GROUP_CURSOR
#exe addfile "" 373 RT_GROUP_CURSOR
hmr=GetExeResHandle
#else
hmr=_hinst
#endif

sel cursor
	case 4
	int-- c4=LoadCursor(hmr +211)
	ret c4
	
	case 5
	int-- c5=LoadCursor(hmr +373)
	ret c5
	
	case [1,2,3,6]
	int-- hm(GetModuleHandle("ole32")) c1(LoadCursor(hm +2)) c2(LoadCursor(hm +3)) c3(LoadCursor(hm +1)) c6(LoadCursor(hm +4))
	sel(cursor) case 1 ret c1;  case 2 ret c2;  case 3 ret c3;  case 6 ret c6


#sub CB_InitVar
function str&cbVar selItem [$items]

 Formats cbVar to be passed to ShowDialog as combo box variable.
 Inserts & depending on selItem.

 cbVar - combo box variable. Should not have lines that begin with &.
 selItem - 0-based item index.
 items - if used, adds to cbVar (then cbVar can be empty).


if(!empty(items)) cbVar.addline(items)
if(selItem<0) ret
selItem=findl(cbVar selItem); if(selItem<0) ret
cbVar.insert("&" selItem)


#sub CheckboxPushlikeNoTheme
function hDlg
 Removes theme from all push-like checkboxes.

ARRAY(int) a; int i j
child "" "Button" hDlg 16 0 0 a
for(i 0 a.len)
	j=GetWinStyle(a[i])
	if(j&BS_PUSHLIKE) sel(j&BS_TYPEMASK) case [BS_AUTOCHECKBOX,BS_AUTORADIOBUTTON,BS_CHECKBOX,BS_RADIOBUTTON] TO_NoTheme a[i]


#sub TvGetParam
function# tv hItem

if(!hItem) ret
TVITEMW ti
ti.mask = TVIF_PARAM; ti.hItem = hItem
SendMessage(tv TVM_GETITEMW 0 &ti)
ret ti.lParam


#sub FlagsDialog
function! int&flags $flagsCSV [hwndParent] [$title] [$help] ;;flagsCSV: "flag,text[]flag,text[]..."

 Input dialog for flags.
 Returns 1 on OK, 0 on Cancel or incorrect argument.

 flags - recaives flags. Also sets initial check box states. The function changes only flags that are specified in flagsCSV.
 flagsCSV - 2-column CSV. Flag value and flag text.
 help - macro containing reference, or a QM function etc. If used, adds [?] button that shows reference in Tips pane or in QM Help.


str controls = "3"
str qmg3x

int mask=sub.FlagsToGridCsv(flags flagsCSV qmg3x); if(!mask) ret

if(empty(title)) title="Flags"
_i=iif(empty(help) 4 5)
str dd=
F
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "{title}"
 3 QM_Grid 0x56035041 0x200 0 0 224 112 "0x37,0,0,4,0x10008000[],,,"
 1 Button 0x54030001 0x4 4 118 48 14 "OK"
 2 Button 0x54030000 0x4 56 118 48 14 "Cancel"
 4 Button 0x{_i}4032000 0x0 108 118 18 14 "?"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" ""

if(!ShowDialog(dd &sub.DlgProc_FlagsDialog &controls hwndParent 0 0 0 +help)) ret

sub.FlagsFromGridCsv(qmg3x flags mask)

ret 1


#sub DlgProc_FlagsDialog p
function# hDlg message wParam lParam
sel message
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	lpstr help=+DT_GetParam(hDlg)
	if(!empty(help)) QmHelp help
ret 1


#sub FlagsToGridCsv
function# flags $csvIn str&csvOut ;;csvIn: "flag,text[]flag,text[]..."

 Creates CSV (csvOut) for a flags grid control.
 Returns mask of flags specified in csvIn, or 0 if incorrect csvIn.
 The control must have style "0x37,0,0,4,0x10008000[],,,".
 Later use sub_to.FlagsFromGridCsv to get selected flags.

 flags - initial flags, used to check checkboxes.


ICsv c._create
c.FromString(csvIn); err
if(_hresult or c.ColumnCount<2) ret

int i f on mask
for i 0 c.RowCount
	f=c.CellInt(i 0)
	on=f and flags&f=f
	c.Cell(i 1)=F"</{f}///{on+1}>{c.Cell(i 1)}"
	mask|f

c.RemoveColumn(0)
c.ToString(csvOut)

ret mask


#sub FlagsFromGridCsv
function# $gridCsv int&flags mask

 Parses flags-grid control CSV and returns flags.

 gridCsv - CSV of checked items. The whole CSV should be created by sub_to.FlagsToGridCsv.
 flags - receives flags. The function changes only flags that are specified in gridCSV.
 mask - all flags that were specified in the initial CSV. Normally it is what sub_to.FlagsToGridCsv returned. Will not remove other flags.


ICsv c._create
int i

flags~mask
c.FromString(gridCsv)
for i 0 c.RowCount
	flags|val(c.Cell(i 0)+2)

ret flags


#sub FlagsMaskDialog
function! int&flags int&mask $flagsCSV [hwndParent] [$title] [$help] ;;flagsCSV: "flag,text[]flag,text[]..."

 Input dialog for flags and mask. For example for style or state.
 Returns 1 on OK, 0 on Cancel or incorrect argument.

 flags, mask - recaives flags and mask. Initial values will be shown in grid.
 flagsCSV - see <help>sub_to.FlagsMaskToGridCsv</help>.
 help - macro containing reference, or a QM function etc. If used, adds [?] button that shows reference in Tips pane or in QM Help.

 Changes only flags/mask that are specified in flagsCSV.


str controls = "3"
str qmg3x

if(!sub.FlagsMaskToGridCsv(flags mask flagsCSV qmg3x)) ret

if(empty(title)) title="Flags"
_i=iif(empty(help) 4 5) 
str dd=
F
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 212 "{title}"
 3 QM_Grid 0x56039041 0x200 0 0 224 188 "0x7,0,0,0,0x10000000[]Flag,72%,,[]On,10%,2,[]Use,10%,2,"
 1 Button 0x54030001 0x4 4 194 48 14 "OK"
 2 Button 0x54030000 0x4 56 194 48 14 "Cancel"
 4 Button 0x0 0x0 108 194 18 14 "?"
 END DIALOG
 DIALOG EDITOR: "" 0x2030406 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc_FlagsMaskDialog &controls hwndParent 0 0 0 +help)) ret

sub.FlagsMaskFromGridCsv(qmg3x flags mask)

ret 1


#sub DlgProc_FlagsMaskDialog p
function# hDlg message wParam lParam
sel message
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	lpstr help=+DT_GetParam(hDlg)
	if(!empty(help)) QmHelp help
ret 1


#sub FlagsMaskToGridCsv p
function# flags mask $csvIn str&csvOut

 Creates CSV (csvOut) for a flags/mask grid control.
 Returns all flags specified in csvIn, or 0 if incorrect csvIn.
 The control must have style "0x7,0,0,0,0x10000000[]Flag,72%,,[]On,10%,2,[]Use,10%,2,".
 Later use sub.FlagsMaskFromGridCsv to get flags/mask.

 csvIn - can be:
   2-col CSV containing flags and names, like "flag,text[]flag,text[]...".
   1-col CSV containing names. First row is for flag 1, and so on.
 flags, mask - initial flags and mask, used to check cells in columns 1 and 2.


ICsv c._create cc._create
c.FromString(csvIn); err ret

int i f m nc(c.ColumnCount) nr(c.RowCount)
sel(nc) case [1,2] case else ret

for i 0 nr
	lpstr s sf sm
	if(nc=1) f=1<<i; s=c.Cell(i 0)
	else f=c.CellInt(i 0); s=c.Cell(i 1)
	m|f
	sf=iif(flags&f "Yes" ""); sm=iif(mask&f "Yes" "")
	cc.AddRowMS(-1 3 F"</{f}>{s}{0%%c}{sf}{0%%c}{sm}{0%%c}")

cc.ToString(csvOut)

ret m


#sub FlagsMaskFromGridCsv p
function $gridCSV int&flags int&mask

 Parses flags/mask grid control CSV and gets flags and mask.

 gridCsv - grid variable.
 flags, mask - receive flags and mask.

 Changes only flags/mask that are specified in gridCSV.


ICsv c._create
int i f

c.FromString(gridCSV)
for i 0 c.RowCount
	f=val(c.Cell(i 0)+2)
	if(!empty(c.Cell(i 1))) flags|f; else flags~f
	if(!empty(c.Cell(i 2))) mask|f; else mask~f


#sub FormatLinkToSelectFileInExplorer
function~ $file_ [$linkText]

 Formats QM output link that opens parent folder and selects the file.
 If linkText empty, uses file_.

if(empty(linkText)) linkText=file_
str sr se
if(_winver<0x600 and rget(sr "" "Folder\shell" HKEY_CLASSES_ROOT) and sr~"explore") se="/e,"
ret F"<link ''explorer.exe /{se}/select,{_s.expandpath(file_)}''>{linkText}</link>"


#sub WaitForShift
function $osdTooltipText

 Shows OSD tooltip and waits for Shift key.
 Can be used eg to capture something from screen (UI object, image) with Shift key.

opt waitmsg 1 ;;caller may be dialog etc
sub_sys.TooltipOsd osdTooltipText 4|8 "WaitForShift" -1
int i
for i 0 1000000
	0.003; ifk(S) break
	if(i=20000) i=0; sub_sys.TooltipOsd "Waiting for Shift..." 4 "WaitForShift" 0 0 0 0 0xff
OsdHide "WaitForShift"
0.1
