 /

function $hotkey

 Sets a hotkey to end current thread (running macro or function).

 hotkey - a single key without modifiers (Ctrl etc), specified as QM key code like with <help>key</help> and QM keyboard triggers.

 EXAMPLE
 SetEndThreadHotkey "F8"
  macro
 rep
	 out 1
	 wait 1


int mod vk; if(!TO_HotkeyFromQmKeys(hotkey mod vk)) end "invalid hotkey string"
if(mod) end "modifier keys not supported"

QMTHREAD qt; GetQmThreadInfo(0 qt)
mac "sub.Thread" "" qt.threadhandle vk


#sub Thread
function ht vk

int- __ht82657(ht) __vk82657(vk)

int hh=SetWindowsHookEx(WH_KEYBOARD_LL &sub.Hook_WH_KEYBOARD_LL _hinst 0)
opt waitmsg 1
wait 0 H ht
UnhookWindowsHookEx hh


#sub Hook_WH_KEYBOARD_LL
function# nCode message KBDLLHOOKSTRUCT&k
if(nCode<0) goto gNext
if(k.flags&(LLKHF_UP|LLKHF_INJECTED)) goto gNext

int- __ht82657 __vk82657
if k.vkCode=__vk82657
	EndThread "" __ht82657
	ret 1

 gNext
ret CallNextHookEx(0 nCode message &k)
