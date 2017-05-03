noClipboardRestore (is restoring really useful? maybe remove, or let it be non-default)
waitMsg
slowMouse
slowKeys
fastKeys (false like QM2 keysync 2, true like QM2 keysync 1)
keyChar
hungWindow
noWarnings
noWarningsHere (not sure is it possible to implement)

hybridText


REMOVED
hidden - QM3 win() always skips hidden windows, unless using flag '+hidden'. When not using win() (eg Win.Activate("partial name")), always skips hidden.
waitcpu - almost useless.
keymark - don't need it because QM3 uses only low-level keyboard hooks.
err - cannot implement in C#. Use try/catch.
noerrorshere - cannot implement in C#. Use try/catch.
save, restore - for temporary options instead pass separate local variables to the functions. Or create a class that saves-restores, and then: using(var t=OptionsSaveRestore()) { _opt.speed=10; ... }



QM2
opt option [value]   ;;`Sets run-time option.`    option: hidden, clip, err, waitmsg, waitcpu, slowmouse, slowkeys, keymark, keysync (0 default, 1 none, 2 exe, 3 max), keychar, hungwindow (0 msgbox, 1 wait, 2 abort, 3 ignore), nowarnings, nowarningshere, noerrorshere.
opt save|restore   ;;`Saves or restores current opt and spe settings.`
