 Windows crashes while using Dialog Editor
 =========================================

 Background

My need: a way to assign a hot key to an edit box in a dialog

For this i tried to assign an ALT key to a Static text item, so that callback function
could process a WM_COMMAND message based on Static text, and then automatically
activate a dialog edit box for user to enter data.

ALT couldnt be processed for a Static text item, so i tried using an Option First, 
which allows ALT-digit processing. 
I then tried covering up the unneeded white option box, leaving only
the Option text visible. For this i tried a Static Text with "Topmost"
extended style.

I know very little about Styles and Extended styles.

 Problem

if a Static-text item is selected and an extended style of 0x8 (Topmost) is set for it:
	This subsequently causes windows to crash
	
For example, my approximate sequence of events that led to one of three crashs:
1) choose Static text, move it
2) choose Static text, move it
3) choose Option first, set extended style to 0x20
4) choose Static text, set extended style to 0x8
5) choose Static text, set extended style to 0x8

6) Minimize the dialog editor: the 0x8 style items remain on the screen!
7) Close dialog editor, choose yes to save

8) Qm has performed an "illegal operation", windows locks up
	-the first time it happened, i got another error after the Qm crash, 
	something regarding the kernal, and then windows locked

Im running Win98SE on a P2 333MHz processor with 440Meg RAM

Possible causes:
-minimizing the window after setting to 0x8
-restoring the window
-saving the dialog
-just choosing the 0x8 in combo with Static text
-using Static text with 0x8 in combo with other units



Update: i tried again, this time, only using a single Static text box set to 0x8
extended style, not moving it.

Then i minimized dialog editor, then restored it, then closed it, hit save, 
then QM crashed.

However, this time i was quickly able to use CTRL-ALT-DEL twice to reboot; 
before i was not able to even do this- i had to use the reset switch before


