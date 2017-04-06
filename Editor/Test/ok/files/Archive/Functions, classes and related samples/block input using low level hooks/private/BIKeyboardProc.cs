 /
function nCode wParam KBDLLHOOKSTRUCT*k

if(nCode>=0)
	 if specified, unblock on Ctrl+Alt+Delete
	if(__biflags&4 and k.vkCode=VK_DELETE and GetMod=6)
		BlockInput2 0
		if(__biflags&8) end
		goto g1
	 don't block if not user-generated, or if temporarily unblocked
	if(k.flags&16 or __bitempunblock) goto g1
	 if a window is specified, block only when it is active
	if(__bihwnd) if(__bihwnd=win) ret 1; else goto g1
	 don't block if the window belongs to a macro (if flags&2 - to current thread)
	if(win and IsMacroWindow(win iif(__biflags&2 GetCurrentThreadId 0))) goto g1

	  example of allowing certain keystrokes
	 sel k.vkCode ;;virtual-key code
		 case 'A' if(GetMod=2) key Ca ;;resend Ctrl+A
		 case VK_F5 if(GetMod=0) key F5 ;;resend F5
		  ...
	 On key up events, k.flags includes 0x80.
	 Help: click on KBDLLHOOKSTRUCT and press F1...
			
	ret 1 ;;eat
 g1
ret CallNextHookEx(0 nCode wParam k) ;;don't eat
