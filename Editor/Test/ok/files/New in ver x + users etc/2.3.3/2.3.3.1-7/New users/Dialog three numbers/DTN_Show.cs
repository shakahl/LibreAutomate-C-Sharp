 /DTN_1
function $OsdId $trigger x y color size

 Shows OSD three-numbers dialog.

 OsdId - any string that identifies the OSD. This function kills this OSD if exists.
 trigger - initial value to display.
 x, y - coordinates. If 0 - center. If negative - from right.
 color - text color in format 0xBBGGRR.
 size - font size.


 THIS CODE SHOWS DIALOG
 Basic code is created by the Dialog Editor. We add more code.

 Declare dialog variables. Optionally you can assign initial values.
str controls = "0 3"
str d0 e3
d0=F"dialog_three_numbers_osd_{OsdId}"
e3=trigger

clo win(d0 "#32770"); err ;;kill existing dialog

type DTN_DATA color size ;;define type to pass more info to the dialog procedure
DTN_DATA z; z.color=color; z.size=size ;;create and set variable of that type

 Show dialog.
 This code waits until the dialog is closed.
 If closed using OK button or Enter or by calling DT_Ok(), it returns 1, else it returns 0 and we exit (then macro ends).
if(!ShowDialog("dialog_three_numbers_osd" &dialog_three_numbers_osd &controls 0 0 0 0 &z x y)) ret
 Now we have dialog variables and can do whatever we want.
out e3
 ...

 then macro ends
