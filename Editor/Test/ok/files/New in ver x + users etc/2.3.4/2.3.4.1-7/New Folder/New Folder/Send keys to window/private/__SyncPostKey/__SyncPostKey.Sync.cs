function &sync

int n=sync; sync=0
 out n

if(!m_tid) 0; ret ;;window of this thread. TODO: test

 PF

if(!_WaitIdle(n)) wait 0.01+(n*0.001)
 ret

 if(!m_hp) m_hp=OpenProcess(PROCESS_QUERY_X_INFORMATION 0 m_pid)
 WaitForInputIdle(m_hp INFINITE)
 rep(1000) SleepMP; if(GetUpdateRect(m_hwnd 0 0)) out "ur"; break
 ret
 PN

 TODO: still bad sync in Notepad with scrollbar, packet

int i
rep 1 ;;in some windows, eg DW, 1 time is not enough. Almost same speed.
	if(!_Invalidate) 0.1; break ;;TODO: create something better for invisible/minimized/offscreen mode
	for i 0 10000
		if(!GetUpdateRect(m_hwnd 0 0)) break ;;WM_PAINT already received; it should be after all WM_KEYDOWN etc, however in some windows it is not always true, that is why need rep 2.
		_Sleep
		if(i>50) 0.01 ;;normally 1-3, sometimes 4-10. With 2 CPU.
	 out i

if(m_tam) SetThreadAffinityMask(GetCurrentThread m_tam); m_tam=0

 PN;PO

 note: in most cases waitforinputidle not necessary.
   But eg in Notepad, if scrolls when sending text, does not sync without this, especially newlines.
   Because, when receiving wm_char, window itself redraws and clears update rect synchronously, earlier than redrawwindow would do it asynchronously.
   In all tested windows, with waitforinputidle was same speed or faster. Speed more stable.
   It seems like waitforinputidle waits while the window is processing older messages, and the rest of code waits while the window is processing newer messages (especially wm_char messages posted by translatemessage).
