 /
function flags [cbFunc] [cbParam] [$accelMap] [accelHwnd] ;;flags: 1 set callback, 2 set accelerators, 0x100 enable dialog keyboard navigation

 Changes default processing of messages in function MessageLoop in this thread.

 flags:
   1 - use cbFunc/cbParam to set/change/remove callback function.
   2 - use accelMap/accelHwnd to set/change/remove keyboard accelerators.
   0x100 - let MessageLoop call IsDialogMessage. It enables standard dialog keyboard navigation in windows, eg Tab key selects controls that have WS_TABSTOP style.
 cbFunc, cbParam - address of a callback function, and something to pass to it.
 accelMap - keyboard accelerators. List of command id and hotkey pairs. Hotkeys are in <help>key</help> format. Example: "15 Ck[]16 F12[]...".
 accelHwnd - handle of a top-level window of current thread. If used, the accelerators will work only in this window.

 REMARKS
 If used flag 1 and cbFunc:
   Registers a callback function to be called for each message received by <help>MessageLoop</help>.
   Received are only posted messages, not sent. For example keyboard, mouse, timer and paint messages usually are posted.
   The function must begin with:
   function# MSG&m cbParam
     m - contains message information - window, message, etc. The function can modify it.
     cbParam - cbParam passed to MessageLoopOptions.
   If it returns a nonzero value, MessageLoop does not process the message and does not pass to the window.

 If used flag 2 and accelMap:
   Adds accelerators to windows of current thread.
   Accelerators are hotkeys that work in active windows of one thread. In this case - of this thread.
   When user presses an accelerator hotkey, the window receives WM_COMMAND message. The low-order word is accelerator id. The high-oder word is 1.
 
 All this is applied only when this thread uses <help>MessageLoop</help> or a function that calls it, eg <help>MainWindow</help>.
 Modal dialogs, message boxes, popup menus and other modal windows don't use MessageLoop.
 To receive messages posted to all windows of this thread, you can instead use SetWindowsHookEx(WH_GETMESSAGE &CbFunc 0 GetCurrentThreadId). Documented in MSDN.
 To add accelerators to dialogs, instead use <help>DT_SetAccelerators</help> or/and add menu bar with hotkeys.

 Added in: QM 2.3.5.


__MSGLOOP- ___t_ml
if flags&1
	___t_ml.cbFunc=cbFunc
	___t_ml.cbParam=cbParam
if flags&2
	___t_ml.accel.Create(accelMap)
	___t_ml.accelHwnd=accelHwnd
___t_ml.flags=flags
