 \
function# hWnd message wParam lParam
if(hWnd) goto messages

type KT2_ITEM @iid @flags !mod !vk !regged

ARRAY(KT2_ITEM)+ __kt2a

hWnd=CreateWindowEx(0 +32770 0 0 0 0 0 0 0 0 _hinst 0) ;;this hidden window will receive WM_HOTKEY and other messages
int+ __kt2wndproc=SubclassWindow(hWnd &KT2_Main) ;;subclass
__kt2hwnd=hWnd
MessageLoop ;;retrieve messages; run until PostQuitMessage
ret

 messages
sel message
	case WM_HOTKEY
	KT2_ITEM& k=__kt2a[wParam]
	 if(dis(k.iid)) ret ;;in this case, this is not needed because, if an item or QM is disabled, the hotkey is not registered
	if(k.flags&2=0) key- CSAW
	mac k.iid
	
	case WM_CLOSE if(wParam) DestroyWindow(hWnd)
	
	case WM_DESTROY
	KT2_Table
	__kt2hwnd=0
	PostQuitMessage 0
	
	case WM_APP
	if(!KT2_Table(+lParam)) DestroyWindow hWnd

ret CallWindowProc(__kt2wndproc hWnd message wParam lParam)
