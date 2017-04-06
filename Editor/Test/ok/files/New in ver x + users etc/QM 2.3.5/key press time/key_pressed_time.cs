function# nCode message KBDLLHOOKSTRUCT&k

 This code runs when you launch this function. It starts or stops detecting key times.
if(getopt(nargs)=0)
	if(getopt(nthreads)>1) shutdown -6 0 "key_pressed_time"; ret
	AddTrayIcon "$qm$\keyboard.ico" "QM - key pressed time"
	int hHook=SetWindowsHookEx(WH_KEYBOARD_LL &key_pressed_time _hinst 0)
	opt waitmsg 1
	wait -1
	UnhookWindowsHookEx hHook
	ret

 This code runs on a key event.
if(nCode<0) goto gNext
if(k.flags&LLKHF_INJECTED) goto gNext

FormatKeyString k.vkCode 0 &_s; out "%s %s" _s iif(k.flags&LLKHF_UP "up" "down") ;;debug

ARRAY(int)+ g_key_times; if(!g_key_times.len) g_key_times.create(256)
int vk=k.vkCode&0xff
if(k.flags&LLKHF_UP=0)
	if(g_key_times[vk]) goto gNext ;;repeated
	g_key_times[vk]=iif(k.time k.time -1)
	goto gNext
int t=k.time-g_key_times[vk]
g_key_times[vk]=0
 ____________________________________________

 This code runs on a user-pressed key up event.
out t ;;key pressed duration in milliseconds
 add more code here, for example:
 MorseFunction vk t
  or
 MyKeyTriggers vk t


 ____________________________________________

 gNext
ret CallNextHookEx(0 nCode message &k)
