2
ARRAY(KEYEVENT) a=key(a)

int tMe(GetCurrentThreadId) tFore(GetWindowThreadProcessId(child 0))
AttachThreadInput(tFore tMe 1)

str s.all(256)
GetKeyboardState s
 s[16]|=0x80
s[16]&=0x7f
SetKeyboardState s
_key a
0.1
 s[16]&=0x7f
s[16]|=0x80
SetKeyboardState s

AttachThreadInput(tFore tMe 0)

#ret
aAAaaa