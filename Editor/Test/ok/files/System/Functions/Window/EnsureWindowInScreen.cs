 /
function hwnd [flags] [monitor] ;;flags: 1 resize if needed, 2 screen, 4 don't restore minimized, 32 monitor is hmonitor.  monitor: 0 same or nearest, 1-30 index, -1 mouse, -2 active window, -3 primary, or window handle

 If the window or part of it is not in the screen, moves it to the nearest position where it is completely in the screen.
 Does not throw errors.

 hwnd - window handle.
 flags:
   1 - resize if needed. By default the function can only move.
   2 - move into screen. By default the function moves into the work area.
 monitor - if not 0, move the window to the monitor. By default, moves into the same or nearest monitor.

 REMARKS
 For standard dialog windows instead can be used SendMessage(hDlg DM_REPOSITION 0 0). However it does not work with other windows.


int fl
fl=3|8
if(flags&1) fl|16
if(flags&2) fl~1
AdjustWindowPos hwnd 0 flags&32|fl monitor

if(flags&4=0 and min(hwnd)) res hwnd

err+
