When a DockPanel instance is created, some operations with other unrelated controls of that thread become significantly slower.

I found the reason: FocusManagerImpl class installs WH_CALLWNDPROCRET hook. I know from my programming experience that it slows down sending messages.

My test results with a tree view control: adding 55000 items normally takes 570 ms. With DockPanel - 1260 ms. Slower 2.2 times. Because the hook makes each TVM_INSERTITEM slower. Deleting all the items is 3.4 times slower - 290 and 1002 ms. Because of TVN_DELETEITEM.

An alternative to WH_CALLWNDPROCRET could be WH_CBT/HCBT_SETFOCUS or SetWinEventHook(EVENT_OBJECT_FOCUS), however both don't have a "kill focus" event.

My current workaround is:
`//sm_localWindowsHook.Install();`