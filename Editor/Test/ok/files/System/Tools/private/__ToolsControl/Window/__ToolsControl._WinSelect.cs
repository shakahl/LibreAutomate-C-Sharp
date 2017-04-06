 /dialog_QM_Tools
function i ;;i: -1 none, 0,3 screen, 1 window, 2 control

 Sets mw_what, checks/unchecks option buttons, disables controls, if need sets default text of edit controls.
 If locked, ignores i and uses the lock instead.

if(mw_lock) i=mw_lock
if(i>2) i=0
if(i=mw_what) ret
mw_what=i

CheckRadioButton m_hwnd 510 512 iif(i>=0 510+i 0)
TO_Enable m_hwnd "521 522" mw_what>0
if(m_flags&0x1000) TO_Show m_hwnd "520" mw_what!0
InvalidateRect mw_heW 0 0; InvalidateRect mw_heC 0 0
