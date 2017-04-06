out

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 414 294 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0 _hwndqm)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_PAINT
	PAINTSTRUCT ps; int dc=BeginPaint(hDlg &ps)
	sub.Icon(dc)
	EndPaint hDlg &ps
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1


#sub Icon
function dc
__Hicon hi1 hi2 hi3 hi4 hi5 hi6 hi7 hi8_1 hi8_2 hi9 hi10 hi11 hi12
str s; int index
 s="q:\app\target.ico"
 s="q:\app\function.ico"
 s="q:\app\qm.ico"
 s="q:\app\qm.exe"; index=-133
 SetEnvVar "ap" "q:\app"; s="%ap%\qm.exe"; index=1
 s="q:\app\app.cpp" ;;with Shell_GetCachedImageIndex we get blank document icon
s.expandpath("$program files$\PicPick\picpick.exe")
int z=24

 int w=win("Document1 - Microsoft Word" "OpusApp")
 int t=SendMessage(w WM_GETICON 0 0)
 out t
  __Hicon hi=CopyIcon(t)
 __Hicon hi=CopyImage(t IMAGE_ICON 20 20 0)
 out hi
 DrawIconEx(dc 160 0 hi 0 0 0 0 DI_NORMAL)
 ret

int hi=LoadImage(_hinst +133 IMAGE_ICON 16 16 LR_SHARED)
out hi
out DestroyIcon(hi)
DrawIconEx(dc 160 0 hi 0 0 0 0 DI_NORMAL)
ret


hi1=sub.TryWithShield("Q:\Downloads\LLVM-3.8.0-win64.exe")
DrawIconEx(dc 160 0 hi1 0 0 0 0 DI_NORMAL)
ret

int i x y
ARRAY(str) a; GetFilesInFolder(a "q:\app")
ARRAY(__Hicon) ai.create(a.len)

long t0=perf
for i 0 a.len
	str& path=a[i]
	out path
	PF
	ai[i]=sub.GetShellIcon(path 3)
	 ai[i]=sub.GetShellIcon_Map(path)
	PN;PO
long t1=perf
out t1-t0

for i 0 ai.len
	DrawIconEx(dc x y ai[i] 0 0 0 0 DI_NORMAL)
	x+20; if(x>=600) x=0; y+20
ret

 hi1=LoadImageW(0 @s IMAGE_ICON z z LR_LOADFROMFILE)
 DrawIconEx(dc 0 0 hi1 0 0 0 0 DI_NORMAL)
 
 PrivateExtractIconsW(@s 0 z z &hi2 0 1 0)
 DrawIconEx(dc 40 0 hi2 0 0 0 0 DI_NORMAL)
 
 hi3=LoadImageW(0 @s IMAGE_ICON 32 32 LR_LOADFROMFILE)
 hi3=CopyImage(hi3 IMAGE_ICON z z LR_COPYDELETEORG)
 DrawIconEx(dc 80 0 hi3 0 0 0 0 DI_NORMAL)
 
 PrivateExtractIconsW(@s 0 32 32 &hi4 0 1 0)
 hi4=CopyImage(hi4 IMAGE_ICON z z LR_COPYDELETEORG)
 DrawIconEx(dc 120 0 hi4 0 0 0 0 DI_NORMAL)

PF
hi5=sub.GetShellIcon(s)
PN;PO
DrawIconEx(dc 160 0 hi5 0 0 0 0 DI_NORMAL)
ret
 
 hi6=LoadImageW(0 @s IMAGE_ICON 32 32 LR_LOADFROMFILE)
 hi6=sub.ResizeIcon(hi6 z)
 DrawIconEx(dc 200 0 hi6 0 0 0 0 DI_NORMAL)
 
 hi7=LoadImageW(0 @s IMAGE_ICON 32 32 LR_LOADFROMFILE)
 PF
 hi7=sub.ResizeIcon2(hi7 z)
 PN;PO
 DrawIconEx(dc 240 0 hi7 0 0 0 0 DI_NORMAL)
 
 _i=MakeInt(32 z)
 PrivateExtractIconsW(@s 0 _i _i &hi8_1 0 2 0)
 DrawIconEx(dc 0 40 hi8_1 0 0 0 0 DI_NORMAL)
 DrawIconEx(dc 40 40 hi8_2 0 0 0 0 DI_NORMAL)
 
  s="c:\app\app.cpp"
 
 hi9=sub.GetShellIcon2(s z)
 DrawIconEx(dc 0 80 hi9 0 0 0 0 DI_NORMAL)
 
 PF
hi10=sub.GetShellIcon3(s index z)
 PN;PO
DrawIconEx(dc 40 80 hi10 0 0 0 0 DI_NORMAL)

 hi11=LoadImageW(0 @s IMAGE_ICON 32 32 LR_LOADFROMFILE)
 PF
 hi11=sub.ResizeIcon4(hi11 z)
 PN;PO
 DrawIconEx(dc 0 120 hi11 0 0 0 0 DI_NORMAL)

 PF
 hi12=sub.GetFileIconWithCache(s index 0)
 PN;PO
  out hi12
 DrawIconEx(dc 0 160 hi12 0 0 0 0 DI_NORMAL)


#sub GetFileIconWithCache
function# $s [index] [size]

int R
BSTR b=s

