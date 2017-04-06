
                                   PART I - Keys
                           
                              -----------------------

NEED T.S. MENU (OR SOMETHING SIMILAR) TO EAT ONE MORE KEY FOR TWO-KEY MACRO ACTIVATION

I realized that what is needed is for QM to "eat" the second key as well as the first. But you mention filtering all subsequent keys. I didnt think of that, but if u can get it to eat just ONE subsequent key typed, this would boost QM's power tremendously.

This would essentially turn EVERY keyboard key into a "Shift" key like Ctrl, Alt, etc, with the convenient distinction that it does not have to be held down to activate the next keystroke. Do you see the power here? It would create a two-key activation option for macros, instead of just one key. This would allow for a huge number of possible key activations for macros. (In practice, the alphabet keys may be unlikely to be used for the first trigger key, but a Function key, or a rarely used or conveniently placed key like ' , followed by a letter or number is highly convenient and efficient.)

Ive used this feature in several ways thus far: To set/reset global variables when i need to quickly change them for various macros. Also to branch to a function, passing the second key pressed as an argument to the function. Ive also used it for short commands like closing certain windows, activating menu's and buttons (i hate using the mouse...so 2 keystrokes to do this is faster and easier) etc. With 2-key option, i dont have to tie up all my Function keys, but just ONE Function key can be the trigger for over 50 macros (eg F12 followed by any other key from the whole keyboard.) So if we just use the function keys as the first trigger - thats over 600 two key activations! (And, though likely not practical, if we make the whole keyboard trigger-happy, thats over 50x50 combinations = over 2500 two-key combinations.)

Its so convenient that im using the feature as-is. I just have to make sure, when i type the trigger, that i dont have the caret in an editor window or notepad etc. where it would alter some text from the second key press.

------------------------------------------------

OBTAINING USER INPUT: WAIT FOR KEY PRESS
 
Wait for key press: can you have it wait for no specific key? ie have it wait for a key (or a specified # of keys), and then return the key code (or key sequence) of the key(s) pressed. Essentially duplicating the INPUT$() (which works without a loop and takes little or no processor time) or the INKEY$ (which, because its placed in a loop, can end up taking 100% processor time if more than one or two simple statements are between the "DO" and the "LOOP") of Microsoft QuickBasic. I have QuickBasic 7.1 (www.geocities.com/rsrapr) if u need to test it (or use QBasic 1.1 off the windows 95/98 cds). Its what im most familiar with at this time. 

"wait 5 K..." is just too restrictive right now - what if we want to wait for ANY key press, and then react according to what key is pressed?

Thinking about it, if you can implement this, you've essentially allowed the user to implement the TS menu request above! I can just then create a macro that waits for a key press, and then does a sel...case... to respond to user input.
---------------------------------------------------
Examples in QuickBasic

PRINT "Hit any key. ": DO: s$ = INKEY$ : LOOP UNTIL LEN(s$)   ' this returns ANY key
PRINT "Enter your choice: ": s$=INPUT$(1)    ' this returns only printable keys & ESC

In both cases: user does not have to hit the ENTER key - QBasic goes to the next statement immediately. INPUT$(n) waits for n # of key presses. INKEY$ gets a single key. Also in both cases: TEXT DOES NOT GET SENT TO THE SCREEN - the programmer has total control over what gets printed on the screen, and so essentially "eats" and filters user input. This is whats needed in QM, both for a better wait command that returns the key pressed (and "eats" it as well - doesnt print it on screen), and for the list and inp dialogs, noted later below.

The ultimate in QBasic: 
'INP(&H60) returns scancode of the last keypress or key release, so its more flexible than
' than the orthodox INKEY$ or INPUT$

Scancode% = INP(&H60)
IF Scancode% = 1 THEN PRINT "ESC pressed"
IF Scancode% = 129 THEN PRINT "ESC released"

' Scancode values can be found in the Qbasic help, or 
' just run the following routine to see them.
'Key release gives a key number + 128

ON TIMER(1) GOSUB printkey: TIMER ON
DO: WHILE LEN(INKEY$): WEND: LOOP

printkey:
Scancode%=INP(&H60): PRINT Scancode%: RETURN

'Or simply this
DO: PRINT INP(&H60): WHILE LEN(INKEY$): WEND: LOOP

Whereas INKEY$ returns a character from the keyboard buffer, and INPUT$(n) just waits for a character, INP(&H60) actually returns the last key press/release AT ANY POINT IN THE PROGRAM.

HOW ABOUT AN INP(&H60)-like function for QM? - The ultimate in detecting/filtering/evaluating keyboard input.

Can you get QM to have such a function. OR is there already a function available in WINAPI? I realize windows programming must be different than DOS programs, but if DOS programs can have this, why not windows programs? Windows is more advanced, is it not? I cant wait to hear an answer on this. :) :) :)





