 ABOUT

 If you have more than one keyboard, you can use this to make
 keyboard-specific triggers. For example, F7 trigger that
 works when you press F7 on your attached USB keyboard, but
 does not work when you press F7 on your notebook keyboard.
 The same with multiple mouses.
 Requirements: Windows XP or later, QM 2.1.5 or later.

 SETUP

 Keyboard_Detector function must run all the time. To run it
 automatically when QM starts, insert this in init2 function
 (create init2 function if does not exist):
 <code>
mac "Keyboard_Detector"
 </code>

 Run Keyboard_Detector (or this macro) two times to show the
 Keyboard Detector dialog. Associate keyboards and/or mouses
 with filter functions.

 Make sure that 'low level hook' is unchecked in Options/Triggers.
 Keyboard detector does not work if it is checked. Also keyboard
 triggers may stop working.

 Normally, if you detach a device and then attach again, you
 don't have to associate it with its filter function again.
 However, you may have to do this after you change a device,
 or attach to a different USB, or reinstall OS, etc.

 USAGE

 To make a keyboard trigger specific to a particular
 keyboard, assign it the filter function that is associated
 with that keyboard. Similarly you can make mouse-specific
 mouse triggers. To assign a filter function, in Properties
 click Filter, select Use, and select the function from the
 drop-down list.

 TROUBLESHOOTING

 If, when you press a hotkey, the macro does not run,
 probably Keyboard_Detector is not running.

 Keyboard Detector does not work perfectly. It is possible
 that ocassionally will be recognized incorrect keyboard or
 mouse, especially when pressing keys on two keyboards
 simultaneously, or when using two mouses simultaneously.
 Possibility to recognize incorrect keyboard is very low,
 even when pressing keys on two keyboards simultaneously.
 Possibility to recognize incorrect mouse is higher. If you
 click one mouse to run macro, you can move other mouse at
 the same time, but don't use buttons.
