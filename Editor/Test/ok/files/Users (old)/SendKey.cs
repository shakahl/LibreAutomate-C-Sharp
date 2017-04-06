 /
function hwnd vk [flags] ;;flags: 0 down-up, 1 down, 2 up

 Posts keyboard messages directly to a control. The window can be inactive.
 You can find virtual-key codes in QM help. For alphanumeric keys, use uppercase characters in '', eg 'A'.

 hwnd - control handle.
 vk - virtual-key code.

 EXAMPLE
 int hwnd=id(15 "Notepad")
 SendKey hwnd 'A'
 1
 SendKey hwnd VK_BACK


int m1 m2 lp=MapVirtualKey(vk 0)<<16|1 ;;should be Ex with HKL of hwnd, although probably nobody will use it
sel(vk) case [3,33,34,35,36,37,38,39,40,44,45,46,91,92,93,111,144,VK_RCONTROL,VK_RMENU] lp|0x1000000 ;;ek
if(flags&3!2) PostMessage hwnd WM_KEYDOWN vk lp
if(flags&3!1) PostMessage hwnd WM_KEYUP vk lp|0xC0000000
