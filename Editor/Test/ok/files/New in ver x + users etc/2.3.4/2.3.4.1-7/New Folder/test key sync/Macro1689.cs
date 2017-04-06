out
 2
 int w=win("Test TSM" "#32770")
 int w=win("Administrator: C:\Windows\system32\cmd.exe" "ConsoleWindowClass")
int w=win("Untitled - Notepad" "Notepad")
 but 4 w
 men 40020 win("DebugView" "dbgviewClass") ;;Clear Display
0.1

 __RegisterHotKey hk
 hk.Register(0 1 0 VK_F20)

act w
 SetThreadPriority GetCurrentThread THREAD_PRIORITY_IDLE
 Sleep 100
_key3 key(abcdef F11)
 _key3 key((VK_F11))
 spe
 opt keysync 1
  opt keymark 1
 key(abcdef F20)

 MSG m
 GetMessage(&m 0 0 0)
 outx m.message
 WM_HOTKEY 0x312

 0.001
 Sleep 1
 SendMessage w 0 0 0
 acc
 acc

KeySync

out F"sent"


 WaitMessage
 MSG m
 out PeekMessage(&m 0 0 0 PM_REMOVE)
 outx m.message
  WM_HOTKEY 0x312
