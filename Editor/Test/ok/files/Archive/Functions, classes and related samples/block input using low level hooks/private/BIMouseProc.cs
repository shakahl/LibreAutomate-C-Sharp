 /
function nCode wParam MSLLHOOKSTRUCT*m

if(nCode>=0)
	 don't block if not user-generated, or if temporarily unblocked
	if(m.flags&1 or __bitempunblock) goto g1
	 if specified, allow mouse move
	if(__biflags&1) if(wParam=WM_MOUSEMOVE) goto g1
	 if a window is specified, block only when it is active
	if(__bihwnd) if(__bihwnd=win) ret 1; else goto g1
	 don't block if the window belongs to a macro (if flags&2 - to current thread)
	int h tid=iif(__biflags&2 GetCurrentThreadId 0)
	if(wParam=WM_MOUSEMOVE)
		h=win
		if(h and IsMacroWindow(h tid)) goto g1 ;;allow mouse move if active
	else
		h=win(m.pt.x m.pt.y)
		if(h and IsMacroWindow(h tid)) goto g1 ;;allow clicks if inside
		 if there are macro windows, activate
		h=IsMacroWindow(0 tid); if(h) act h; err
	ret 1 ;;eat
 g1
ret CallNextHookEx(0 nCode wParam m) ;;don't eat
