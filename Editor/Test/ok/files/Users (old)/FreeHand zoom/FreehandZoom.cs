 /
function size action ;;action: 0 set, 1 add, -1 sub

int handle=id(432 "FreeHand")
str s
if(action)
	s.getwintext(handle)
	int i=val(s)
	if(action>0) size=i+size
	else size=i-size
s=size
act handle
s.setsel(0 handle)
SendMessage(handle WM_KEYDOWN VK_RETURN 0)
