 /
function! ARRAY(str)&a [hwnd] [flags] ;;flags: 1 selection

 Gets full paths of items in the folder that is opened in Windows Explorer (WE).
 Returns 1 on success, 0 if failed. If empty, or empty selection (flag 1), returns 1, but the array is empty.

 a - receives paths.
 hwnd - WE window handle. If 0 or omitted, finds WE window that is above all other WE windows in Z order.

 EXAMPLE
 ARRAY(str) a
 if GetFolderWindowItems(a win("" "CabinetWClass") 0)
	 out a


a=0
SHDocVw.InternetExplorer ie=GetFolderWindowIE(hwnd); if(!ie) ret
IShellView isv
IShellBrowser isb
IServiceProvider isp=+ie
isp.QueryService(uuidof(IShellBrowser) uuidof(IShellBrowser) &isb)
isb.QueryActiveShellView(&isv)
IDataObject ido
isv.GetItemObject(iif(flags&1 SVGIO_SELECTION SVGIO_ALLVIEW) IID_IDataObject &ido); err ret 1
FORMATETC f.cfFormat=RegisterClipboardFormat(CFSTR_SHELLIDLIST)
f.dwAspect=DVASPECT_CONTENT; f.lindex=-1; f.tymed=TYMED_HGLOBAL
#opt nowarnings 1
STGMEDIUM sm
if(ido.GetData(&f &sm)) ret
CIDA* c=GlobalLock(sm.hGlobal)

int i
a.create(c.cidl)
for i 0 c.cidl
	ITEMIDLIST* il=ILCombine(+(c+c.aoffset[0]) +(c+c.aoffset[i+1]))
	PidlToStr(il &a[i])
	CoTaskMemFree il

GlobalUnlock(sm.hGlobal); ReleaseStgMedium(&sm)

err+ ret
ret 1
