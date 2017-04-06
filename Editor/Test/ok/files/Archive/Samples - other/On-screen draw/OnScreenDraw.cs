 /
function# x y cx cy drawfunction [param] [transparency] [flags] [sizeof_param] [hwnd] ;;flags: 1 don't adjust coordinates

 Calls an user-defined function to perform on-screen drawing.
 The function can draw anything (shapes, text, icons, etc) using Windows API functions.
 The on-screen image disappears when the macro ends. To remove the image earlier, call OnScreenDrawEnd.
 Returns window handle that can be used with OnScreenDrawEnd. Also can be used to add controls, etc (not tested).

 x, y, cx, cy - bounding rectangle on screen. Depends on flag 1.
 drawfunction - address of user-defined function that performs drawing. Must begin with:
  function hwnd hdc cx cy param
  Arguments:
  hwnd - transparent window that is used for on-screen drawing.
  hdc - device context.
  cx, cy - width and height of bounding rectangle.
  param - value passed to OnScreenDraw.
 param - value to pass to the draw function.
 transparency - value between 1 (almost transparent) and 255 (opaque). If omitted or 0, the drawn image will be opaque.
 flags:
   1 - don't adjust coordinates. By default, 0 would be interpreted as screen center, and negative - as offset from screen right/bottom. Note: in newer QM versions the dialog always behaves as if flag 1 is set.
 sizeof_param - if used and not 0, copies memory whose address is param and size is sizeof_param, and passes address to the copied memory to the drawfunction as param.
 hwnd - draw window handle. It must be value returned by previous call of OnScreenDraw. If used, draws in the same window. Erases what is drawn previously. If not used or 0, creates new window.


type OSDA fa param cx cy transp flags str'pcopy
OSDA* d
if(hwnd) d=+DT_GetParam(hwnd); else d._new
d.fa=drawfunction
d.param=param
d.cx=cx
d.cy=cy
d.transp=transparency
d.flags=flags
if(sizeof_param) byte* p=+param; d.pcopy.fromn(p sizeof_param); d.param=d.pcopy

if(hwnd)
	SetWindowPos hwnd 0 x y cx cy SWP_NOZORDER
	RedrawWindow hwnd 0 0 RDW_INVALIDATE|RDW_ERASE
else if(flags&1)
	hwnd=ShowDialog("OSD_Dialog" &OSD_Dialog 0 0 1 0 WS_VISIBLE d)
	mov x y hwnd
	hid- hwnd
else
	hwnd=ShowDialog("OSD_Dialog" &OSD_Dialog 0 0 1 0 WS_VISIBLE d x y)
	hid- hwnd

atend OnScreenDrawEnd hwnd
opt waitmsg 2
0
ret hwnd
