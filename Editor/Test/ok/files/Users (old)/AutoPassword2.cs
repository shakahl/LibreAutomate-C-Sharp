 int hwnd
 web "http://xxxxxxxx" 0|8 "" "" 0 hwnd
 wait 1.5
 AutoPassword "username" "password" 0x0 win("xxxxxxxxx" "IEFrame")
 key TY;wait 2.5 ;;this for clicking login after entering name and password
 lef 506 422 0 1 ;;to click on something after i have logged on
 10
 clo hwnd

int hwnd
web "http://xxxxxxxx" 1|8 "" "" 0 hwnd
AutoPassword "username" "password" 0x1 hwnd 5
 find an element in the web page; wait max 30 s for the page/element
Htm el=htm("x" "x" "x" hwnd "" x x 30) ;;to insert this command, use dialog "Find html element"
el.Click
10
clo hwnd

 wait 0.5 removed because: 1. Flag 1 used in web. 2. A wait time used in AutoPassword.
 key TY removed because flag 1 can be used in AutoPassword.
 wait 2.5 removed because htm waits if you specify a wait time.
 lef 506 422 0 1 removed because el.Click can precisely click without the mouse.