------------------
OBTAINING USER INPUT: INP, LIS DIALOGS

Talking about not having to hit the enter key......


inp:
Works like the QuickBasic INPUT (not the INPUT$(n)) statement: User has to hit ENTER

What i would like to see as well (as option, or as separate dialog) is something like the QuickBasic INPUT$(n) and/or INKEY$. Basically a function where i dont have to hit enter once a single (or predefined # of) character is entered. This is useful when just 1 or 2 or 3 characters need be entered. Also, perhaps, ability to error check input before function returns control to macro.

Im so use to this in my programming, it would be handy to have in QM as well. :)


lis: 
str s = "A)First item[]B) Second Item[]C) Third Item"
sel list(s "Menu")	
	case 1 out "A"
	case 2 out "B"
	case 3 out "C"
	case else out "Cancel"
In the box that comes up:
- need to be able to navigate without a mouse
- would like to be able to use arrow keys
- would like option to be able to type in first character of an item and have it immediately return control to the macro
- would like (option) to have it come to focus when it executes (currently u have to click in box to activate it)


------
Just as i finished writing this ive realized i can probably accomplish some of these things, though maybe more roughly or with more learning, using dialogs created using the dialog editor. For example, the "mes"s" dialog allows display of 3 buttons: when i enter a single letter, without having to hit enter, the macro then gets control again. Well i have not learned about dialogs as yet. Will do later.

I also just saw that eg. ifk(a 1) tells us if a key is toggled. So since QM tracks toggles of the whole keyboard, it think it should be easy to have QM reveal, and let us act on, more info about key presses by the user.




ATTACHED: 2 QBasic programs. 1) BETTERIN.BAS shows keyboard scancodes. 2) KBRDCODE.BAS shows  ASCII codes for keyboard input.

(By the way, if u use QBasic, heres something curious: The Win98 System Monitor, and the Coolbeans System Info (www.coolbeans.ws) show 100% processor usage at times when running certain QBasic routines (and with QBasic v4.5, it shows 100% with NO program running). However the Process Explorer (www.sysinternals.com) shows little effect on processor usage. Strange.)


--------



SUMMARY

1. TS MENU - two key macro activation in TS menu (or something similar: perhaps a new QM item, and/or a new way to define macro activation)
2. WAIT for any key & return key pressed. Option to eat the key. Option to specify 1 key or an n # of keys. Somewhat of a combination of the QBasic INKEY$ and INPUT$(n) functions.
3. Function: get last key pressed/released. Like QBASIC INP(&H60) function.
4. Better inp dialog: option to specify # of keys to be entered. Option to not need to hit ENTER
5. Better list dialog: auto focus, keyboard navigation, one key 'activation' upon hitting first letter of an item in list.




                                 PART II - Windows
        
                                 -----------------
                                    
SUMMARY

1. Need handle of windows created by "men" and "but" commands
2. Wait for "window x" or "window y"
3. "key" in a specified window, not if it is no longer in focus, or keep it focused when keying.



1. Expanding on val(command) - handles of subsequent windows opened by a macro.

When using "men" command or "but" command, it opens another window, and clicking a button in that window opens another window, etc. 

Need handle of those windows. If we use the window name, then what if another window with same name appears as the macro executes?  Need QM to give us handle of the window that appears when using "men" & "but" commands.

For example
men 2013 "Quick Macros";1
int x = win("Find")
out x

Here we could have other "Find" windows open, like the Windows 98 Find-files dialog, or the Notepad Find dialog...

I dont know how to go about this. Im thinking:

men 2013 "Quick Macros";1
int x = val(command)

where x would receive handle of most recently window activated by the macro.

or a modified men:

men(2013 "Quick Macros" h)

where h would receive the handle of the window opened by the "men" command.
Same thing for the "but" command if it opens a window.

I havent been working on it recently, but i really think this would help me in my AIM-Chat spam blocker macro; incoming messenger popup windows come sometimes at a high rate, sometimes simultaneously, and keeping track of the handles is vital for proper operation.

	men 668 sn
	3 "Send Warning" ;; can i get handle with val(command) ? incase other window
		err
			but 211 "Error"
			goto next
	but 1 "Send Warning"
	clo sn
	out "%s was warned at &i%%" sn wl
	
