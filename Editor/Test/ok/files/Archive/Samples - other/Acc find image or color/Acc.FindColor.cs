function# color [RECT&foundLocation] [scanFlags] [RECT&margins]

 Finds color within the accessible object.
 Uses <help "::/Functions/IDP_SCAN.html">scan</help>, which finds a single-pixel image created from the color.
 Returns 1 if found, 0 if not found, or throws error thrown by <color "0xff0000">scan</color> or Acc.Location.
 To get color value from screen, you can use <help "::/Functions/IDP_PIXEL.html">pixel</help> or the Wait dialog from the floating toolbar.

 color - color in 0xBBGGRR format.
 foundLocation - optional RECT variable that will receive screen coordinates of the color. Use 0 if not needed. Note that it is not used to limit the search area. It only receives results.
 scanFlags - same as flags argument of <color "0xff0000">scan</color>.
 margins - optional RECT variable that contains search area margins. For example, if margins.left is 20, the function will not search in first 20 pixels in the left of the object. Or, if margins.left is -30, the function also will search in 30 pixels to the left from the object.

 See also: <ColorFromRGB>.

 EXAMPLE
 Acc a=acc("Notification Area" "TOOLBAR" win("" "Shell_TrayWnd") "ToolbarWindow32" "" 0x1001)
 if(!a.FindColor(ColorFromRGB(0 0 255) 0 0x3)) ret
 out "found"


__MemBmp b.Create(1 1) ;;create 1x1 bitmap
SetPixel(b.dc 0 0 color)
int ok=this.FindImage(0 foundLocation scanFlags b.bm margins)
err end _error
ret ok
