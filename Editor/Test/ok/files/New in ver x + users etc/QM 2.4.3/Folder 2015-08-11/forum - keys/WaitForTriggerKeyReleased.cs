 /
function [vk] [^timeS]

opt noerrorshere
if !vk
	QMITEM q
	if(!qmitem(getopt(itemid 3) 0 q)) end ERR_FAILED
	if(q.ttype!=1 or !q.tkey or q.tkey2) end "if vk 0, this macro must have a single-key keyboard trigger"
	vk=q.tkey
if(!timeS) timeS=-1

ifk vk ;;probably not using low-level hooks
	wait timeS (vk)
	ret 

__WindowsHook hh=SetWindowsHookEx(WH_KEYBOARD_LL &sub.HookProc _hinst 0)
if(!hh) end ERR_FAILED
int- t_WaitForTriggerKeyReleased
opt waitmsg 1
wait timeS -V vk



#sub HookProc v
function# nCode message KBDLLHOOKSTRUCT&k
if(nCode<0) goto gNext

 FormatKeyString k.vkCode 0 &_s; out "%s %s%s" _s iif(k.flags&LLKHF_UP "up" "down") iif(k.flags&LLKHF_INJECTED ", injected" "") ;;debug

if(k.vkCode=vk) vk=0

 gNext
ret CallNextHookEx(0 nCode message &k)
