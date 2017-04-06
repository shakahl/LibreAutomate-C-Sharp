 Sometimes stop working acc hooks in some single process.
 Start working after restarting QM or that process.
 It happens when:
   1. Macro activates a window (eg Notepad), an that window triggers a function (win act trigger). Or maybe user activates, but probably everything must happen quickly.
   2. Then macro quickly restarts QM (after ~0.4 s), and at QM startup the macro runs again (activates the window).
 Tested: does not load qmhook32.dll into that process. Although loads into other processes.
 Reproduced on Windows 7 and XP (notebook).
 Also tested with Avast temporarily disabled.
 It started happening after reprogramming hooks. Never before.

 Solved.
 Actually hooks work, but cannot write to mailslot.
 After quick restart QM, in some processes qm dll is not unloaded/loaded, and the process uses old invalid mailslot handle.
 Then need to close and open mailslot again, and retry write.
