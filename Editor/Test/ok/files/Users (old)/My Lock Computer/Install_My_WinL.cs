 Run this function. It runs continuously and launches macro MyLockComputer on Win+L.
 To run it automatically when QM starts, insert this line into init2 function:
 mac "Install_My_WinL"
 Does not work on Windows 98 and Me.


#if _winnt

def LLKHF_EXTENDED 1
def LLKHF_INJECTED 0x10
def LLKHF_ALTDOWN 0x20
def LLKHF_UP 0x80
def LLMHF_INJECTED 1
def WH_KEYBOARD_LL 13
def WH_MOUSE_LL 14
type KBDLLHOOKSTRUCT vkCode scanCode flags time dwExtraInfo
type MSLLHOOKSTRUCT POINT'pt mouseData flags time dwExtraInfo
dll user32 #SetWindowsHookEx idHook lpfn hmod dwThreadId
dll user32 #UnhookWindowsHookEx hHook
dll user32 #CallNextHookEx hHook ncode wParam !*lParam

if(getopt(nthreads)>1) ret

int+ __llkhook_winl=SetWindowsHookEx(WH_KEYBOARD_LL &LLKeyboardProc_WinL _hinst 0)
MessageLoop
UnhookWindowsHookEx __llkhook_winl
