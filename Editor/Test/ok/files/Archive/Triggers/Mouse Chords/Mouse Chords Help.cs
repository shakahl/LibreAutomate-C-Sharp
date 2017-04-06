 Launches macros when you press two mouse buttons in certain sequence.
 It works as follows:

 1. You press the first mouse button, which is common for all chords. When you
    press it, runs function MC_First. It gets current time and stores it into
    global variable. Initially, the first button is the middle mouse button.
    To change, change MC_First trigger (you can use whatever mouse button, or
    other trigger, even non-mouse). Note: the first button is filtered. Don't
    set it to the left or right mouse button.

 2. You press the second mouse button, which is the trigger of the macro that you
    want to launch on mouse chord. At first runs its filter-function FF_MChords.
    It gets current time. If current time is within certain perion after the first
    button was pressed, it allows the macro to run, and also filters the second
    button. Otherwise, the macro doesn't run, and the second button is not filtered.
    To change the period during which the second button is active, change tWait
    value in MC_Init (initially it is 1000 ms).

 The My Mouse Chords folder contains several macros that run on mouse chords.
 All you have to do - write your own code. You can delete or disable macros that
 you don't need. You can optionally change some values in MC_Init.

 If your mouse has more than 5 buttons, some buttons probably are mapped to
 certain keystrokes. For example, if the 6-th button is mapped to F6, when you
 press Middle+6th, runs MC_Key_F6 macro.

 You can also make Middle+Key triggers (eg Middle+A) and Middle+MouseMovement
 triggers. Just create new macro, set an usual trigger, and assign filter function
 FF_MCChords. Then the trigger will be activated only within certain time period
 after the Middle mouse button was pressed.

 Initially, mouse chords works in all applications except Internet Explorer.
 To make it work only in certain application, right click Mouse Chords folder,
 select Folder Properties (or Properties, if your QM version is older), and set
 scope. Or set scope for individual macros.

 Initially does not work because the folder is disabled. Right click it and select Enable.

 Tip: If you will accidentally set mouse triggers so that the left mouse button
 will be always disabled, press Ctrl+Shift+Alt+D to disable QM.
