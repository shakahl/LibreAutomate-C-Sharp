 \
function [hwnd]

 Unhides the button and window.
 If hwnd is omitted, applies to all previously hidden windows.


#compile MTB_Main
if(getopt(nargs)) g_taskbar.UnhideButton(hwnd)
else g_taskbar.UnhideAll
