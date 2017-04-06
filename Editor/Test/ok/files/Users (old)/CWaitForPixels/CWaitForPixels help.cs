 Waits for colors of multiple pixels in a window or control.
 At first call Add for each pixel. x and y must be relative to the window that will be used with Wait. If flag 1 used - to its client area.
 Then call Wait.
 When color of one of added pixels match, the function returns. The return value is 0-based index of the pixel.
 On timeout the function throws error. Also if the window destroyed or something fails.
 The window should be visible, because the function gets pixels from screen, and uses the window only to get the rectangle.

 EXAMPLES

#compile CWaitForPixels
CWaitForPixels w
w.Add(0xffffff 200 10)
w.Add(0xf0f0f0 90 90)
out w.Wait(15 win("Notepad") 1)
