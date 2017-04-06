 dll "qmhook32.dll" TestHook !on
dll- "$desktop$\testhk.dll" TestHook !on

TestHook 1
mes "running"
TestHook 0

UnloadDll "$desktop$\testhk.dll"
