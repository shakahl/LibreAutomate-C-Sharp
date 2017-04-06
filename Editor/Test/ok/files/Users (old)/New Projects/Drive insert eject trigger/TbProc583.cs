 /Toolbar583
function# hWnd message wParam lParam

type SHChangeNotifyEntry ITEMIDLIST*pidl fRecursive
dll shell32
	[2]#SHChangeNotifyRegister hwnd fSources fEvents wMsg cEntries SHChangeNotifyEntry*pshcne
	[4]#SHChangeNotifyDeregister ulID

type TB583 nid ITEMIDLIST*pidl
TB583* p

sel message
	case WM_CREATE
	SetProp(hWnd "p" p._new)
	SHChangeNotifyEntry e.pidl=PidlFromStr(":: 14001F50E04FD020EA3A6910A2D808002B30309D") ;;My Computer
	p.nid=SHChangeNotifyRegister(hWnd 3 -1 WM_APP 1 &e)
	
	case WM_DESTROY
	p=+GetProp(hWnd "p")
	if(p)
		SHChangeNotifyDeregister p.nid
		CoTaskMemFree p.pidl
		p._delete
	
	case WM_APP
	out 