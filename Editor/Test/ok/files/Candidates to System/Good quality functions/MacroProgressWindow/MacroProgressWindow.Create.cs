function [flags] [x] [y] [width] [height] ;;flags: 1 "End" button

 Creates macro progress window.

 x, y - window position. If 0, in screen center. If negative, relative to the right/bottom.
 width, height - window size. Default: 200 70.

 REMARKS
 This variable will close the window when dying.


if(m_hwnd) Destroy

int iid=getopt(itemid 3)
if(!width) width=200
if(!height) height=70

lock
int+ __g_mpw_hwnd=0
mac("MPW_Thread" "" iid flags x y width height)
wait 10 V __g_mpw_hwnd; err ret
m_hwnd=__g_mpw_hwnd
