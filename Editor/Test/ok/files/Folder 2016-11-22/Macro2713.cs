 /exe
out
 mes 1
 1
int w=win("Process Monitor" "PROCMON_WINDOW_CLASS")
 w=0

str ss=
 Q:\Test\test.cs

 .sst

 .chk

 .asp

 c:\windows\Boot\DVD\PCAT\etfsboot.com

 .as
 .txt
 .exe
 .msc

 http

 Q:\Test\test.cs

 q:\app\hhuser.hhp

 Q:\Test\a.txt

 Q:\Test\test.cs
 Q:\Test\a.txt

 Q:\Test

ARRAY(str) a=ss
int i
for i 0 a.len
	if w
		DestroyIcon sub.GetShellIcon("Q:\Test\a.txt" 3)
		
		0.5
		lef 133 13 w 1|4 ;;tool bar, push button 'Clear (Ctrl+X)', "~:2EBB33E0"
		1
	
	str s=a[i]
	
	mes "now will find icon"
	PF
	 int hi=GetFileIcon(s)
	 int hi=sub.GetIcon2(s)
	 int hi=sub.GetIcon3(s)
	int hi=sub.GetShellIcon(s 7)
	PN;PO
	out hi
	
	int dc=GetDC(0)
	DrawIconEx(dc 0 0 hi 16 16 0 0 DI_NORMAL)
	ReleaseDC 0 dc
	
	DestroyIcon hi
	
	 ITEMIDLIST* pidl
	 if(SHParseDisplayName(@s 0 &pidl 0 0)) out "error"
	

 ret
if(w) wait 0 MM
 mes 2

 BEGIN PROJECT
 main_function  Macro2713
 exe_file  $my qm$\Macro2713.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {D4906F43-2093-48F6-A6C1-2BB504872CBB}
 END PROJECT


#sub GetIcon
function# $s

ITEMIDLIST* pidl
if(SHParseDisplayName(@s 0 &pidl 0 0)) ret

int shFlags=SHGFI_SMALLICON|SHGFI_SYSICONINDEX|SHGFI_PIDL
SHFILEINFOW x
int il=SHGetFileInfoW(+pidl 0 &x sizeof(x) shFlags)
if(il=0) ret
ret ImageList_GetIcon(il, x.iIcon, 0)


#sub GetIcon2
function# $s

int shFlags=SHGFI_SMALLICON|SHGFI_SYSICONINDEX
if(s[0]='.') shFlags|SHGFI_USEFILEATTRIBUTES
SHFILEINFOW x
int il=SHGetFileInfoW(@s 0 &x sizeof(x) shFlags)
if(il=0) ret
ret ImageList_GetIcon(il, x.iIcon, 0)


#sub GetIcon3
function# $s

int n=1000
str icon.all(n)

int hr=AssocQueryString(0 ASSOCSTR_DEFAULTICON s 0 icon &n)
if(hr) _s.dllerror(s "" hr); out F"<><c 0xff>{_s}</c>"; ret
icon.fix(n)
out icon
PN
ret GetFileIcon(icon)


#sub GetIcon4
function# $s

int hr
IQueryAssociations q
if q=0
	GUID clsid; memcpy &clsid CLSID_QueryAssociations sizeof(GUID)
	hr=AssocCreate(clsid IID_IQueryAssociations &q)
	if(hr) end s 16 hr
	q.Init(0 @s 0 0)
PN
int n=1000
BSTR bicon.alloc(n)

q.GetString(0 ASSOCSTR_DEFAULTICON 0 bicon &n)
str icon.ansi(bicon)
out icon
PN
ret GetFileIcon(icon)


#sub GetIcon5
function# $s

int hr
IQueryAssociations q
if q=0
	GUID clsid; memcpy &clsid CLSID_QueryAssociations sizeof(GUID)
	hr=AssocCreate(clsid IID_IQueryAssociations &q)
	if(hr) end s 16 hr
	PN
	q.Init(0 @s 0 0)
PN
int k
q.GetKey(0 ASSOCKEY_CLASS 0 &k)
 out k

 int i n
 BSTR b.alloc(1000)
 for i 0 1000000000
	 n=1000
	 if(RegEnumKeyExW(k i b &n 0 0 0 0)) break
	 out _s.ansi(b)

 RegKey rk
 out rk.Open("shell" k KEY_READ)
 rk.Close

str icon
if(!rget(icon "" "DefaultIcon" k)) ret
 out icon

RegCloseKey k

PN
ret GetFileIcon(icon)


#sub GetIcon6
function# $s

str ext.GetFilenameExt(s); ext-"."
 out ext
str s1; if(!rget(s1 "" ext HKEY_CLASSES_ROOT)) ret
 out s1
str icon; if(!rget(icon "" F"{s1}\DefaultIcon" HKEY_CLASSES_ROOT)) ret
 out icon
 ret

 RegKey rk
 out rk.Open("shell" k KEY_READ)
 rk.Close

 str icon
 if(!rget(icon "" "DefaultIcon" k)) ret
  out icon
 
 RegCloseKey k
 
PN
ret GetFileIcon(icon)


#sub GetShellIcon
function# $s [flags] ;;flags: 1 use PIDL, 2 use SHGetImageList, 4 show msgbox

int shFlags=SHGFI_SMALLICON|SHGFI_SHELLICONSIZE|SHGFI_SYSICONINDEX
if(s[0]='.') shFlags|SHGFI_USEFILEATTRIBUTES; flags~1
SHFILEINFOW x
int il
if flags&1
	ITEMIDLIST* pidl
	if(SHParseDisplayName(@s 0 &pidl 0 0)) ret
	PN
	il=SHGetFileInfoW(+pidl 0 &x sizeof(x) shFlags|SHGFI_PIDL)
	CoTaskMemFree pidl
else
	il=SHGetFileInfoW(@s 0 &x sizeof(x) shFlags)
if(il=0) ret
PN
if(flags&4) mes "now will get icon from imagelist"
if flags&2
	IImageList k
	if(!SHGetImageList(SHIL_SMALL IID_IImageList &k))
		int R
		k.GetIcon(x.iIcon 0 &R)
		ret R
else
	ret ImageList_GetIcon(il, x.iIcon, 0)
