 /Dialog_Editor

int h=child(mouse); if(!h) ret
___DE_CLIPBOARD x
 g1
x.style=GetWinStyle(h)
x.cls.getwinclass(h)
if(!sub.GetBaseClass(h x.cls x.style)) ret

sel x.cls 1
	case ["Edit","ComboLBox"] ;;is combo child?
	int hp=GetParent(h); str clsp
	if(sub.GetBaseClass(hp clsp _i) and clsp~"ComboBox") h=hp; goto g1

GetWinXY h x.x x.y x.wid x.hei GetAncestor(h 2)
x.exstyle=GetWinStyle(h 1)
if(__CDD_CanHaveText(x.cls x.style)>0) x.txt.getwintext(h)
x.c.txt=x.txt

sel x.cls 1
	case "Button"
	if x.style&BS_TYPEMASK=BS_OWNERDRAW ;;winforms Button and Static are ownerdraw
		x.style~BS_TYPEMASK
		Acc ab.FromWindow(h OBJID_CLIENT); err
		str sr; ab.Role(sr); err
		sel(sr) case "CHECKBUTTON" x.style|BS_AUTOCHECKBOX; case "RADIOBUTTON" x.style|BS_AUTORADIOBUTTON
	case "Static" if(x.style&SS_TYPEMASK=SS_OWNERDRAW) x.style~SS_TYPEMASK
	case "ListBox" x.style|LBS_NOINTEGRALHEIGHT

 resize form if the control is outside
RECT r; GetClientRect _hform &r
int resize cRight(x.x+iif(x.wid<=400 x.wid 400)) cBottom(x.y+iif(x.hei<=200 x.hei 200))
if(r.right<cRight) r.right=cRight; resize=1
if(r.bottom<cBottom) r.bottom=cBottom; resize=1

if(!_AddControl(-1 0 x)) ret

if resize ;;this is after, to combine Undo
	TO_AdjustWindowRect r 0 0 _hform
	SetWindowPos _hform 0 0 0 r.right-r.left r.bottom-r.top SWP_NOMOVE|SWP_NOZORDER
	subs.AutoSizeEditor

err+


#sub GetBaseClass
function! h str&cls int&style

 out SendMessage(h WM_GETOBJECT 0 OBJID_QUERYCLASSNAMEIDX)-65536
 out _s.GetWinBaseClass(h)

WNDCLASSEXW w.cbSize=sizeof(w)
if(GetClassInfoExW(0 @cls &w) or GetClassInfoExW(_hinst @cls &w)) ret 1

int i=SendMessage(h WM_GETOBJECT 0 OBJID_QUERYCLASSNAMEIDX)-65536
__MapIntStr m.AddList("0 ListBox[]2 Button[]3 Static[]4 Edit[]5 ComboBox[]10 ScrollBar[]11 msctls_statusbar32[]12 ToolbarWindow32[]13 msctls_progress32[]14 SysAnimate32[]15 SysTabControl32[]16 msctls_hotkey32[]17 SysHeader32[]18 msctls_trackbar32[]19 SysListView32[]22 msctls_updown32[]25 SysTreeView32[]28 RichEdit20A[]30 RichEdit20W") ;;30 undocumented, returns winforms
i=m.FindInt(i)
if(i>=0) cls=m.a[i].s; ret 1

_s.GetWinBaseClass(h) ;;see note in to_controls
if(!(_s~cls)) cls=_s; ret 1

str s ss
ss="ComboBoxEx32[]ReBarWindow32[]SysDateTimePick32[]SysIPAddress32[]SysMonthCal32[]SysPager[]SysLink[]RichEdit"
foreach(s ss) if(find(cls s 0 1)>=0) cls=s; ret 1

for(i 0 m.a.len) if(find(cls m.a[i].s 0 1)>=0) cls=m.a[i].s; ret 1

Acc ab.FromWindow(h OBJID_CLIENT)
str sr; ab.Role(sr)
 out sr ;;todo: more
sel(sr)
	case "TOOLBAR" cls="ToolbarWindow32"
	case "STATUSBAR" cls="msctls_statusbar32"
	case "SPINBUTTON" cls="msctls_updown32"
	case "GROUPING" cls="Button"; style~BS_TYPEMASK; style|BS_GROUPBOX
	case else ret
ret 1

err+
