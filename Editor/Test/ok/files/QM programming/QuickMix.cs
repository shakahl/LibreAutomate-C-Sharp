dll "qm.exe" __QuickMix !*v lens

int bytes=0

str s
 s="ReadProcessMemory"
 s="VirtualAllocEx"
 s="VirtualFreeEx"
 s="This file created with unregistered Quick Macros.[][]Try again after 1 minute."


__QuickMix s s.len
 outb s s.len 1

if bytes
	s.encrypt(8 s "" 1)
	s.replacerx("\w\w" "0x$0,")
else
	s.encrypt(8 s)
	s.replacerx("\w\w" "\x$0")
out s
