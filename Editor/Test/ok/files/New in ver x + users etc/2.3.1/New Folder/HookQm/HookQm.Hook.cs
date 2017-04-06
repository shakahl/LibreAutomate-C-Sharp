 /
function flags ;;flags: 1 WH_CALLWNDPROC (__HQ_Send), 2 WH_GETMESSAGE (__HQ_Post)

if(flags&1 and !m_hs) m_hs=SetWindowsHookEx(WH_CALLWNDPROC &__HQ_Send 0 GetWindowThreadProcessId(_hwndqm 0))
if(flags&2 and !m_hp) m_hp=SetWindowsHookEx(WH_GETMESSAGE &__HQ_Post 0 GetWindowThreadProcessId(_hwndqm 0))
