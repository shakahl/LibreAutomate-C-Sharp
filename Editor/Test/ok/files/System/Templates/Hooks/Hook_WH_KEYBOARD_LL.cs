 /
function# nCode message KBDLLHOOKSTRUCT&k
if(nCode<0) goto gNext

FormatKeyString k.vkCode 0 &_s; out "%s %s%s" _s iif(k.flags&LLKHF_UP "up" "down") iif(k.flags&LLKHF_INJECTED ", injected" "") ;;debug

 your code here

 gNext
ret CallNextHookEx(0 nCode message &k)

  SetWindowsHookEx example
 int hh=SetWindowsHookEx(WH_KEYBOARD_LL &sub.Hook_WH_KEYBOARD_LL _hinst 0)
 out "Press a key"
 opt waitmsg 1
 wait -1
 UnhookWindowsHookEx hh
