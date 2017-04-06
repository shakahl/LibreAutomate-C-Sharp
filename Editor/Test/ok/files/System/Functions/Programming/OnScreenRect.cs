 /
function action [RECT&rect] ;;action: 1,0 show (begin or move), 2 hide, 3 temporarily hide

 Draws on-screen rectangle.

 rect - variable with rectangle coordinates. Optional if action is 2 or 3.

 REMARKS
 Before QM 2.3.4 this function was a dll function exported by qm.exe. It was unavailable in exe. rect was not optional; it was address of a RECT variable; now can be address or not. action is the same.
 Can be only 1 rectangle in thread.
 You can use class __OnScreenRect instead. Supports color, style and multiple rectangles in thread.

 EXAMPLES
 int w=win("" "CabinetWClass")
 RECT r; GetWindowRect w &r
 OnScreenRect 1 r
 2
 OnScreenRect 2

  red rectangle
 RECT rr; SetRect &rr 100 100 200 200
 __OnScreenRect osr.SetStyle(0xff)
 osr.Show(1 rr)
 2
 osr.Show(2)


__OnScreenRect-- osr
osr.Show(action rect)
