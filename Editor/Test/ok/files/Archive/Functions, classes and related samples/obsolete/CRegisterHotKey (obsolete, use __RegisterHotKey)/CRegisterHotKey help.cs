 Registers a hotkey using Windows API function RegisterHotKey.
 Automatically uregisters when the variable is destroyed.
 You can use it to:
   add global hotkeys to your dialog;
   create key triggers;
   wait for hotkey;
   wait for key instead of wait 0 KF, which is unavailable in exe.

 EXAMPLES

#compile __CRegisterHotKey
CRegisterHotKey k
k.Register("Ck" 0 70) ;;register hotkey Ctrl+K with id 70
k.Register("CSF10" 0 71) ;;register hotkey Ctrl+Shift+F10 with id 71

 Waiting with WaitFor.
int hkId=k.WaitFor(10) ;;wait max 10 s until one of the hotkeys is pressed
out "WaitFor: hotkey %i pressed." hkId

 Waiting with message loop.
rep
	MSG m; if(GetMessage(&m 0 0 0)<1) break
	sel m.message
		case WM_HOTKEY
		out "Message loop: hotkey %i pressed" m.wParam
		break
	TranslateMessage &m; DispatchMessage &m

 See also in examples folder.
