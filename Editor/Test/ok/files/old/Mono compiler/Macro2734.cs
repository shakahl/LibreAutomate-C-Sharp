del- "Q:\Test\test.exe"; err
del- "Q:\Test\test.dll"; err
SetCurDir "Q:\Test"
PF
RunConsole2 "''C:\Program Files (x86)\Mono\lib\mono\4.5\mcs.exe'' ''Q:\Test\test.cs''" _s
PN
PO
if(_s.len) out _s

 speed: default 170 ms, with /noconfig 70-80 ms
