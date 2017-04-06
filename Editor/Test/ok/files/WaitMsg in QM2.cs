WAIT FOR SOMETHING
WaitKeysUp (wait for keys, called in loop). Used in 4 places, only in editor UI. All can be replaced with lambda or async etc.
WaitToRelease (wait for keys, called in loop). Used in 3 places, only in editor UI.
q_beginthread_wait (wait for thread). In portable setup. Can be replaced with something.
_RunMacro (wait for thread). Can be replaced with something.
RunSync -> RunSyncInNewThread (wait for thread). Used in 3-5 places. Maybe remove caller.
RegisterComComponent -> WaitForProcessAndGetExitCode (wait for process).
PipeHandle::ReadFileTimeout. Maybe .NET has something for it.

For 'wait for handle' use native MsgWaitForMultipleObjectsEx. Will not need to abort thread etc. Not used in scripts.
For 'wait for keys' use DoEvents + Sleep.


EASIER
TraySetFore. Can be replaced with lambda or async etc.
TrayProc(WM_RBUTTONUP). Can be replaced with timer etc.
wmUser(WMU20_TB_MOVE). Can be replaced with timer.
MyMacros.OnNotify(NM_RCLICK). Can be replaced with timer.
Record. Can be replaced with something, or removed. Maybe with DoEvents.
trig_InitKey. Try to replace with DoEvents.
_AutotextMenu_. Try to replace with DoEvents.
TriggersQmEvents::Event(TQ_TRAY_R). Try to replace with DoEvents + Sleep.
RunFromQM(minimize). Try to replace with DoEvents + Sleep.
KeysToSend::Wait. Used in <=4 places. Try to replace with DoEvents + Sleep.
HtmlGetDocument. Try to replace with DoEvents + Sleep.


ALREADY IMPLEMENTED IN CATKEYS
WaitForAnActiveWindow. Currently uses DoEvents + Sleep.


