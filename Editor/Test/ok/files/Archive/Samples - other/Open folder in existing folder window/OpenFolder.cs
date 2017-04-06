 /
function hwnd $folder

 Opens folder in existing Windows Explorer window.

 EXAMPLES
 OpenFolder win("" "CabinetWClass") "$program files$"
 OpenFolder win("" "CabinetWClass") "$0$" ;;desktop
 OpenFolder win("" "CabinetWClass") ":: 14001F50E04FD020EA3A6910A2D808002B30309D" ;;My Computer


SHDocVw.InternetExplorer f=GetFolderInterface(hwnd)
if(!f) end ES_FAILED
VARIANT v
str s.expandpath(folder)
if(s.beg(":: "))
	ITEMIDLIST* il=PidlFromStr(s)
	int ils=ILGetSize(il)
	ARRAY(byte) a.create(ils)
	memcpy(&a[0] il ils)
	CoTaskMemFree(il)
	v.attach(a)
else v=s
f.Navigate2(v)
