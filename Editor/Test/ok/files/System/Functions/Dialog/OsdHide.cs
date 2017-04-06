 /
function [$osdid]

 Hides (stops displaying) on-screen text that is displayed by OnScreenDisplay.

 osdid - osdid used with OnScreenDisplay. If omitted or "", hides all.

 REMARKS
 OnScreenDisplay creates a transparent window where it draws the text. This function closes the window.
 Tip: to hide automatically when macro ends, use OnScreenDisplay with flag 8.

 Added in: QM 2.2.1.


win osdid "QM_OSD_Class" "" 0x8000 &sub.Enum 0


#sub Enum
function# hwnd lParam

clo hwnd; err ret 1
wait 1 -WC hwnd; err
ret 1
