 Shows how to use __RegisterHotKey to use hotkeys in a windowless thread.

__RegisterHotKey hk1.Register(0 1 MOD_CONTROL|MOD_SHIFT VK_F5)
 also can register more hotkeys, for example
__RegisterHotKey hk2.Register(0 2 MOD_CONTROL 'B')
 ...

rep
	MSG m; if(GetMessage(&m 0 0 0)<1) break
	DispatchMessage &m
	sel m.message
		case WM_HOTKEY
		 edit this code if need. The numbers must match the numbers passed to Register.
		sel m.wParam
			case 1 ;;Ctrl+Shift+F5 pressed
			mac "Function_that_does_something_on_Ctrl_Shift_F5"
			
			case 2
			out "Ctrl+B"
			
			 ...
