function! hWnd hkId mod vk  ;;mod: MOD_ALT (1), MOD_CONTROL (2), MOD_SHIFT (4), MOD_WIN (8)

 Registers hotkey.
 Uses <google>RegisterHotKey</google>. Parameters are the same.
 Returns 1. Returns 0 if failed, eg when the hotkey is already registered by an application or Windows.

 hWnd - handle of a window or dialog of current thread. On hotkey it will receive WM_HOTKEY message in its window/dialog procedure.
   If 0, WM_HOTKEY message will be posted to current thread. Retrieve it in a message loop (repeatedly call GetMessage or PeekMessage).
 hkId - hotkey id. Must be 0 to 0xBFFF.
 mod - flags for modifier keys.
   QM 2.4.2: if vk is 0, uses mod as a word containing virtual-key code and modifier flags (HOTKEYF_) as retrieved by a hotkey control.
 vk - <help #IDP_VIRTUALKEYS>virtual-key code</help>.

 REMARKS
 When this variable dies, it unregisters the hotkey. Also unregisters old hotkey before registering new hotkey.
 If you need multiple hotkeys, use multiple __RegisterHotKey variables or single ARRAY(__RegisterHotKey) variable.


if m_hkid
	end "to register multiple hokeys, use multiple __RegisterHotKey variables or ARRAY(__RegisterHotKey)" 8
	Unregister

if(!vk) vk=mod&0xff; mod=(mod>>10&1)|(mod>>8&2)|(mod>>6&4) ;;HOTKEYF_ to MOD_

if(!RegisterHotKey(hWnd hkId mod vk)) ret
m_hwnd=hWnd; m_hkid=hkId
ret 1
