int atom=RegWinClass("testcls" &WndProcA)
out _s.dllerror
out atom
 int h=CreateWindowEx(0 +atom "aqą" WS_VISIBLE|WS_OVERLAPPEDWINDOW 10 10 300 200 0 0 _hinst 0)
int h=CreateWindowEx(0 "testcls" "aqą" WS_VISIBLE|WS_OVERLAPPEDWINDOW 10 10 300 200 0 0 _hinst 0)
out _s.dllerror
out h
if(!h) ret
MessageLoop
