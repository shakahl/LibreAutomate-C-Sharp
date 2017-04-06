 __sub_sys contains shared subs used anywhere in \System.
 __sub_to contains shared subs used in \System\Tools.

#sub TooltipOsd
function $text [flags] [$osdId] [^timeS] [x] [y] [hwndBy] [textColor] ;;flags: 1 below hwndBy (default right), 2 synchronous, 4 bigger text, 8 hide when macro ends, 16 use raw x y, 32 place by the mouse, 0x100 bold, 0x200 italic

 Shows a tooltip using OnScreenDisplay.

 hwndBy - a window or control handle. If used, x y are relative to its top-right corner. If flag 1, to its bottom-left corner.
 Other parameters are same as with <help>OnScreenDisplay</help>.
 You can use function OsdHide to hide (destroy) the tooltip.

if hwndBy
	RECT r; GetWindowRect hwndBy &r
	if(flags&1) x+r.left; y+r.bottom
	else x+r.right; y+r.top
	flags|16
else if(flags&32) x+16

ret OnScreenDisplay(text timeS x y "" iif(flags&4 12 9) iif(textColor textColor 1) flags|5 osdId GetSysColor(COLOR_INFOBK))


#sub MsgBox
function# hwndOwner ~text [$caption] [$style] ;;style: "[O|OC|YN|YNC|ARI|RC|CTE] [1|2|3] [?|!|x|i|q|v] [s] [a|n] [t]"

MES m.hwndowner=hwndOwner; m.style=style
ret mes(text caption m)


#sub FileCopyMove_Prepare
function $from $to flags str&s1 str&s2
 Used by FileCopy and FileMove.

GetFullPath from &s1
GetFullPath to &s2

