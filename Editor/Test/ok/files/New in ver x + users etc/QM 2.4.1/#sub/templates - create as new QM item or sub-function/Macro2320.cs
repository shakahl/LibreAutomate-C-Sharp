win "" "" "" 0 F"callback={&sub.Callback_win_or_child}"


#sub Callback_win_or_child
function# hwnd cbParam

 Callback function for win or child.
 Read more about <help #IDP_ENUMWIN>callback functions</help>.

 hwnd - handle of the found window.
 cbParam - y argument of win or child.

 Return: 1 continue, 0 stop (let win or child return hwnd).


outw hwnd
ret 1
