  \Dialog_Editor
 Dont run this, see notes below
function# hDlg message wParam lParam
str s
if(hDlg) goto Dialog_Procedure

s.getsel
int selected(0)
if(s.len) selected=1
str controls = "3 4 5"
str edit1 edit2 edit3
if(!ShowDialog("Dialog4" &Dialog7 &controls _hwndqm)) ret
ret

 Accidental Reference to Function not Meant to be a Callback
 ===========================================================
 I renamed a Callback function, but forgot to change the name in the call similar to the above.
 Dialog7 had a dialog, and this command at the end: ShowDialog("Dialog7")
 The dialog appeared, but since the "callback function" only had the the showdialog command,
  for every message sent to it, the dialog would remain on screen - the dialog 
  could not be closed at all, not even CTRL-ALT-DEL & hitting endtask would kill it.
 Had to reboot windows since it crashed windows.

 Cant remember if it was Dialog4 or 7 that appeared, i figure it would have been both, 
 #4 called from here, and #7 called from 7

 Comment:
 Shouldn't there be a compile-time check of whether a Callback function referred to is a
 valid call-back function? But incase you have no way to be sure a function is a callback function,
 then perhaps a new Qm item called "Callback Function" would be suitable.