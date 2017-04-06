function [nCode] [wParam] [MSLLHOOKSTRUCT*m]

 User-defined mouse triggers.
 To add/delete triggers, edit this function.
 To start the trigger engine, run this function without arguments. For example click the Run button or use a trigger or mac.
 Runs all the time, but if you want to stop it, use the Threads dialog or Ctrl+Alt+Win+LeftMouseButton.


 ------------------------------
 --- don't modify this code ---
 ------------------------------


if(getopt(nargs)=0)
	int-- t_mhook=SetWindowsHookEx(WH_MOUSE_LL &AltMouseTriggers _hinst 0)
	opt waitmsg 1
	AddTrayIcon "mouse.ico" "My mouse triggers" ;;remove this line if tray icon is not needed
	wait -1 -V t_mhook
	ret

if(nCode<0) goto g1
 out m.flags
if(m.flags&LLMHF_INJECTED) goto g1 ;;not user-generated


 ------------------------------
 --- can modify this code -----
 ------------------------------


 This code runs whenever a mouse button is clicked, wheel rotated or mouse moved.
 Edit this code to do whatever you need.
 Don't add here code that runs > 0.1 s or uses key/mouse commands. Rather put the code in a macro and use mac to launch it.
 Return 1 to "eat" the mouse event. Otherwise make sure that CallNextHookEx is executed.
 For reference, search for LowLevelMouseProc in the MSDN library on the internet.
 This is just example code. Launches Macro522 on Ctrl+MouseRightButton.


sel wParam
	case [WM_LBUTTONDOWN,WM_LBUTTONUP]
	sel GetMod ;;1 Shift, 2 Ctrl, 4 Alt, 8 Win
		case 2|4|8 ;;Ctrl+Alt+Win
		if(wParam=WM_LBUTTONDOWN)
			UnhookWindowsHookEx t_mhook
			t_mhook=0 ;;tell to end this thread
		ret 1 ;;eat
		 here you can add more case for other Ctrl/Shift/Alt/Win combinations
		
	case [WM_RBUTTONDOWN,WM_RBUTTONUP]
	sel GetMod
		case 2 ;;Ctrl
		if(wParam=WM_RBUTTONDOWN)
			mac "Macro522"
			err out "there is no Macro522"
		ret 1 ;;eat
		 here you can add more case for other Ctrl/Shift/Alt/Win combinations
	
	  below is commented barebone code for other mouse events
	 case [WM_MBUTTONDOWN,WM_MBUTTONUP]
	  here you can add code similar to above
	
	 case [WM_XBUTTONDOWN,WM_XBUTTONUP]
	 sel m.mouseData>>16
		 case XBUTTON1
		  here you can add code similar to above
		 case XBUTTON2
		  here you can add code similar to above
		
	 case WM_MOUSEWHEEL
	 if(m.mouseData>0) ;;forward
		  here you can add code similar to above
	 else if(m.mouseData<0) ;;backward
		  here you can add code similar to above
		
	 case WM_MOUSEMOVE
	  here you can add code



 ------------------------------
 --- don't modify this code ---
 ------------------------------

 g1
ret CallNextHookEx(0 nCode wParam m)
