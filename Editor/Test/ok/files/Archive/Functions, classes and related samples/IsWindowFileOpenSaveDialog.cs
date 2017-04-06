 /
function! hwnd

 Returns 1 if hwnd is handle of a file open or save dialog. Returns 0 if not.


if(!wintest(hwnd "" "#32770")) ret
if(!child("" "shelldll_defview" hwnd)) ret
if(!child(stc4 "" "Static" hwnd)) ret
if(!child(lst1 "" "ListBox" hwnd)) ret
if(!child(pshHelp "" "Button" hwnd)) ret
 Tested on win 2000, xp, vista, 7 beta. Not tested old style dialogs.
 On all OS these dialogs are different, also different are Open and Save As dialogs, etc, but always have these controls (most of them are hidden).

ret 1
err+ ret
