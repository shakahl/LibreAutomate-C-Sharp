 Run this function to turn on remapping. Run again to turn off.


 if already running, end the thread and exit
if getopt(nthreads)>1
	shutdown -6 0 "remap_keyboard"
	ret

 set hook and wait
int keyhook=SetWindowsHookEx(WH_KEYBOARD_LL &remap_keyboard_hook _hinst 0)
MessageLoop ;;need to process messages
UnhookWindowsHookEx keyhook
