ShutDownProcess "CatkeysTasks"; 0.1
 0.5

PerfFirstSM
 out perf


 StartProcess 0 "Q:\app\catkeys\tasks\CatkeysTasks.exe" "/perf"
StartProcess 0 "Q:\app\catkeys\tasks\CatkeysTasks.exe" "/run ''%catkeys%\scripts\test.cs''"
 StartProcess 0 "Q:\app\catkeys\tasks\CatkeysTasks.exe" "/perf /run ''Q:\Test\form.cs''"
 StartProcess 0 "notepad.exe"

 menu strip:
 17 + 26
 speed:  5188  6081  7370  5308  3360  (27311) (no icons), GAC+ngen = 21. But this is when PC has been restarted recently. Before restarting was > 50.

 WPF menu:
 speed:  7456  16011  2613  108025  3699 = 137 (no icons)
 speed:  7310  16677  10669  118360  3962 = 155 (12 icons)

 WPF toolbar with Popup
 speed:  7080  16709  504  10019  75758  3299 = 111 (30 buttons, no icons), next time in same or other thread 57 ms

 old toolbar with form:
 speed:  2540  39  10490  14603  1810  (29487) (1 button, no icons)
 speed:  335  1  16  95  28444  7403  (36299)  (31 buttons, no icons), next time in same thread the same

 old toolbar with native window. Problem: no ButtonClick event.
 speed:  5882  1969  31  8  13938  3864  1915  (27614)  (31 buttons, no icons), next time in same thread 15 ms

 toolstrip with form
 speed:  2599  14459  54  3006  12602  4455  (37180) (1 button, no icons)
 speed:  2570  15525  53  46  9339  12751  7535  (47826)  (31 buttons, no icons), next time in same thread 38 ms

 toolstrip with native window (CatBar)
 speed:  4272  6457  8684  1531  64  8882  4011  6243  (40152)  (31 buttons, no icons), next time in same thread 18 ms

 just native window
 speed:  934  6981  4200  2705  (14823), next time in same thread 8 ms