if size>0
	int hr=SHDefExtractIconW(b 0 0 &R 0 size)
	ret R

int iil
sel size
	case 0 iil=SHIL_SMALL
	case -1 iil=SHIL_LARGE
	case -2 iil=SHIL_EXTRALARGE
	case -3 iil=SHIL_JUMBO
	case else ret

IImageList il
if(SHGetImageList(iil IID_IImageList &il)) ret
int i=Shell_GetCachedImageIndex(b index 0)
 out i
PN
il.GetIcon(i 0 &R) ;;very slow first time, because extracts icon. Then quite fast, but much slower than CopyImage.

ret R
 
 
 #sub GetFileIconWithCache
 function# $s [index] [size]
 
 int R
 BSTR b=s
 
 if size>0
	 int hr=SHDefExtractIconW(b 0 0 &R 0 size)
	 ret R
 
 int iil
 sel size
	 case 0 iil=SHIL_SMALL
	 case -1 iil=SHIL_LARGE
	 case -2 iil=SHIL_EXTRALARGE
	 case -3 iil=SHIL_JUMBO
	 case else ret
 
 IImageList il
 if(SHGetImageList(iil IID_IImageList &il)) ret
 int n; il.GetImageCount(&n)
 out n
 
 int i=Shell_GetCachedImageIndex(b index 0)
 PN
 if i>=n ;;added new index
	 out "new"
	 il.GetIconSize(&size &size)
	 hr=SHDefExtractIconW(b 0 0 &R 0 size)
	 if(!hr) il.ReplaceIcon(i R &i)
 else
	 out "cached"
	 il.GetIcon(i 0 &R)
 
 ret R


#sub GetShellIcon
function# $s [flags] ;;flags: 1 use PIDL, use SHGetImageList

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
if flags&2
	IImageList k
	if(!SHGetImageList(SHIL_SMALL IID_IImageList &k))
		int R
		k.GetIcon(x.iIcon 0 &R)
		ret R
else
	ret ImageList_GetIcon(il, x.iIcon, 0)


#sub GetShellIcon_Map
function# $s

int R
ITEMIDLIST* pidl
if(SHParseDisplayName(@s 0 &pidl 0 0)) ret
IShellFolder folder; ITEMIDLIST* pidlItem
if(!SHBindToParent(pidl IID_IShellFolder &folder &pidlItem))
	PN
	int ii=SHMapPIDLToSystemImageListIndex(folder pidlItem 0)
	PN
	if(ii>=0)
		IImageList il
		if(!SHGetImageList(SHIL_SMALL IID_IImageList &il))
			il.GetIcon(ii 0 &R)

CoTaskMemFree pidl
ret R


#sub GetShellIcon2
function# $sf size

dll shell32 #SHExtractIconsW @*pszIconFile iIndex cx cy *phicon *pRI n flags

int hi
if(!SHExtractIconsW(@sf 0 size size &hi 0 1 0)) ret
ret hi


#sub GetShellIcon3
function# $sf index size ;;flags: 1 large

 ret GetFileIcon(sf 0 flags|8)

int hr fl
hr=SHDefExtractIconW(@sf index fl &_i 0 size)
ret _i


#sub TryWithShield
function# $s

 SHGetFileInfo does not get icon with shield overlay. Only with link overlay (SHGFI_LINKOVERLAY).
 Don't know how to get it, maybe with IExtractIcon.

int f=SHGFI_SMALLICON|SHGFI_SHELLICONSIZE;;|SHGFI_SYSICONINDEX
f|SHGFI_ICON
f|SHGFI_ADDOVERLAYS
 f|SHGFI_LINKOVERLAY
 f|SHGFI_OPENICON
 f|SHGFI_SELECTED
SHFILEINFOW x
int il=SHGetFileInfoW(@s FILE_ATTRIBUTE_NORMAL &x sizeof(x) f)
ret x.hIcon


#sub ResizeIcon
function# hi size

__ImageList il.Create("" size)
ImageList_ReplaceIcon(il -1 hi)
DestroyIcon(hi)
ret ImageList_GetIcon(il 0 0)


#sub ResizeIcon
function# hi size

__ImageList il.Create("" size)
ImageList_ReplaceIcon(il -1 hi)
DestroyIcon(hi)
ret ImageList_GetIcon(il 0 0)


#sub ResizeIcon2
function# hi size

__ImageList g_il
if(!g_il) g_il.Create("" size)
ImageList_ReplaceIcon(g_il -1 hi)
DestroyIcon(hi)
ret ImageList_GetIcon(g_il 0 0)


#sub ResizeIcon3 ;;does not work
function# hi size

__MemBmp m.Create(size size)
DrawIconEx(m.dc 0 0 hi size size 0 0 DI_NORMAL)
DestroyIcon(hi)
ret CopyImage(m.bm IMAGE_ICON size size 0)


#sub ResizeIcon4
function# hi size

ret CsFunc("macro:Dialog196" hi size)



#ret
using System;
using System.Drawing;

public class C
{
public static IntPtr ResizeIcon(int hi, int size)
{
Bitmap bitmap = new Bitmap(size, size);

using (Graphics g = Graphics.FromImage(bitmap))
{
    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
    g.DrawImage(Icon.FromHandle((IntPtr)hi).ToBitmap(), new Rectangle(0, 0, size, size));
}

return bitmap.GetHicon();
}
}
