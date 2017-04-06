 /dialog_QM_Tools
function _lock

 Sets mw_lock and enables/disables buttons.

int i=_lock; if(i<0 or i>3) i=0
if(i=mw_lock) ret
int plock=mw_lock
mw_lock=i

EnableWindow id(510 m_hwnd) i=0||i=3
EnableWindow id(511 m_hwnd) i=0||i=1
EnableWindow id(512 m_hwnd) i=0||i=2

if(i) _WinSelect(i)
else if(plock=3) _WinSelect(1)
