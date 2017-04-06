 Watches QM output pane for new text. On a QM run-time
 error, launches OU_OnRtError. To watch for other strings
 too, edit OU_Wndproc.

 Run OU_Main. To run automatically at startup, insert this
 in function init2 (if init2 does not exist, create):

mac "OU_Main"

 Be careful when editing OU_Wndproc and functions launched
 by it. OU_Wndproc runs in QM main thread. Incorrect
 programming can make QM unstable. Avoid endless recursion,
 which occurs when out in a macro causes the macro to run
 again.
