function [^waitS]

 Waits until the object is pressed.
 Error on timeout.

 waitS - max number of seconds to wait. 0 is infinite.

 Tested and works:
 Standard button controls. Tested push button and check box.
 System buttons (Minimize etc).
 Buttons of standard toolbar controls.
 Taskbar buttons. Tested on Windows 7.
 
 Tested and does not work:
 Buttons in web pages. Tested in IE and FF.
 Buttons in MS Office dialogs.

 EXAMPLE
 Acc a=acc("Paste" "PUSHBUTTON" win("Text" "#32770") "Button" "" 0x1001)
 a.WaitForPressed(10)


if(waitS<0 or waitS>2000000) end ES_BADARG
opt waitmsg -1

int wt(waitS*1000) t1(GetTickCount)
rep
	0.01
	
	if(this.State&STATE_SYSTEM_PRESSED) ret
	
	if(wt and GetTickCount-t1>=wt) end "wait timeout"
