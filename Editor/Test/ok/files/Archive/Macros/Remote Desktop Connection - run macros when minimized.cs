 On computer from which you run Remote Desktop Connection (RDC):
   1. Exit RDC, if running.
   2. Run this macro.
 Then UI macro commands (act, key...) will work when RDC window is minimized. I tested on Windows7, and it works.
 Found in http://smartbear.com/support/viewarticle/18731/

rset 2 "RemoteDesktop_SuppressWhenMinimized" "Software\Microsoft\Terminal Server Client" HKEY_LOCAL_MACHINE
