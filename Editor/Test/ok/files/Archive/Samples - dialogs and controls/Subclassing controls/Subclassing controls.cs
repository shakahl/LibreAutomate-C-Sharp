 Shortly about subclassing. More information can be found in the MSDN Library.

 In response to user input events Windows sends various messages to the window
 under the mouse pointer, or to the focused window. Various other messages are
 also sent, for example, when the window is created, destroyed, painted. As you
 probably know, controls are windows too. If the window is a dialog, we can
 intercept these messages in the dialog procedure. But if the window is a control,
 it only notifies the dialog about few events, by sending to the dialog WM_COMMAND
 and some other messages.

 If we want to intercept messages sent to a particular control, we can subclass it.
 If we want to intercept messages sent to all windows of a particular class, we can
 either create new class, or superclass an existing class. The second method is good
 when you want to use features of an existing class and add several new features.
 If we want to intercept certain messages sent to any window, including or not
 including windows of other programs, we can set hooks.

 To subclass a window, at first create a window procedure that will receive messages.
 It is an user-defined function, similar to an usual window procedure or dialog procedure.
 It must call CallWindowProc (not DefWindowProc) to forward messages for default processing.
 To subclass (begin receiving messages) a window, call SubclassWindow and pass
 address of the new window procedure. It returns the address of the default window
 procedure, which must be used as first argument of CallWindowProc.


str controls = "3"
str e3
e3="1[]2[]3[]4[]5[]6[]7[]8[]9"
if(!ShowDialog("dlg_with_subclassed_control" &dlg_with_subclassed_control &controls)) ret
