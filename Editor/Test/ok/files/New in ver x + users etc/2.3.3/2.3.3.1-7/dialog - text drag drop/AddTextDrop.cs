 /
function# hwndControl [flags] ;;flags: 1 modeless

 Creates a partially transparent window over a control.
 When you drop text on it, it transfers the text to the control.
 Not fully working. Just an example of how you can do it. Need much more code in dlg_TextDrop.

 hwndControl - control handle.
 flags:
   0 - wait until the control is destroyed (its window closed). Returns 1.
   1 - return immediately. Returns handle of the transparent window. Then the thread must not exit; it must process messages.


if(!IsWindow(hwndControl)) ret
int hDlg=ShowDialog("dlg_TextDrop" &dlg_TextDrop 0 hwndControl 1 0 0 hwndControl)

if(flags&1) ret hDlg
opt waitmsg 1
wait 0 -WC hDlg
ret 1
