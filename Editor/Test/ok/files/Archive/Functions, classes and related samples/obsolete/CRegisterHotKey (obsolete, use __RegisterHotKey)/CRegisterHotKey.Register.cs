function $hotkey hWnd hkId

 Registers hotkey using Windows API function RegisterHotKey.
 Error if fails (an argument is invalid, or the hotkey is already registered).
 You can register multiple hotkeys with a single variable. Call this function multiple times with different hkId.

 hotkey - hotkey in QM format, like with key command. Can be used Ctrl, Shift, Alt and Win followed by a single key, or only a single key. Virtual key codes in parenthese cannot be used.
 hWnd - handle of window that will receive WM_HOTKEY message when the hotkey is pressed. Must belong to current thread. If 0, WM_HOTKEY must be handled in message queue, or you can wait for it using WaitFor. If you register multiple hotkeys with same variable, hWnd must be same for all hotkeys.
 hkId - hotkey id. Must be 1 to 0xBFFF. Every hotkey in current thread must be registered with different id. 

 For more info, read about RegisterHotKey in MSDN library.


if(hkId<1 or hkId>0xBFFF) end "hkId must be 1 to 0xBFFF"
if(m_a.len and hWnd!=m_hwnd) end "All hotkeys of a single variable must be with same hWnd."
int k mod i vk
rep
	i=QmKeyCodeToVK(hotkey &vk)
	if(!i) break
	sel(vk)
		case VK_SHIFT mod|MOD_SHIFT
		case VK_CONTROL mod|MOD_CONTROL
		case VK_MENU mod|MOD_ALT
		case [VK_LWIN,VK_RWIN] mod|MOD_WIN
		case else k=vk; break
	hotkey+i
if(!k) ret ES_BADARG
 out "%i %i" mod k
for(i 0 m_a.len) if(hkId=m_a[i]) UnregisterHotKey m_hwnd hkId; m_a.remove(i); break
if(!RegisterHotKey(hWnd hkId mod k)) end _s.dllerror
m_hwnd=hWnd
m_a[]=hkId