if(flags&2=0 and FileExists(s2 1)) ;;if s2 folder, append filename
	s2.rtrim("\")
	_s.getfilename(s1 1)
	s2.from(s2 "\" _s)

if(flags&1=0 and FileExists(s2 2)) FileDelete s2; err end _error


#sub FileType_GetClass
function! lpstr&ext str&cls

if(ext[0]='.') ext+1
if(StrCompare(ext "*")=0) cls=ext; ret 1
if(!rget(cls "" _s.from("." ext) HKEY_CLASSES_ROOT)) ret
ret 1


#sub CBLB_SelectedItem
function# hwnd [ctrlType] [str&itemText] ;;ctrlType: 0 combobox, 1 listbox.

int m; sel(ctrlType) case 0 m=CB_GETCURSEL; case 1 m=LB_GETCURSEL
int i=SendMessage(hwnd m 0 0)
if(&itemText and i>=0) sub.CBLB_GetItemText hwnd i itemText ctrlType
ret i


#sub CBLB_SelectItem
function# hwnd itemIndex [ctrlType] [hwndnotify] ;;ctrlType: 0 combobox, 1 listbox.

int R=-1

sel ctrlType
	case 0
	R=SendMessage(hwnd CB_SETCURSEL itemIndex 0)
	if(hwndnotify=0) hwndnotify=GetParent(hwnd)
	int cid=GetDlgCtrlID(hwnd)
	SendMessage(hwndnotify WM_COMMAND CBN_SELENDOK<<16|cid hwnd)
	SendMessage(hwndnotify WM_COMMAND CBN_SELCHANGE<<16|cid hwnd)
	
	case 1 R=SendMessage(hwnd LB_SETCURSEL itemIndex 0)

ret R


#sub CBLB_FindItem
function# hwnd $itemText [ctrlType] [startFrom] [exact] ;;ctrlType: 0 combobox, 1 listbox.

if(itemText=0 or hwnd=0) ret -1
int m
sel ctrlType
	case 0 m=iif(exact CB_FINDSTRINGEXACT CB_FINDSTRING)
	case 1 m=iif(exact LB_FINDSTRINGEXACT LB_FINDSTRING)

ret SendMessageW(hwnd m startFrom-1 @itemText)


#sub CBLB_GetItemText
function! hwnd itemIndex str&itemText [ctrlType] ;;ctrlType: 0 combobox, 1 listbox.

itemText.fix(0)
int m; sel(ctrlType) case 0 m=CB_GETLBTEXT; case 1 m=LB_GETTEXT
int tl=SendMessageW(hwnd m+1 itemIndex 0); if(tl<0) ret
if tl
	BSTR w.alloc(tl+100&~31)
	tl=SendMessageW(hwnd m itemIndex w.pstr); if(tl<0) ret
	itemText.ansi(w _unicode tl)
ret 1


#sub WinFromStr
function# ~s

if(!s.len) ret win
if(s[0]='+') ret win("" s+1)
ret win(s)


#sub QueryService
function'IUnknown IUnknown'iFrom GUID&iid [GUID&guidService]

IUnknown u
if(!&guidService) &guidService=iid
IServiceProvider sp=+iFrom
sp.QueryService(guidService iid &u); err u=0

err+
ret u


#sub PortableWarning
if _portable
	mes "Portable QM warning: This will make changes in registry or file system." "" "!"


#sub ExeQmPlusDll
function!

 Use for qmplus.dll like ExeQmGridDll for qmgrid.dll.

#if EXE=1
#exe addfile "$qm$\qmplus.dll" 21089
int+ ___eqpd
if !___eqpd
	lock
	if !___eqpd
		if !GetModuleHandle("qmplus.dll")
			_s.expandpath(F"$temp qm$\ver 0x{QMVER}\qmplus.dll")
			if !FileExists(_s)
				if(!ExeExtractFile(21089 _s)) ret ;;also creates folders.
			if(!LoadLibraryW(@_s)) ret
		___eqpd=1
#endif
ret 1


#sub Font_ParseControls
function hDlg $controls ARRAY(int)&a
 Gets handles.

int i v h
if(!empty(controls))
	rep
		v=val(controls+i)
		if(!v) h=hDlg; else h=id(v hDlg)
		if(h) a[]=h
		i=findcs(controls " -" i)+1; if(i=0) break
		if(controls[i-1]='-')
			for(v v+1 val(controls+i)+1)
				h=id(v hDlg)
				if(h) a[]=h
			i=findc(controls 32 i)+1; if(i=0) break
else
	h=GetWindow(hDlg GW_CHILD)
	rep
		if(!h) break
		a[]=h
		h=GetWindow(h GW_HWNDNEXT)


#sub RKK_Remap
function! ICsv&c

ARRAY(int) a.create(3)
int i k1 k2; str s rk="SYSTEM\CurrentControlSet\Control\Keyboard Layout"

for i 0 c.RowCount
	k1=c.CellInt(i 0); if(!k1) continue
	k2=c.CellInt(i 1)
	a[]=k1<<16|(k2&0xffff)
a[]=0

if a.len>4
	a[2]=a.len-3
	s.fromn(&a[0] a.len*4)
	i=rset(s "Scancode Map" rk HKEY_LOCAL_MACHINE REG_BINARY)
else
	i=rset("" "Scancode Map" rk HKEY_LOCAL_MACHINE -1)
	if(!i and GetLastError=ERROR_FILE_NOT_FOUND) i=1

ret i!0


#sub GetWindowsStoreAppId
function! hwnd str&appID [flags] ;;flags: 1 prepend "shell:AppsFolder\" (to run or get icon), 2 get exe full path if hwnd is not a store app

 Gets Windows store app user model id, like "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App".
 Returns 1 if gets user model id, 2 if gets path, 0 if fails.

if flags&2
	int isApp
	if _winver>=602
		sel WinTest(hwnd "Windows.UI.Core.CoreWindow[]ApplicationFrameWindow")
			case 1 isApp=1
			case 2 if(_winver>=0xA00) _i=sub.GetWindowsStoreAppFrameChild(hwnd); if(_i) hwnd=_i; isApp=1
	if(!isApp) appID.getwinexe(hwnd 1); ret 2
else if(_winver<0x602) ret

__HProcess hp; if(!hp.Open(hwnd PROCESS_QUERY_LIMITED_INFORMATION)) ret
_s.all(1000); _i=1000
if(GetApplicationUserModelId(hp &_i +_s)) ret
appID.ansi(_s)
if(flags&1) appID-"shell:AppsFolder\"
ret 1
err+


#sub GetWindowsStoreAppFrameChild
function# hwnd

 On Win10+, if hwnd is "ApplicationFrameWindow", returns the real app window "Windows.UI.Core.CoreWindow" hosted by hwnd.
 If hwnd is minimized, cloaked (eg on other desktop) or the app is starting, the "Windows.UI.Core.CoreWindow" is not its child. Then searches for a top-level window with the same name as of hwnd. It is unreliable, but MS does not provide API for this.
 Info: "Windows.UI.Core.CoreWindow" windows hosted by "ApplicationFrameWindow" belong to separate processes. All "ApplicationFrameWindow" windows belong to a single process.

 g1
if(_winver<0xA00 or !WinTest(hwnd "ApplicationFrameWindow")) ret
int c=FindWindowExW(hwnd 0 L"Windows.UI.Core.CoreWindow" 0)
if(c) ret c
int retry; if(retry) ret

_s.getwintext(hwnd); if(!_s.len) ret
BSTR b=_s

rep
	c=FindWindowExW(0 c L"Windows.UI.Core.CoreWindow" b) ;;I could not find API for it
	if(!c) break
	if(IsWindowCloaked(c)) ret c ;;else probably it is an unrelated window

retry=1; goto g1 ;;maybe SetParent called while we searched for top-level window etc, eg when starting the app or switching Win10 desktops

err+


#sub GetWindowsStoreAppHost
function# hwnd

 See GetWindowsStoreAppFrameChild. This func does vice versa.

if(_winver<0xA00 or !WinTest(hwnd "Windows.UI.Core.CoreWindow")) ret
int o=GetParent(hwnd); if(o and WinTest(o "ApplicationFrameWindow")) ret o
_s.getwintext(hwnd); if(!_s.len) ret
ret win(_s "ApplicationFrameWindow")

err+
