int w=win("Quick Macros" "QM_Editor") ;;your window

RECT r
GetClientRect w &r ;;get size of client area (ie without caption, border, menu)
MapWindowPoints w 0 +&r 2 ;;convert r to screen coordinates
ARRAY(int) a
if(!GetRectPixels(r a)) end "error" ;;get colors of all pixels in the rectangle

int col=a[300 35] ;;this is like in QM_Experts code, but don't need to call pixel() because we already have colors of all pixels in array a

 compare the color in all 6 other areas
if a[500 35]=col
	out 1
	 lef 500 35 w 1 ;;click
else if a[600 35]=col
	out 2
else if a[700 35]=col
	out 3
 and so on, for all 6 areas


 All coordinates (500, 35, etc) are relative to the client area of the window.

 Color values in a are in different format than with pixel(). Read more in GetRectPixels.

 Tip: To see client coordinates in QM status bar, check "Client" in Options dialog, Record tab.
