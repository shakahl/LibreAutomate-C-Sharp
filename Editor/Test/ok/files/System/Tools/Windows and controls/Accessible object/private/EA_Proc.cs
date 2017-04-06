 /
function hDlg [onlypar]

___EA- dA
int hwnd=dA.hwnd
if(!hwnd or dA.working) ret

EA_Smallest 0 hDlg

___EA_ENUM e
e.tv=id(3 hDlg)
int flags=0x8000|(but(7 hDlg)*32)|(but(5 hDlg)*16)
dA.isTreePlusInvisible=flags&16!0

 EnableWindow hDlg 0 ;;the user could not close
 SetCursor LoadCursor(0 +IDC_WAIT) ;;0 in EA_Proc2 would restore
hid e.tv
dA.isIgnoreTreeWmNotify=1
SendMessage e.tv TVM_SELECTITEM TVGN_CARET 0
SendMessage e.tv TVM_DELETEITEM 0 0
dA.isIgnoreTreeWmNotify=0

Acc ar; int onlyWeb
if(but(63 hDlg) and EA_IsVisibleDoc(dA.ai 0 ar)) onlyWeb=1; else ar=acc(hwnd)
dA.ar.create(1)
dA.ar[0].a=ar
dA.ar[0].htvi=TvAdd(e.tv 0 +LPSTR_TEXTCALLBACK 0)

if !onlypar
	_s="Stop"; _s.setwintext(id(8 hDlg))
	dA.working=3
	 enum all
	if onlyWeb
		acc("" "" ar "" "" flags &EA_Proc2 &e)
	else
		acc("" "" hwnd "" "" flags &EA_Proc2 &e)
		if(dA.working&1) sub.JavaInfo hDlg hwnd dA.ai
	dA.working=0
	 select
	if(EA_Select(hDlg dA.ai !onlyWeb flags)<0) SendMessage e.tv TVM_EXPAND 2 dA.ar[0].htvi

err+
dA.working=0
hid- e.tv
 EnableWindow hDlg 1
_s="Refresh"; _s.setwintext(id(8 hDlg))


#sub JavaInfo
function hDlg hwnd Acc&a

 If hwnd is a Java window, but JAB not installed/enabled, informs the user to do it.
 Without this, nobody would use the QM JAB support.

 is JAB installed/enabled?
int+ ___is_jab ;;0 not tested, 1 is, 2 disabled, 3 not installed, 4 installed only 64-bit
if !___is_jab
	if !FileExists(iif(_win64 "$system$\WindowsAccessBridge-32.dll" "$system$\WindowsAccessBridge.dll"))
		___is_jab=3+(_win64 and FileExists("$Windows$\Sysnative\WindowsAccessBridge.dll"))
	else
		str sf sd; int i
		for i 0 2
			if(!i) sf="$User Profile$\.accessibility.properties"
			else if(TO_JavaGetDir(sf)) sf+"\lib\accessibility.properties"
			else break
			sd.getfile(sf); err sd.all
			___is_jab=1+(findrx(sd "(?m)^\s*assistive_technologies=")<0)
			if(___is_jab=1) break
if(___is_jab=1) ret

 is Java window?
a.Role(_s); if(_s!"CLIENT") ret
if(hwnd!a.Hwnd) ret
_s=a.a.Help(0); err
if(_s="QM Java object") ___is_jab=1; ret

sel _s.getwinexe(hwnd) 1
	case ["javaw","java","SOFFICE.BIN"] ;;I don't know a better way. Could look for loaded java modules, but: cannot see 64-bit; not all java apps use them (eg OO).
	if ___is_jab=2
		_s="<macro ''JavaEnableJAB''>enable Java Access Bridge</macro> and restart Java applications."
	else _s="install Java 7.6 or later, 32-bit."
	EA_Info hDlg F"<>Accessible objects in this window are unavailable. Need to {_s} <help #IDP_ACCESSIBLE>Read more.</help>"
 
err+
