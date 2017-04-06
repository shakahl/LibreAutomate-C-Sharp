 \

 is first item selected?
int hwnd=val(_command)
Acc a=acc(hwnd)
a.Navigate("first child1")
if(a.State&STATE_SYSTEM_FOCUSED) ret

 select first item using down arrow
PostMessage(hwnd WM_KEYDOWN VK_DOWN 0)

err+