This is a part of that macro- i want to wait for "Send Warning" window to appear, but there may be other "Send Warning" windows open. I dont want to warn etc. the wrong person by clicking in another window. So i need to keep track of the window handles, as windows from buttons and menus are created/activated.


---------------
2. Wait command - waiting for window - conditional

wait command - can we do a conditional wait? eg wait 3 seconds for 
"window x" OR "window y" . Sometimes we dont know which window will appear.

In the above example i'm using an err statement to handle the case where one of two possible windows will appear (either "Send Warning" will appear, or "Error" will appear when i do the "men 668 sn" command)

So it would be far more convenient to have a wait command that can do an OR operation - wait for one of two (or more) windows to appear. Otherwise as i wait for one window to appear, another window may have appeared in its place and that wait was then for nothing.
-----------------

3.
Recording of menu items & key command extension

I tried the recorder, but cant seem to get it to insert the menu id's for any of the menus in Microsoft Word97. I tried Tools-->Record Menu, and the recorder (CSAr), i even tried the following:
 
men "&File\&Save" "Word"

Curious thing is that even QM133 doesnt show any menu id's in the status bar for the Word97 menus. Whats going on here? The status bar in QM2 shows;

File   MsoCommandBarPopup    Style=0x96000000(popup)
WINWORD

Where "File" refers to the menu (Can be File, Edit, View, Insert, etc.)
Mouse coordinates are also displayed there, and QM133 displays the same info.

Ok i've read the QM documentation and apparently this is non standard menu that you talk about. Then can we have it do a 

key Afs

AND be GUARANTEED THAT THE KEYS WILL BE PRESSED IN A SPECIFIC WINDOW? If another window suddenly pops up, for QM to keep keying can possibly cause catastrophic results. QM will need an expanded key statement for this (or have an option to stop keying if a window pops up). Without being able to specify a window, results are not always predictable. Or maybe im just being too demanding. :) o.o ^ ^ hehehe Happy April is just around the bend. 


"Intellectuals solve problems, geniuses prevent them." -Einstein








               PART III - Other notes (may be incoherent, sorry, out of time now)

                                      --------
                                      
1. ACTIVATION ALREADY EXISTS MESSAGE - NEED MORE INFO
 
When defining macro trigger, i get a message like:

  Activation 'F11' already exists (Macro x)

So i go to check Macro x, and find that i had previously disabled it, and dont need to worry about it at this point. However, QM should indicate in the output that the other macro (or macros?) is disabled. That way i dont have to take the extra time to look it up.


2. NETWORK BUG?

Network: im using QM on computer#2, i go to computer#1  and am presented with the dialog to reload file, i just leave (leaving that dialog on the screen) and come back an hour later and find my screen saver hasn't kicked in nor has monitor shut off as it should have - the screen is fully lit with that dialog on the screen. Both the computers are using the same macro file stored on computer #1. Seems that the reload dialog works like a CTRL-ALT-DEL or something, halting all processes until user responds? Well i havent tested this further, but i can if u need me to.



3. KEY IN A MOUSE CLICK AT CARET?

want: "key" a mouse click at the caret position, rather than at the pointer position
 if i can key a letter or number, why not a mouse click?
how else to activate a child window and have it click inside it without giving specific coordinates which are subject to change like the QM Find box has from QM208 to QM209.
I guess another way is to change the "act" command to have it click in the activated window? I dont know.

 eg. want do delete the text in the find input field
str f
f = "Find"
act id(1127 f)
rig f ;; doesnt click in the activated window, so the rest of this macro cant work.
key a
key X

 key (VK_RBUTTON) doesnt seem to do anything
 so im stuck with using "key HXXXXXXXXXXXXXXXX",
   or specifying x & y to click at.


4. ifk TOGGLE DETECT OF CTRL + character?

ifk statement & Shift keys - toggle: Any way to detect a toggle of CTRL-C, ALT-A, etc? Can do ifk(C 1) and ifk(c 1) ok, but how to do ifk(Cc 1), where Cc represents CTRL-C.


5. Possibly a "Plain Text" QM Item for note taking? It could also be used within macros for assigning text to a QM variable, without having to have [] entered at every new paragraph. A macro could use "x = textitem", where "textitem" is a "macro" where plain text is. It could be also used to easily send encrypted messages to friends who have QM... Just a basic "notepad" needed with no fancy editing features, except wordwrap. But notes can bloat the macro file, so if u do do this, may need to store notes in a separate file? So much to think about. Which brings us to the next topic (Macro management).



Good night. I mean good morning. But i bet is night there when its morning here. So good night. But i sleep in the morning and stay awake at night. So good morning! hahahahahahahaha 




 
-Ravinder Singh
27-Feb-2003
