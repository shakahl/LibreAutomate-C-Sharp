 The following is from ShowWindow and WINDOWPLACEMENT documentation, with my notes.

0 SW_HIDE
Hides the window and activates another window.
Edit: does not activate another window.

1 SW_SHOWNORMAL
Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
Edit: activates only if was minimized. Activates only if the window is of same process (else deactivates current window, and then there is no active window).
Note: if minimized maximized, makes normal.

2 SW_SHOWMINIMIZED
Activates the window and displays it as a minimized window.
Edit: does not activate. Same as SW_SHOWMINNOACTIVE.

3 SW_SHOWMAXIMIZED
Activates the window and displays it as a maximized window.
Edit: same bugs as with SW_SHOWNORMAL.

4 SW_SHOWNOACTIVATE
Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except the window is not actived.
Edit: this value is similar to SW_RESTORE, not to SW_SHOWNORMAL.
Note: if minimized maximized, maximizes.

5 SW_SHOW
Activates the window and displays it in its current size and position. 
Edit: does not activate. Same as SW_SHOWNA.

6 SW_MINIMIZE
Minimizes the specified window and activates the next top-level window in the z-order.

7 SW_SHOWMINNOACTIVE
Displays the window as a minimized window. 
This value is similar to SW_SHOWMINIMIZED, except the window is not activated.

8 SW_SHOWNA
Displays the window in its current size and position. 
This value is similar to SW_SHOW, except the window is not activated.

9 SW_RESTORE
Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.
Edit: same bugs as with SW_SHOWNORMAL.
Note: if minimized maximized, maximizes.

10 SW_SHOWDEFAULT
Show as specified by the program that started window's program.
Note: ?.

11 SW_FORCEMINIMIZE
Minimize even if the thread is not responding.
Edit: also hides.
