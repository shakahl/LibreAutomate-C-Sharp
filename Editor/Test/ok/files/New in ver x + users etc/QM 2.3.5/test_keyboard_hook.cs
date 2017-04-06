 /exe
function nCode wParam KBDLLHOOKSTRUCT&h

if(getopt(nargs)=0)
	if(getopt(nthreads)>1) ret
	File-- f.Open("$desktop$\test_keyboard_hook.txt" "w")
	int keyhook=SetWindowsHookEx(WH_KEYBOARD_LL &test_keyboard_hook _hinst 0)
	mes "Now this macro is recording keys to $desktop$\test_keyboard_hook.txt, until you close this message box."
	f.Close
	run "$desktop$\test_keyboard_hook.txt"
	ret

 ---- this code runs on each key down and up event -----

str s sk st
FormatKeyString h.vkCode 0 &sk
st.timeformat("{TT}")
s.format("%s. %s %s%s" st sk iif(h.flags&LLKHF_UP "up" "down") iif(h.flags&LLKHF_INJECTED ", injected" ""))
f.WriteLine(s)

ret CallNextHookEx(0 nCode wParam &h)

 BEGIN PROJECT
 main_function  test_keyboard_hook
 exe_file  $desktop$\test_keyboard_hook.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {BC94C259-A195-47DB-B7B2-F7C6E8246C2B}
 END PROJECT
