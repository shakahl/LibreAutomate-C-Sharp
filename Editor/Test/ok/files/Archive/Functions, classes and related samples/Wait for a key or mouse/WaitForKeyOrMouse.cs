 /
function# [^waitMaxS] [KBDLLHOOKSTRUCT&ks] [MSLLHOOKSTRUCT&ms]

 Waits for a key or mouse movement or click or wheel.

 Returns:
   on key: virtual key code (value 1-255). The table is in QM help. To get more info, eg key down or up, use ks.
   on mouse action: mouse message (WM_MOUSEMOVE etc, value 512-526). Documented in MSDN library. To get more info, use ms.

 waitMaxS - max number of seconds to wait. Default or 0 is infinite. Error on timeout.
 ks - if used, receives last keyboard event info. Documented in MSDN library.
 ms - if used, receives last mouse event info. Documented in MSDN library.

 This function does not work well if this process uses direct input or raw input or keyboard detector.

 Tip: If need to "eat" the event, before add BlockInput 1. Other way - edit the hook functions: to eat the event, return 1 instead of CallNextHookEx(...).


type WFKMDATA w r KBDLLHOOKSTRUCT*ks MSLLHOOKSTRUCT*ms
WFKMDATA- __wfkm
__wfkm.w=0
__wfkm.r=0
__wfkm.ks=&ks
__wfkm.ms=&ms
int hkey=SetWindowsHookEx(WH_KEYBOARD_LL &__WFKM_KeyProc _hinst 0)
int hmouse=SetWindowsHookEx(WH_MOUSE_LL &__WFKM_MouseProc _hinst 0)
opt waitmsg 1
int e
wait waitMaxS V __wfkm.w
err e=1
UnhookWindowsHookEx hkey
UnhookWindowsHookEx hmouse
if(e) end _error
ret __wfkm.r
