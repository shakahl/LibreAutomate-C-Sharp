 /
function# [^waitMaxS] [vk] [flags] ;;flags: 1 eat

 Waits for key down event.
 Can be used instead of wait KF, which cannot be used in exe.

 Returns virtual key code (value 1-255). The table is in QM help.

 waitMaxS - max number of seconds to wait. Default or 0 is infinite. Error on timeout.
 vk - virtual-key code. If used and not 0, waits for this key. Else waits for any key.

 This function does not work well if this process uses direct input or raw input or keyboard detector.


type __WFKDATA vk r flags
__WFKDATA- __wfk
__wfk.r=0
__wfk.vk=vk
__wfk.flags=flags
int hkey=SetWindowsHookEx(WH_KEYBOARD_LL &__WFK_KeyProc _hinst 0)
opt waitmsg 1
int e
wait waitMaxS V __wfk.r
err e=1
UnhookWindowsHookEx hkey
if(e) end _error
ret __wfk.r
