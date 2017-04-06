 /
function# [flags] ;;flags: 1 start if not found, 2 activate window

 Finds Selenium Server console window that was created by Selenium_StartServer.
 Returns window handle or 0.

 REMARKS
 If window not found, and flag 1, calls Selenium_StartServer without arguments. The server .jar file must be in QM folder, Selenium subfolder.
 Flag 2 can be used to activate new web browser window.


opt noerrorshere 1

int w=win("Selenium Server" "ConsoleWindowClass" "" 1)
if(!w and flags&1) w=Selenium_StartServer
if(flags&2) AllowActivateWindows; SetForegroundWindow(w)
ret w
