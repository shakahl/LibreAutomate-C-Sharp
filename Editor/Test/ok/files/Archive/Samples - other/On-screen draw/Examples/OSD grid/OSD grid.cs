int hwnd=OnScreenGrid(0 0 ScreenWidth ScreenHeight 32 1 ColorFromRGB(255 200 0) 160)
wait 2

 if need to change grid properties at run time, use hwnd
OnScreenGrid(0 0 ScreenWidth ScreenHeight 100 4 ColorFromRGB(0 200 0) 50 hwnd)
wait 2

 if need to remove the grid before the thread ends
OnScreenDrawEnd hwnd
