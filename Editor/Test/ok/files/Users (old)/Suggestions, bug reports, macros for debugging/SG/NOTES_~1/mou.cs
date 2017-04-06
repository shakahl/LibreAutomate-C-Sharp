 part of a macro to extract certain text in an AIM chat window
 run macro and observe the mouse movement & output of the y-coordinate before and after the move
 str s
out ym
mou 20 ym(0 win) win
out ym
 lef+
 mou+ 200 0
 out ym
 lef-
 s.getsel
 out s


 Comments:

 "ym" output should be the same number here before and after this move, since we are using 
	 "mou xx ym", ie, move to xcoordinate: xx, ycoordinate: current y location
 But, depending on the window in which macro is run, y location gains or loses pixels in the movement:

 in QM: loses 5 pixels
 in QM Help: gains 5 pixels
 in metapad: ym gained 100 pixels b4 reboot, 150 after
 in AIM chat window: loses 4
 in notepad: gained 22 b4 reboot, 110 after reboot 
 the numbers for notepad and metapad vary before and after reboot

 i worked around the problem, but still wondering why ym gains or loses pixels with the "mou xx ym" command


 --
 metapad (free) available at: http://www.liquidninja.com/metapad/