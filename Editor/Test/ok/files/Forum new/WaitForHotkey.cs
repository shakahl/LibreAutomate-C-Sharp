 /
function $qmKeyCode

 Temporariy registers a hotkey and waits for it.

 qmKeyCode - QM <help>key</help> codes.

 EXAMPLE
 WaitForHotkey "CF10" ;;Ctrl+F10


int i k vk mod
rep
	i=QmKeyCodeToVK(qmKeyCode k)
	if(i=0) break
	qmKeyCode+i
	sel k
		case VK_CONTROL mod|MOD_CONTROL
		case VK_SHIFT mod|MOD_SHIFT
		case VK_MENU mod|MOD_ALT
		case VK_LWIN mod|MOD_WIN
		case else vk=k
if(vk=0) end "no key"

__RegisterHotKey hk
if(!hk.Register(0 45227 mod vk)) end "failed to register the hotkey, probably it is taken be some other application or Windows"

rep
	MSG m; if(GetMessage(&m 0 0 0)<1) end
	if(m.message==WM_HOTKEY and m.wParam=45227) ret
	DispatchMessage &m
