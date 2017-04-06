function block [hwnd] [flags] ;;block: 1 block keyboard, 2 block mouse, 3 block mouse and keyboard, 0 unblock, -1 unblock temporarily, -2 restore blocking;  flags: 1 allow mouse move, 2 block in windows of other macros too, 4 unblock on Ctrl+Alt+Delete, 8 end thread on Ctrl+Alt+Delete 

 Blocks keyboard and/or mouse input.

 Like BlockInput, automatically unblocks when the macro ends, even if it does not call BlockInput2 0.

 Unlike BlockInput:
     Ctrl+Alt+Delete cannot unblock. To unblock on Ctrl+Alt+Delete, use flag 4, and also at least keyboard must be blocked. If flag 8 also used, also ends thread that called BlockInput2.
     Can block only keyboard or only mouse.
     Other threads still can use keyboard and mouse commands. Blocked is only real user input.
     Does not block in macro windows (mes, inp, custom dialogs, etc). If flag 2 used, does not block only in windows of current thread.
     Other threads can temporarily unblock input.
     You can modify BIKeyboardProc/BIMouseProc to block or allow certain keystrokes etc.
     If you specify a window (hwnd), input is blocked only while that window is active.
 
 EXAMPLE
 BlockInput2 3
 run "Notepad"
 5
 inp _s
 key (_s)
 5
 BlockInput2 0


int+ __bikhook __bimhook __bihwnd __biflags __bitempunblock=0
if(block>0)
	opt waitmsg 2
	if(__bikhook) UnhookWindowsHookEx(__bikhook)
	if(__bimhook) UnhookWindowsHookEx(__bimhook)
	if(block&1) __bikhook=SetWindowsHookEx(WH_KEYBOARD_LL &BIKeyboardProc _hinst 0)
	if(block&2) __bimhook=SetWindowsHookEx(WH_MOUSE_LL &BIMouseProc _hinst 0)
	__bihwnd=hwnd
	__biflags=flags
else if(!block)
	UnhookWindowsHookEx(__bikhook); __bikhook=0
	UnhookWindowsHookEx(__bimhook); __bimhook=0
else __bitempunblock=block=-1
