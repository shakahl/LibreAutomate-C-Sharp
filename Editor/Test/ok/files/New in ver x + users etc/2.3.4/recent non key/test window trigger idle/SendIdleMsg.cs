 /
function# hwnd timeout

 Calls SendMessageTimeout(0).
 Returns: 1 success, 0 timeout, -1 error


if(SendMessageTimeout(hwnd 0 0 0 0 timeout &_i)) ret 1
sel(GetLastError) case [0,ERROR_TIMEOUT] ret
ret -1


  Calls SendMessageTimeout. If possible, sends to a child of hwnd (more reliable, eg with IE).
  Returns: 1 success, 0 timeout, -1 error
 
 
 int w
  GUITHREADINFO g.cbSize=sizeof(g)
  if(!GetGUIThreadInfo(GetWindowThreadProcessId(hwnd 0
 if(hwnd=win) w=child
 if(!w or w!=GetAncestor(hwnd 2)) w=GetWindow(hwnd GW_CHILD)
 if(!w) w=hwnd
  outw w
 
 if(SendMessageTimeout(w 0 0 0 0 timeout &_i)) ret 1
 sel(GetLastError) case [0,ERROR_TIMEOUT] ret
 ret -1
