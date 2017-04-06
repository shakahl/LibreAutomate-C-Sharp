out
PF
WINAPI2.SHSTOCKICONINFO x.cbSize=sizeof(x)
int img=WINAPI2.SIID_APPLICATION
 int img=WINAPI2.SIID_DOCNOASSOC
int hr=WINAPI2.SHGetStockIconInfo(img, WINAPI2.SHGSI_ICONLOCATION &x) ;;always fast
if(hr) end "failed" 16 hr
PN
_s.ansi(&x.szPath)
int hi=GetFileIcon(_s x.iIcon)
PN
 int hi2=CopyIcon(hi)
 int hi2=DuplicateIcon(0 hi) ;;same as CopyIcon
int hi2=CopyImage(hi IMAGE_ICON 0 0 0) ;;3 times faster than CopyIcon
PN;PO

out _s
out x.iIcon
out hi
out hi2



__ImageList il.Create
PF
rep 5
	_i=ImageList_ReplaceIcon(il -1 hi) ;;slower than CopyImage
	PN
PO
out _i

ARRAY(__Hicon) ai.create(5)
PF
for _i 0 5
	ai[_i]=ImageList_GetIcon(il _i 0) ;;>3 times slower than CopyImage
	PN
PO
out ai[0]

DestroyIcon hi
DestroyIcon hi2
