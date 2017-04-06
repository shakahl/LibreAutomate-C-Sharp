out

spe 10
key+ r

lock hookThread258963 "" 0; err ret
out 1
mac "sub.HookThread"


#sub HookThread
lock hookThread258963
int hh=SetWindowsHookEx(WH_KEYBOARD_LL &sub.HookProc _hinst 0)
opt waitmsg 1
wait -1
UnhookWindowsHookEx hh


#sub HookProc
function# nCode message KBDLLHOOKSTRUCT&k
if(nCode<0) goto gNext

 FormatKeyString k.vkCode 0 &_s; out "%s %s%s" _s iif(k.flags&LLKHF_UP "up" "down") iif(k.flags&LLKHF_INJECTED ", injected" "") ;;debug

 if(k.vkCode=VK_F8) 

 gNext
ret CallNextHookEx(0 nCode message &k)


#ret
rr